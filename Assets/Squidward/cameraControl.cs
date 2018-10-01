using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraControl : MonoBehaviour
{
	public bool lockCursor;
	public float mouseSensitivity = 10;
	public Transform cameraTarget;
	public float distanceFromTarget = 20;
	public Vector2 pitchMinMax = new Vector2(-10, 70);

	public float rotationSmoothTime = 0.1f;
	private Vector3 rotationSmoothVelocity;
	private Vector3 currentRotation;
	
	private float yaw;
	private float pitch;
	
	// Use this for initialization
	void Start () {
		if (lockCursor)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}
	
	// Update is called once per frame
	void LateUpdate ()
	{
		yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
		pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
		pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
		currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity,
			rotationSmoothTime);
		transform.eulerAngles = currentRotation;
		transform.position = cameraTarget.position - transform.forward * distanceFromTarget;
	}

}
