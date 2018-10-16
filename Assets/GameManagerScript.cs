using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.SocialPlatforms;
using Random = UnityEngine.Random;

public class GameManagerScript : MonoBehaviour {


    public GameObject player;
    public GameObject Seagull;
    public GameObject Crab;
    public GameObject CrabBurrow;
    public Camera MainCamera;
    public GameObject EggPrefab;

    public Vector3 centre;
    public int diameter;
    public AudioSource audioSrc;
    public TextMeshProUGUI scoreValue;

    public int CurrentScore;
    public int TotalScore;
    public bool FirstEggCollected;
    
    public AudioClip battleAudio;

    Vector3 battleCameraPosition = new Vector3(0.04f, 24.2f, 96);

    const float eggHeight = 7.37f;
    const float crabHeight = 2;

    // stage
    const int fightThreshold = 10;
    const int healthThreshold = 20;
    SeagullHealthManager seagullHealthManager;
    public bool inBossFight;
    bool battleStarted;
    private bool inCutscene;
    float countdown = 5f;
    bool canSpawnCrab = true;
    public int stage;
    
    // player health
    private int health;
    public Material playerMaterial;
    
    // death screen
    public Material cameraTintMaterial;
    private bool gameEnded = false;
    public Color cameraDeathColour;
    [Range(0,1)] public float deathSaturationValue;
    public GameObject deathText;
    public GameObject scoreText;
    public GameObject healthText;
    public GameObject explosion;
    private AudioSource seagullAudio;
    public AudioClip deathAudio;
    

    void Start()
    {
        FirstEggCollected = false;
        SpawnEgg(new Vector3(0, eggHeight, 0));
        cameraTintMaterial.SetColor("_Color", Color.white);
        cameraTintMaterial.SetFloat("_DesaturationValue", 0);
        seagullAudio = Seagull.GetComponent<AudioSource>();
        seagullHealthManager = Seagull.GetComponent<SeagullHealthManager>();
    }

    void Update ()
    {
        if (player.GetComponent<interaction>().health <= 0 && !gameEnded)
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
        countdown -= Time.deltaTime;
        if (countdown <= 0 && FirstEggCollected && canSpawnCrab)
        {
            SpawnCrab();
            countdown = Random.Range(5, 10);
        }

        /* ============================================ SEAGULL CONTROL ============================================ */
        
        // start boss battle if score threshold satisfied
        // hotkey added for testing purposes
        if (((CurrentScore > 0 && CurrentScore % fightThreshold == 0) || Input.GetKeyDown(KeyCode.B)) && !inBossFight)
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
        if (((seagullHealthManager.damageTaken > healthThreshold) || Input.GetKeyDown(KeyCode.N)) && inBossFight)
        {
            Debug.Log("ending battle");
            SeagullBossController SBC = Seagull.GetComponent<SeagullBossController>();

            if (!SBC.attacksLocked){
                SBC.attacksLocked = true;
            }

            if (SBC.IsIdle()){
                SBC.TakeOff();
                EndBattle();
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
        audioSrc.clip = battleAudio;
        audioSrc.Play();
    }
    
    void StartBattle()
    {
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
        MainCamera.GetComponent<BossFightThirdPersonCameraController>().enabled = true;

        canSpawnCrab = true;
    }
    
    void EndBattle()
    {

        inBossFight = false;
        inCutscene = true;
        SeagullFlightController seagullFlight = Seagull.GetComponent<SeagullFlightController>();
        seagullFlight.enabled = true;
        SeagullBossController seagullBoss = Seagull.GetComponent<SeagullBossController>();
        seagullBoss.enabled = false;
        
        MainCamera.GetComponent<BossFightThirdPersonCameraController>().enabled = false;
        
        player.GetComponent<bossControls>().enabled = false;
        player.GetComponent<movement>().enabled = false;

        seagullHealthManager.damageTaken = 0;

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

        canSpawnCrab = true;
    }

    // spawn a crab in a random location
    public void SpawnCrab()
    {
        Vector3 pos = centre + new Vector3(Random.Range(-diameter / 2, diameter / 2), crabHeight, Random.Range(-diameter / 2, diameter / 2));
        Quaternion rotation = Quaternion.identity;
        rotation.eulerAngles = new Vector3(-90, 0, 0);
        Instantiate(CrabBurrow, pos + new Vector3(0,crabHeight,0), rotation);
        StartCoroutine(WaitToSpawn(pos));
    }

    // spawn a crab in a specific location
    public void SpawnCrab(Vector3 pos)
    {
        Instantiate(CrabBurrow, pos + new Vector3(0, crabHeight, 0), Quaternion.LookRotation(transform.position, Vector3.up));
        StartCoroutine(WaitToSpawn(pos));
    }

    IEnumerator WaitToSpawn(Vector3 pos)
    {
        yield return new WaitForSeconds(5f);
        Instantiate(Crab, pos, Crab.transform.rotation);
    }

    // spawn an egg in a random location
    public void SpawnEgg()
    {
        Vector3 pos = centre + new Vector3(Random.Range(-diameter / 2, diameter / 2), eggHeight, Random.Range(-diameter / 2, diameter / 2));
        Instantiate(EggPrefab, pos, Quaternion.identity);
    }

    // spawn an egg in a specific location
    public void SpawnEgg(Vector3 pos)
    {
        Instantiate(EggPrefab, pos, Quaternion.identity);
    }

    private void PlayerDeath()
    {
        seagullAudio.mute = true;
        audioSrc.clip = deathAudio;
        audioSrc.Play();
        Instantiate(explosion, player.transform);
        playerMaterial.SetColor("_Color", Color.white);
        cameraTintMaterial.SetColor("_Color", cameraDeathColour);
        cameraTintMaterial.SetFloat("_DesaturationValue", deathSaturationValue);
        healthText.SetActive(false);
        scoreText.SetActive(false);
        deathText.SetActive(true);
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
    }
}