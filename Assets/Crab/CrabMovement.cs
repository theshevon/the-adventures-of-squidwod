using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabMovement : MonoBehaviour
{
	private GameObject player;
	public float crabSpeed;
	private Vector3 directionToPlayer;
	
	// Use this for initialization
	void Start ()
	{
		player = GameObject.FindWithTag("Player");
	}
	
	// Update is called once per frame
	void Update ()
	{
		
		directionToPlayer = player.transform.position - transform.position;
		transform.LookAt(player.transform, Vector3.up);
		//transform.Translate(directionToPlayer * Time.deltaTime * crabSpeed, Space.World);
		transform.position = Vector3.MoveTowards(transform.position, player.transform.position,
			crabSpeed * Time.deltaTime);
		transform.position = new Vector3(transform.position.x, 2, transform.position.z);
		transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
	}
}
