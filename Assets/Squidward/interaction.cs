using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class interaction : MonoBehaviour {

	public GameManagerScript gameManager;
	GameObject GameManagerObject;
	GameManagerScript GameManager;
    int damage;
	
	public TextMeshProUGUI healthValue;
    public Image playerHealthBar;

	bool isDamaged;
	public Material playerMaterial;
	Color damageColour = new Color(1.0f, 0.57f, 0.57f, 1.0f);
    const int maxHealth = 100;
    int health = maxHealth;

	AudioSource audioSrc;
	public AudioClip[] hurt;
	public AudioClip pickup;

	void Start()
	{
		audioSrc = GetComponent<AudioSource>();
		GameManagerObject = GameObject.FindWithTag("GameController");
		GameManager = GameManagerObject.GetComponent<GameManagerScript>();

        damage = gameObject.GetComponent<GameSettings>().GetDifficulty() == 0 ? 5 : 10;
    }

	void Update()
	{
		healthValue.SetText(health.ToString());
        playerHealthBar.fillAmount = health / 100f;
	}
	
	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.CompareTag("Egg"))
		{
            GameManagerScript GMS = GameManager.GetComponent<GameManagerScript>();
			if (!GameManager.FirstEggCollected) gameManager.OnFirstEggCollect();
            if (!GameManager.inBossFight) gameManager.SpawnEgg();
            if (GameManager.inBossFight)
            {
                GMS.extraEggSpawned = false;
                GetComponent<bossControls>().canAttack = true;
            }
			GameManager.TotalScore += 1;
			GameManager.CurrentScore += 1;
			audioSrc.PlayOneShot(pickup, 0.7f);
			Destroy(col.gameObject);
		}

        if (col.gameObject.CompareTag("Crab") && !isDamaged)
		{
			StartCoroutine(TakeDamage());
			Destroy(col.gameObject);
		} 

        if (col.gameObject.CompareTag("HealthToken")){
            health += 10;
            if (health > maxHealth)
            {
                health = maxHealth;
            }
            Destroy(col.gameObject);
        }
	}

	void OnTriggerEnter(Collider col)
	{
		if ((col.gameObject.CompareTag("Laser") || col.gameObject.CompareTag("Flame")) && !isDamaged)
		{
			StartCoroutine(TakeDamage());
		}
	}

	public void OnPlayerHit()
	{
		if (!isDamaged) StartCoroutine(TakeDamage());
	}
	
	IEnumerator TakeDamage()
	{
		PlayHurtSound();
		health -= damage;
		playerMaterial.SetColor("_Color", damageColour);
		isDamaged = true;
		yield return new WaitForSeconds(0.5f);
		playerMaterial.SetColor("_Color", Color.white);
		isDamaged = false;
	}

	void PlayHurtSound()
	{
		int index = Random.Range(0, hurt.Length);
		audioSrc.PlayOneShot(hurt[index]);
	}

    public int GetPlayerHealth(){
        return health;
    }
}