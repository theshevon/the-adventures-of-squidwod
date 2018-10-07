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
	public Transform aimCamPos;
	public Transform prevCamPos;

	private LineRenderer lineRenderer;
	public Material lineRendererMaterial;
	private float count;
	private Vector3 aimTarget;
	public float aimTime;
	private Boolean countUp = true;
	public float smooth;
	
	public float panSteps = 10f; // 10 frames?
	private float currentStep;
	private RaycastHit hit;
	private Ray ray;

	public float throwSpeed;
	
	
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
		if (Input.GetMouseButtonDown(0))
		{
			currentStep = 0;
			
		}
		if (Input.GetMouseButton(0))
		{
			
			transform.LookAt(new Vector3(0, 1, 0));
			if (currentStep >= panSteps)
			{
				
			}
			else
			{
				currentStep += Time.deltaTime;
				cam.GetComponent<BossFightThirdPersonCameraController>().enabled = false;
				GetComponent<movement>().enabled = false;
				//cam.transform.position = Vector3.MoveTowards(cam.transform.position, 
				//	aimCam.transform.position,  Time.deltaTime * smooth);
				
				cam.transform.position = Vector3.Lerp(cam.transform.position,
					aimCamPos.transform.position,currentStep);
				cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, aimCamPos.rotation, currentStep);
				//cam.transform.LookAt(target);
			}
			
			
			if (countUp) count += Time.deltaTime;
				else count -= Time.deltaTime;
			if (count >= aimTime) countUp = false;
			if (count <= 0) countUp = true;
			Mathf.Clamp(count, 0, 1);
			
			ray = cam.ScreenPointToRay(Input.mousePosition);
        
			if (Physics.Raycast(ray, out hit)) {
				lineRenderer.positionCount = 2;
				lineRenderer.SetPosition(0, startPoint.position + (4 * cam.transform.forward));
				lineRenderer.SetPosition(1, hit.point);
				// Do something with the object that was hit by the raycast.
			}
			
			/*aimTarget = new Vector3(target.transform.position.x, -4 + count*50,
				target.transform.position.z);
			lineRenderer.SetPosition(0, aimTarget);
			lineRenderer.SetPosition(1, startPoint.position);*/
			
			
		}
		if (Input.GetMouseButtonUp(0))
		{
			//GetComponent<movement>().enabled = true;
			//cam.GetComponent<ThirdPersonCameraController>().enabled = true;
			currentStep = 0;
			lineRenderer.positionCount = 0;
			GetComponent<movement>().enabled = true;
			
			egg = Instantiate(egg, startPoint.position + (4 * cam.transform.forward), Quaternion.identity);
			//egg.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, 0, 2000));
			Rigidbody rb = egg.GetComponent<Rigidbody>();
			//rb.transform.LookAt(position);
			rb.velocity = ray.direction * throwSpeed;
		}

		if (!Input.GetMouseButton(0))
		{
			if (currentStep >= 1)
			{
				//cam.GetComponent<ThirdPersonCameraController>().enabled = true;
				//GetComponent<movement>().enabled = true;
				//Debug.Log("moved back!");
				//moveBack = false;
			}
			else
			{
				currentStep += Time.deltaTime;
				//cam.transform.position = Vector3.MoveTowards(cam.transform.position, 
				//	aimCam.transform.position,  Time.deltaTime * smooth);
				cam.transform.position = Vector3.Lerp(cam.transform.position,
					prevCamPos.position,currentStep);
				cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, prevCamPos.rotation, currentStep);
				//cam.transform.LookAt(target);
			}

			if (cam.transform.position == prevCamPos.position)
			{
				cam.GetComponent<BossFightThirdPersonCameraController>().enabled = true;
			}
		}
	}
}
