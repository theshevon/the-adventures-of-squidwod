using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flameCollision : MonoBehaviour
{

	public GameObject flame;
	public Vector3 direction;
	const float velocity = 0.5f;
	
	void Update()
	{
		if (gameObject.CompareTag("Fireball"))
		{
			transform.Translate(direction * Time.deltaTime * velocity, Space.World);
		}
	}
	
	void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.CompareTag("Crab"))
		{
			Destroy(col.gameObject);
		}

		if (col.gameObject.CompareTag("Terrain") && gameObject.CompareTag("Fireball"))
		{
			Vector3 pos = transform.position;
			pos.y = 0;
			Instantiate(flame, pos, flame.transform.rotation);
			Destroy(gameObject);
		}
	}
}
