using System;
using System.Collections;
using System.Collections.Generic;
using Boo.Lang;
using UnityEngine;

public class movement : MonoBehaviour
{
	public float speed = 3;
	public float gravity = 20.0f;
	public float jumpSpeed;
	private bool grounded;

	private Vector3 moveDirection = Vector3.zero;
	private CharacterController cc;
	private int jumps;
	
    private Camera cam;
	private Transform cameraT;

	public float turnSmoothTime = 0.2f;
	private float turnSmoothVelocity;
	
	public float speedSmoothTime = 0.1f;
	private float speedSmoothVelocity;
	private float currentSpeed;
	private float currentJump;

	private Vector3 hitNormal;
	private bool isOnSlope;
	public float slopeLimit = 45;
	public float slideFriction = 0.3f;
	public Vector2 inputDir;

    public float fallMultiplier = 2f;
    public float lowJumpMultiplier = 1.5f;

    bool isJump;

	private Animator animator;
	private AudioSource audioSrc;
	public AudioClip jumpSound;

	public GameObject StatCounter;
	
	
	// Use this for initialization
	void Start ()
	{
		cam = Camera.main;
		cameraT = cam.transform;
		cam = GetComponent<Camera>();
		animator = GetComponent<Animator>();
		cc = GetComponent<CharacterController>();
		audioSrc = GetComponent<AudioSource>();
	}
	// Update is called once per frame
	void Update ()
	{
		
		
		Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		inputDir = input.normalized;

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
	        animator.speed = 1;
			moveDirection = new Vector3(0, 0, 0);
            if (Input.GetButtonDown("Jump"))
            {
	            StatCounter.GetComponent<StatCounterScript>().numberJumps++;
	            audioSrc.PlayOneShot(jumpSound, 0.8f);
                moveDirection.y = jumpSpeed;
                //moveDirection.y += Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;

                if (inputDir == Vector2.zero)
                {
                    animator.SetTrigger("ToJump");
                }
                else
                {
	                animator.speed = 0.1f;
                }
            }

	        if (Input.GetKeyDown(KeyCode.F))
	        {
		        animator.SetTrigger("DAB");
	        }

        } else if (moveDirection.y < 0)
        {
            moveDirection.y -= gravity * (fallMultiplier - 1) * Time.deltaTime;
        } else if (moveDirection.y > 0 && !Input.GetButton("Jump"))
        {
        	moveDirection.y -= gravity * (lowJumpMultiplier-1) * Time.deltaTime;
        }

		moveDirection.y -= gravity * Time.deltaTime;
		if (isOnSlope)
		{
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
		if (hit.gameObject.CompareTag("Terrain"))
		{
			hitNormal = hit.normal;
		}
		else
		{
			hitNormal = Vector3.zero;
		}
	}
}
