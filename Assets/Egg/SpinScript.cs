using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinScript : MonoBehaviour {
	
	public float spinSpeed = 150;
    public int yOffset;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update()
	{
        transform.localRotation *= Quaternion.AngleAxis(Time.deltaTime * spinSpeed, Vector3.up);
		transform.position = new Vector3(transform.position.x, yOffset + Mathf.Sin(Time.time * 1.5f), transform.position.z);
	}
}
