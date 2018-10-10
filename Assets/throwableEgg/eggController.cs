using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eggController : MonoBehaviour {

    public GameObject explosionEffect;

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "ArenaWall")
        {
            GameObject explosion = Instantiate(explosionEffect);
            explosion.transform.position = transform.position;
        }
        Destroy(this.gameObject);
    }
}
