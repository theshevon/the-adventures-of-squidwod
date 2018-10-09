using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class interaction : MonoBehaviour {

	public GameManagerScript gameManager;
	private GameObject GameManagerObject;
	private GameManagerScript GameManager;

	void Start()
	{
		GameManagerObject = GameObject.FindWithTag("GameController");
		GameManager = GameManagerObject.GetComponent<GameManagerScript>();
	}
	
	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.CompareTag("Egg"))
		{
			gameManager.SpawnEgg();
			GameManager.CurrentScore += 1;
			Destroy(col.gameObject);
		}

		else if (col.gameObject.CompareTag("LoseLife"))
		{
			Debug.Log("Squid loses a life!");
		}
	}
}