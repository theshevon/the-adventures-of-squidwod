﻿using System.Collections;
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
    const int maxHealth = 100;
    int health = maxHealth;

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
		audioSrc.PlayOneShot(hurt[index]);
	}

    public int GetPlayerHealth(){
        return health;
    }
}