using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class interaction : MonoBehaviour {
	public EggSpawner eggSpawner;
	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.CompareTag("Egg"))
		{
			Debug.Log("Squid hit egg!");
			eggSpawner.SpawnEgg();
			Destroy(col.gameObject);
		}
		else if (col.gameObject.CompareTag("LoseLife"))
		{
			Debug.Log("Squid loses a life!");
		}
	}
}