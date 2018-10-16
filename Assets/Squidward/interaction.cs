using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class interaction : MonoBehaviour {

	public GameManagerScript gameManager;
	private GameObject GameManagerObject;
	private GameManagerScript GameManager;
	
	public TextMeshProUGUI healthValue;

	private bool isDamaged;
	public Material playerMaterial;
	private Color damageColour = new Color(1.0f, 0.57f, 0.57f, 1.0f);
	public int health = 100;

	private AudioSource audioSrc;
	public AudioClip[] hurt;

	void Start()
	{
		audioSrc = GetComponent<AudioSource>();
		GameManagerObject = GameObject.FindWithTag("GameController");
		GameManager = GameManagerObject.GetComponent<GameManagerScript>();
	}

	void Update()
	{
		healthValue.SetText(health.ToString());
	}
	
	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.CompareTag("Egg"))
		{
			if (!GameManager.FirstEggCollected) gameManager.OnFirstEggCollect();
            if (!GameManager.inBossFight) gameManager.SpawnEgg();
			GameManager.TotalScore += 1;
			GameManager.CurrentScore += 1;
			Destroy(col.gameObject);
		}
		else if (col.gameObject.CompareTag("Crab") && !isDamaged)
		{
			StartCoroutine(TakeDamage());
			Destroy(col.gameObject);
		} 
	}

	void OnTriggerEnter(Collider col)
	{
		if ((col.gameObject.CompareTag("LoseLife") || col.gameObject.CompareTag("Flame")) && !isDamaged)
		{
			StartCoroutine(TakeDamage());
		}
	}

	IEnumerator TakeDamage()
	{
		PlayHurtSound();
		health -= 10;
		playerMaterial.SetColor("_Color", damageColour);
		isDamaged = true;
		yield return new WaitForSeconds(0.5f);
		playerMaterial.SetColor("_Color", Color.white);
		isDamaged = false;
	}

	void PlayHurtSound()
	{
		int index = Random.Range(0, hurt.Length);
		Debug.Log(index);
		audioSrc.PlayOneShot(hurt[index]);
	}
}