using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spinEgg : MonoBehaviour {
	
	public float spinSpeed = 150;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	private void Update()
	{
		transform.localRotation *= Quaternion.AngleAxis(Time.deltaTime * spinSpeed, new Vector3(0.0f, 1.0f, 0.0f));
	}
}
