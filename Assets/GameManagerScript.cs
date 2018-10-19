using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManagerScript : MonoBehaviour {

    public GameObject player;
    public GameObject Seagull;
    public GameObject Crab;
    public GameObject CrabBurrow;
    public Camera MainCamera;
    public GameObject EggPrefab;
    public GameObject healthTokenPrefab;
    public GameObject Crosshair;
    public GameObject StatCounter;

    const int radius = 60;
    const int innerRadius = 25;
    const int outerRadius = 65;
    public AudioSource audioSrc;

    public TextMeshProUGUI scoreValue;

    public int CurrentScore;
    public int TotalScore;
    public bool FirstEggCollected;
    
    public AudioClip battleAudio;

    Vector3 battleCameraPosition = new Vector3(0.04f, 24.2f, 96);

    const float eggHeight = 7.37f;
    const float crabHeight = 1;
    const float healthTokenHeight = 2;
    public bool extraEggSpawned;

    // stage
    const int fightThreshold = 10;
    const int healthThreshold = 20;
    const int maxLevels = 5;
    SeagullHealthManager seagullHealthManager;
    public bool inBossFight;
    bool battleStarted;
    bool inCutscene;
    float healthSpawnTimeThreshold;
    float healthTokenSpawnCountdown;
    float crabSpawnCountdown = 5f;
    bool canSpawnCrab = true;
    public int currentLevel = 1;
    Coroutine currentCrabSpawn;
    public Boolean CameraMovingBack;

    // player health
    //const int maxHealth = 100;
    //int health = maxHealth;
    public Material playerMaterial;
    
    // death screen
    public Material cameraTintMaterial;
    bool gameEnded;
    public Color cameraDeathColour;
    [Range(0,1)] public float deathSaturationValue;
    public GameObject deathText;
    public GameObject scoreText;
    public GameObject healthText;
    public GameObject seagullText;
    public GameObject explosion;
    AudioSource seagullAudio;
    public AudioClip deathAudio;
    
    // victory screen
    public AudioClip victoryAudio;
    public GameObject credits;
    public Color cameraVictoryColour;
    [Range(0,1)] public float victorySaturationValue;

    void Start()
    {
        currentLevel = 1;
        FirstEggCollected = false;
        SpawnEgg(new Vector3(0, eggHeight, 0));
        cameraTintMaterial.SetColor("_Color", Color.white);
        cameraTintMaterial.SetFloat("_DesaturationValue", 0);
        seagullAudio = Seagull.GetComponent<AudioSource>();
        seagullHealthManager = Seagull.GetComponent<SeagullHealthManager>();

        // the first health token will spawn within 5 and 20s regardless of game mode
        healthTokenSpawnCountdown = Random.Range(5, 20);

        if (gameObject.GetComponent<GameSettings>().GetDifficulty() == 0)
        {
            healthSpawnTimeThreshold = 25;
            healthTokenPrefab.GetComponent<DespawnScript>().enabled = false;
        }
        else
        {
            healthSpawnTimeThreshold = 60;
            healthTokenPrefab.GetComponent<DespawnScript>().enabled = true;

        }
    }

    void Update ()
    {
        float delta = Time.deltaTime;

        if (TotalScore < 1 && inBossFight && !extraEggSpawned && seagullHealthManager.damageTaken < healthThreshold)
        {
            extraEggSpawned = true;
            SpawnEgg();
        }

        if (player.GetComponent<interaction>().GetPlayerHealth() <= 0 && !gameEnded)
        {
            PlayerDeath();
            gameEnded = true;
        }

        scoreValue.text = TotalScore.ToString();
        
        // if both the camera scripts are disabled, look at the seagull (if both are disabled we are in a transition)
        if (inCutscene)
        {
            MainCamera.transform.LookAt(Seagull.transform);
        }
        

        if (inBossFight && Seagull.GetComponent<SeagullBossController>().IsOnGround() && !MainCamera.GetComponent<BossFightThirdPersonCameraController>().enabled)
        {
            MainCamera.GetComponent<BossFightThirdPersonCameraController>().enabled = true;
        }

        // randomly spawn crabs
        crabSpawnCountdown -= delta;
        if (crabSpawnCountdown <= 0 && FirstEggCollected && canSpawnCrab)
        {
            SpawnCrab();
            crabSpawnCountdown = Random.Range(5, 10);
        }

        // randomly spawn health tokens
        healthTokenSpawnCountdown -= delta;
        if (FirstEggCollected && healthTokenSpawnCountdown <= 0 && !gameEnded){
            SpawnHealthToken();
            healthTokenSpawnCountdown = Random.Range(5, healthSpawnTimeThreshold);
        }

        /* ============================================ SEAGULL CONTROL ============================================ */

        // start boss battle if score threshold satisfied
        if (CurrentScore > 0 && CurrentScore % fightThreshold == 0 && !inBossFight)
        {
            Debug.Log("starting battle");
            CurrentScore = 0;
            inBossFight = true;
            StartBattle();
        }

        // run battle start initiation method when Seagull reaches the ground
        if (Seagull.GetComponent<SeagullBossController>().IsOnGround() && !battleStarted)
        {
            Debug.Log("battle started");
            OnBattleStart();
            battleStarted = true;            
        }

        // set the camera position while the seagull is transitioning from air to ground
        if (!Seagull.GetComponent<SeagullBossController>().IsOnGround() && !battleStarted && inBossFight)
        {
            MainCamera.transform.position = battleCameraPosition;
        }
        
        // trigger the end battle sequence
        // need to change the conditions
        if (seagullHealthManager.damageTaken >= healthThreshold && inBossFight && currentLevel < maxLevels
                && seagullHealthManager.seagullHealth >= 0)
        {
            Debug.Log("ending battle");
            SeagullBossController SBC = Seagull.GetComponent<SeagullBossController>();
            bossControls BC = player.GetComponent<bossControls>();
            
            if (!SBC.attacksLocked){
                SBC.attacksLocked = true;
            }

            if (BC.canAttack)
            {
                BC.canAttack = false;
            }

            if (SBC.IsIdle()){
                SBC.TakeOff();
                EndBattle();
            }
        }

        if (seagullHealthManager.seagullHealth <= 0 && !gameEnded)
        {
            canSpawnCrab = false;
            DestroyCrabs();
            SeagullBossController SBC = Seagull.GetComponent<SeagullBossController>();
            bossControls BC = player.GetComponent<bossControls>();
            
            if (!SBC.attacksLocked){
                SBC.attacksLocked = true;
            }

            if (BC.canAttack)
            {
                BC.canAttack = false;
            }
            
            if (SBC.IsIdle())
            {
                gameEnded = true;
                EndGame();
            }
        }

        // once the seagull has returned to the sky, call OnBattleEnd to reenable movement and camera
        if (Seagull.GetComponent<SeagullFlightController>().isFlying && battleStarted)
        {
            Debug.Log("battle ended!");
            battleStarted = false;
            OnBattleEnd();
        }
        
        /* ========================================================================================================= */
    }

    public void OnFirstEggCollect()
    {
        FirstEggCollected = true;
        Seagull.gameObject.SetActive(true);
        StartCoroutine(ChangeMusic(battleAudio, 0.5f));
        seagullText.SetActive(true);
        healthText.SetActive(true);
        scoreText.SetActive(true);
    }
    
    void StartBattle()
    {

        //ENSURE REMOVAL OF EGGS AND HEALTH TOKENS (TO COMBAT HAVING ONES ON MAP THAT CANT BE REACHED)
        inCutscene = true;
        SeagullFlightController seagullFlight = Seagull.GetComponent<SeagullFlightController>();
        seagullFlight.enabled = false;
        SeagullBossController seagullBoss = Seagull.GetComponent<SeagullBossController>();
        seagullBoss.enabled = true;
        //seagullBoss.totalTime = seagullFlight.totalTime;

        MainCamera.GetComponent<ThirdPersonCameraController>().enabled = false;
        MainCamera.GetComponent<BossFightThirdPersonCameraController>().enabled = false;
        MainCamera.GetComponent<Transform>().LookAt(transform);

        player.transform.position = new Vector3(0, 2, 75);
        player.transform.LookAt(new Vector3(0, player.transform.position.y, 0));
        player.GetComponent<bossControls>().enabled = false;
        player.GetComponent<movement>().enabled = false;

        canSpawnCrab = false;
        DestroyCrabs();
    }

    public void OnBattleStart()
    {
        inCutscene = false;
        player.GetComponent<bossControls>().enabled = true;
        player.GetComponent<movement>().enabled = true;
        player.GetComponent<bossControls>().canAttack = true;
        MainCamera.GetComponent<BossFightThirdPersonCameraController>().enabled = true;
        Crosshair.SetActive(true);
        canSpawnCrab = true;
        DestroyEggs();
        DestroyHealthTokens();
    }
    
    void EndBattle()
    {
        inBossFight = false;
        inCutscene = true;
        SeagullFlightController seagullFlight = Seagull.GetComponent<SeagullFlightController>();
        seagullFlight.enabled = true;
        SeagullBossController seagullBoss = Seagull.GetComponent<SeagullBossController>();
        seagullBoss.enabled = false;
        
        // increment levels
        Seagull.GetComponent<SeagullFlightController>().currentLevel++;
        currentLevel++;
        if (seagullBoss.NUM_OF_ATTACKS < 4) seagullBoss.NUM_OF_ATTACKS++;
        
        MainCamera.GetComponent<BossFightThirdPersonCameraController>().enabled = false;
        
        player.GetComponent<bossControls>().enabled = false;
        player.GetComponent<movement>().enabled = false;

        seagullHealthManager.seagullHealth += seagullHealthManager.damageTaken - healthThreshold;
        seagullHealthManager.damageTaken = 0;
        Crosshair.SetActive(false);
        canSpawnCrab = false;
        DestroyCrabs();
    }

    void OnBattleEnd()
    {
        inBossFight = false;
        battleStarted = false;
        inCutscene = false;
        
        player.GetComponent<movement>().enabled = true;
        MainCamera.GetComponent<ThirdPersonCameraController>().enabled = true;
        MainCamera.GetComponent<BossFightThirdPersonCameraController>().enabled = false;
        player.GetComponent<bossControls>().enabled = false;

        DestroyEggs();
        canSpawnCrab = true;
        SpawnEgg();
    }

    void EndGame()
    {
        StatCounter.GetComponent<StatCounterScript>().finalEggCount = TotalScore;
        StatCounter.GetComponent<StatCounterScript>().gameEnded = true;
        StopAllCoroutines();
        StartCoroutine(ChangeMusic(victoryAudio, 0.5f));
        
        healthText.SetActive(false);
        scoreText.SetActive(false);
        seagullText.SetActive(false);
        Seagull.GetComponent<SeagullBossController>().enabled = false;
        Seagull.GetComponent<Animator>().SetTrigger("IdleToDie");
        Crosshair.SetActive(false);

        player.GetComponent<bossControls>().enabled = false;
        player.GetComponent<movement>().enabled = false;
        player.GetComponent<interaction>().enabled = false;
        player.GetComponent<Animator>().SetFloat("motion", 0);

        StartCoroutine(ShowCredits());
    }

    // spawn a crab in a random location
    public void SpawnCrab()
    {
        Vector3 pos = new Vector3(Random.Range(-radius, radius), crabHeight, Random.Range(-radius, radius));
        Quaternion rotation = Quaternion.identity;
        rotation.eulerAngles = new Vector3(-90, 0, 0);
        Instantiate(CrabBurrow, pos + new Vector3(0,crabHeight,0), rotation);
        currentCrabSpawn = StartCoroutine(WaitToSpawn(pos));
    }

    // spawn a crab in a specific location
    public void SpawnCrab(Vector3 pos)
    {
        Instantiate(CrabBurrow, pos + new Vector3(0, crabHeight, 0), Quaternion.LookRotation(transform.position, Vector3.up));
        currentCrabSpawn = StartCoroutine(WaitToSpawn(pos));
    }

    IEnumerator WaitToSpawn(Vector3 pos)
    {
        yield return new WaitForSeconds(5f);
        Instantiate(Crab, pos, Crab.transform.rotation);
    }

    // spawn an egg in a random location
    public void SpawnEgg()
    {
        Vector3 pos;

        if (!inBossFight)
        {
            pos = new Vector3(Random.Range(-radius, radius), eggHeight, Random.Range(-radius, radius));
        }
        else
        {
            Vector3 randomCirclePoint = Random.insideUnitCircle.normalized;
            randomCirclePoint.z = randomCirclePoint.y;
            randomCirclePoint.y = 0;
            randomCirclePoint *= Random.Range(innerRadius + 5, outerRadius);
            pos = randomCirclePoint;
            //pos = new Vector3(Mathf.Sin(Random.Range(0, Mathf.PI)) * innerRadius, healthTokenHeight, Mathf.Cos(Random.Range(0, Mathf.PI)) * innerRadius);
            //int offset = Random.Range(1, outerRadius - innerRadius);
            //pos.x = pos.x >= 0 ? pos.x += offset : pos.x -= offset;
            //pos.z = pos.z >= 0 ? pos.z += offset : pos.z -= offset;
        }

        Instantiate(EggPrefab, pos, Quaternion.identity);
    }

    // spawn an egg in a specific location
    public void SpawnEgg(Vector3 pos)
    {
        Instantiate(EggPrefab, pos, Quaternion.identity);
    }

    // spawn a Health token in a random location
    public void SpawnHealthToken()
    {
        Vector3 pos;

        if (!inBossFight)
        {
            pos = new Vector3(Random.Range(-radius, radius), healthTokenHeight, Random.Range(-radius, radius));
        } else {
            Vector3 randomCirclePoint = Random.insideUnitCircle.normalized;
            randomCirclePoint.z = randomCirclePoint.y;
            randomCirclePoint.y = 0;
            randomCirclePoint *= Random.Range(innerRadius+5, outerRadius);
            pos = randomCirclePoint;
            //pos = new Vector3(Mathf.Sin(Random.Range(0,Mathf.PI))*innerRadius, healthTokenHeight, Mathf.Cos(Random.Range(0, Mathf.PI))*innerRadius);
            //int offset = Random.Range(1, (outerRadius - innerRadius)/2);
            //pos.x = pos.x >= 0 ? pos.x += offset : pos.x -= offset;
            //pos.z = pos.z >= 0 ? pos.z += offset : pos.z -= offset;
        }

        Instantiate(healthTokenPrefab, pos, Quaternion.identity);
    }

    void PlayerDeath()
    {
        seagullAudio.mute = true;
        StartCoroutine(ChangeMusic(deathAudio, 0.5f));
        Instantiate(explosion, player.transform);
        playerMaterial.SetColor("_Color", Color.white);
        cameraTintMaterial.SetColor("_Color", cameraDeathColour);
        cameraTintMaterial.SetFloat("_DesaturationValue", deathSaturationValue);
        healthText.SetActive(false);
        scoreText.SetActive(false);
        deathText.SetActive(true);
        seagullText.SetActive(false);
        player.gameObject.SetActive(false);
        MainCamera.GetComponent<ThirdPersonCameraController>().enabled = false;
        MainCamera.GetComponent<BossFightThirdPersonCameraController>().enabled = false;
    }

    private void DestroyCrabs()
    {
        GameObject[] crabs = GameObject.FindGameObjectsWithTag("Crab");
        for(int i=0; i < crabs.Length; i++)
        {
            Destroy(crabs[i]);
        }
        StopCoroutine(currentCrabSpawn);
    }

    void DestroyEggs()
    {
        GameObject egg = GameObject.FindWithTag("Egg");
        Destroy(egg);
    }

    void DestroyHealthTokens(){
        GameObject[] tokens = GameObject.FindGameObjectsWithTag("HealthToken");
        for (int i=0; i < tokens.Length; i++){
            Destroy(tokens[i]);
        }
    }

    IEnumerator ChangeMusic(AudioClip newMusic, float rate)
    {
        float startVolume = audioSrc.volume;
        while (audioSrc.volume > 0)
        {
            audioSrc.volume -= startVolume * Time.deltaTime / rate;
            yield return null;
        }
        
        if (audioSrc.volume <= 0)
        {
            audioSrc.volume = startVolume;
            audioSrc.clip = newMusic;
            audioSrc.Play();
        }
    }

    IEnumerator ShowCredits()
    {
        yield return new WaitForSeconds(4.3f);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        cameraTintMaterial.SetColor("_Color", cameraVictoryColour);
        cameraTintMaterial.SetFloat("_DesaturationValue", victorySaturationValue);
        credits.SetActive(true);
        yield return new WaitForSeconds(40f);
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
}