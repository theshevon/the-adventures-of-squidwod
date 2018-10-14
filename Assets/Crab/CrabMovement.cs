using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabMovement : MonoBehaviour
{
	public GameObject player;
	public float crabSpeed;
	
	// Use this for initialization
	void Start ()
	{
		player = GameObject.FindWithTag("Player");
	}
	
	// Update is called once per frame
	void Update ()
	{
		transform.LookAt(player.transform, Vector3.up);
		//transform.Translate(directionToPlayer * Time.deltaTime * crabSpeed, Space.World);
		transform.position = Vector3.MoveTowards(transform.position, player.transform.position,
			crabSpeed * Time.deltaTime);
		transform.position = new Vector3(transform.position.x, 2, transform.position.z);
		transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
	}
}
