using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flameControl : MonoBehaviour {
	private ParticleSystem ps;
	// Use this for initialization
	void Start ()
	{
		ps = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
		if(gameObject.name == "Smoke")
		{
			if(!ps.IsAlive())
			{
				Destroy(transform.parent.gameObject);
			}
		}
		// turn off flame collider so the player isn't damaged when flame is extinguished
		if(gameObject.name == "Flame01")
		{
			if(ps.time >= 3.0f)
			{
				GetComponent<BoxCollider>().enabled = false;
			}
		}
	}
}
