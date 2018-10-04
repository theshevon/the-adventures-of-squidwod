// Code to control the Giant Seagull in the game developed by
// Adam Turner, Amie Xie & Shevon Mendis for Project 02 of COMP30019.
//
// Written by Adam Turner, October 2018.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossControls : MonoBehaviour
{

	public GameObject target;
	public GameObject egg;
	public Transform startPoint;
	public float distance;
	public Camera cam;

	private LineRenderer lineRenderer;
	public Material lineRendererMaterial;
	private float count;
	private Vector3 aimTarget;
	public float aimTime;
	private Boolean countUp = true;
	
	
	// Use this for initialization
	void Start ()
	{
		lineRenderer = gameObject.AddComponent<LineRenderer>();
		lineRenderer.material = lineRendererMaterial;
		lineRenderer.positionCount = 2;
		lineRenderer.startColor = Color.cyan;
		lineRenderer.startWidth = 0.3f;
		lineRenderer.endWidth = 0.3f;
		lineRenderer.endColor = Color.cyan;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButton(0))
		{

			if (countUp) count += Time.deltaTime;
				else count -= Time.deltaTime;
			if (count >= aimTime) countUp = false;
			if (count <= 0) countUp = true;
			Debug.Log(count);
			aimTarget = new Vector3(target.transform.position.x, target.transform.position.y,
				target.transform.position.z);
			lineRenderer.SetPosition(0, aimTarget);
			lineRenderer.SetPosition(1, startPoint.position);
			
			
		}
		if (Input.GetMouseButtonUp(0))
		{
			Debug.Log("Test!");
			//egg = Instantiate(egg, startPoint.position + (10 * cam.transform.forward), Quaternion.identity);
			//egg.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, 0, 2000));
			//Rigidbody rb = egg.GetComponent<Rigidbody>();
			//rb.transform.LookAt(position);
			//rb.velocity = rb.transform.forward * 5;
		}
	}
}
