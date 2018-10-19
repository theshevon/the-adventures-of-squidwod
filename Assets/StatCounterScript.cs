using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatCounterScript : MonoBehaviour
{

	public int eggsCollected;
	public int eggsThrown;
	public int finalEggCount;

	public int damageTaken;
	public int numberJumps;
	public float survivalTime;
	public bool gameEnded;

	public TextMeshProUGUI eggsCollectedValue;
	public TextMeshProUGUI eggsThrownValue;
	public TextMeshProUGUI finalEggCountValue;
	public TextMeshProUGUI damageTakenValue;
	public TextMeshProUGUI numberJumpsValue;
	public TextMeshProUGUI survivalTimeValue;
	
	// Use this for initialization
	void Start ()
	{
		survivalTime = 0;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(!gameEnded) survivalTime += Time.deltaTime;
		else
		{
			eggsCollectedValue.SetText(eggsCollected.ToString());
			eggsThrownValue.SetText(eggsThrown.ToString());
			finalEggCountValue.SetText(finalEggCount.ToString());
			damageTakenValue.SetText(damageTaken.ToString());
			numberJumpsValue.SetText(numberJumps.ToString());
			int min = Mathf.FloorToInt(survivalTime / 60);
			int sec = Mathf.FloorToInt(survivalTime % 60);
			survivalTimeValue.SetText(min.ToString("00") + ":" + sec.ToString("00"));
		}
	}
}
