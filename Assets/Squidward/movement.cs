using System;
using System.Collections;
using System.Collections.Generic;
using Boo.Lang;
using UnityEngine;

public class movement : MonoBehaviour
{
	public float speed = 3;
	public float gravity = 20.0f;
	public float jumpSpeed = 20.0f;
	private bool grounded;

	private Vector3 moveDirection = Vector3.zero;
	private CharacterController cc;
	private int jumps;
	
    private Transform cameraT;

	public float turnSmoothTime = 0.2f;
	private float turnSmoothVelocity;
	
	public float speedSmoothTime = 0.1f;
	private float speedSmoothVelocity;
	private float currentSpeed;
	private float currentJump;
	private bool isJump = false;

	private Vector3 hitNormal;
	private bool isOnSlope = false;
	public float slopeLimit = 45;
	public float slideFriction = 0.3f;

	private Animator animator;
	
	
	// Use this for initialization
	void Start ()
	{
	    cameraT = Camera.main.transform;
		animator = GetComponent<Animator>();
		cc = GetComponent<CharacterController>();
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
		
		// Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
		// when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
		// as an acceleration (ms^-2)
		/*transform.position = new Vector3(transform.position.x, transform.position.y - gravity * Time.deltaTime,
			transform.position.z);
		if (!cc.isGrounded)
		{
			if (Input.GetButtonDown("Jump"))
			{
				Debug.Log("jump!");
			}
		}

		cc.Move(transform.forward * currentSpeed * Time.deltaTime);*/
		grounded = Physics.Raycast(transform.position, Vector3.down, 0.2f, LayerMask.NameToLayer("Terrain"));
		if (grounded)
		{
			moveDirection = new Vector3(0, 0, 0);
			if (Input.GetButtonDown("Jump"))
			{
				moveDirection.y = jumpSpeed;
				Debug.Log("test jump");
			}
		}
		else
		{
			//moveDirection.y -= gravity * Time.deltaTime;
		}
		moveDirection.y -= gravity * Time.deltaTime;
		if (isOnSlope)
		{
			Debug.Log("on slope!");
			moveDirection.x += (1f - hitNormal.y) * hitNormal.x * (1f - slideFriction);
			moveDirection.z += (1f - hitNormal.y) * hitNormal.z * (1f - slideFriction);
		}
		cc.Move(moveDirection * Time.deltaTime);
		cc.Move(transform.forward * currentSpeed * Time.deltaTime);
		

		float animationSpeedPercent = 1 * inputDir.magnitude;
		animator.SetFloat("motion", animationSpeedPercent, speedSmoothTime, Time.deltaTime);

		isOnSlope = (Vector3.Angle (Vector3.up, hitNormal) >= slopeLimit);
		
	}
	
	void OnControllerColliderHit (ControllerColliderHit hit) {
		hitNormal = hit.normal;
	}
}
