using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeagullBossController : MonoBehaviour
{
	public Boolean startBattle;
	public GameObject Player;
	public Camera cam;
	private Boolean movingDown;
	private Boolean movedDown;
	private LineRenderer laser;
	private Animator animator;
	private AudioSource audioSrc;
	private float currentStep;

	private Vector3 battlePosition = new Vector3(0,2,0);
	// Use this for initialization
	void Start ()
	{
		laser = GetComponent<LineRenderer>();
		audioSrc = GetComponent<AudioSource>();
		animator = GetComponent<Animator>();
		laser.positionCount = 0;
		audioSrc.Stop();
	}

	void OnEnable()
	{
		startBattle = true;
	}

	// Update is called once per frame
	void Update () {
		if (startBattle)
		{
			MoveSeagull();
			if (transform.position == battlePosition)
			{
				startBattle = false;
			}
		}
		else
		{
			
		}
		
		
	}

	void MoveSeagull()
	{
		cam.GetComponent<BossFightThirdPersonCameraController>().enabled = false;
		cam.GetComponent<Transform>().LookAt(transform);
		
		Vector3 centerPos = battlePosition;
		centerPos.y = 250;
		if (!movingDown)
		{
			transform.position = Vector3.MoveTowards(transform.position, centerPos, 50 * Time.deltaTime);
			transform.LookAt(centerPos);
		}
		if(transform.position == centerPos) { movingDown = true;}

		if (movingDown)
		{
			transform.position = Vector3.MoveTowards(transform.position, battlePosition, 90 * Time.deltaTime);
			transform.LookAt(Player.transform);
			transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
			animator.SetTrigger("diveFromAir");
		}
		if (transform.position.y < 15) {animator.SetTrigger("diveLand");}
		if (transform.position == battlePosition)
		{
			transform.LookAt(Player.transform);
			transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
			cam.GetComponent<BossFightThirdPersonCameraController>().enabled = true;
		}
	}
}
