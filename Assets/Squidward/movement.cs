using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour
{

	public float speed = 3;
	
    public int rotateSpeed;
    private Transform cameraT;

	public float turnSmoothTime = 0.2f;
	private float turnSmoothVelocity;
	
	public float speedSmoothTime = 0.1f;
	private float speedSmoothVelocity;
	private float currentSpeed;
	
	// Use this for initialization
	void Start ()
	{
	    cameraT = Camera.main.transform;
	}
	
	// Update is called once per frame
	void Update ()
	{
		Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		Vector2 inputDir = input.normalized;

		if (inputDir != Vector2.zero)
		{
			float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
			transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation,
				                        ref turnSmoothVelocity, turnSmoothTime);
		}

		float targetSpeed = speed * inputDir.magnitude;
		currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);
		
		transform.Translate(transform.forward * currentSpeed * Time.deltaTime, Space.World);
		
	}
}
