using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameThrowerCollision : MonoBehaviour {

	private void OnParticleCollision(GameObject other)
	{
		if (other.transform.CompareTag("Player"))
		{
			other.gameObject.GetComponent<interaction>().OnPlayerHit();
		}
	}
}
