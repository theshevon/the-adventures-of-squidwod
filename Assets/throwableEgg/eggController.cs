using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eggController : MonoBehaviour {

    public GameObject explosionEffect;
    private GameObject Seagull;
    public SeagullHealthManager seagullHealthManager;

    // Use this for initialization
    void Start () {
        Seagull = GameObject.FindWithTag("Seagull");
        seagullHealthManager = Seagull.GetComponent<SeagullHealthManager>();
	}
	
	// Update is called once per frame
	void Update () {

	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "ArenaWall")
        {
            Destroy(this.gameObject);
        } else if (collision.gameObject.tag == "Normal")
        {
            Destroy(this.gameObject);
            seagullHealthManager.Hit(false);
            GameObject explosion = Instantiate(explosionEffect);
            explosion.transform.position = transform.position;
            
        } else if (collision.gameObject.tag == "Critical")
        {
            Destroy(this.gameObject);
            seagullHealthManager.Hit(true);
            GameObject explosion = Instantiate(explosionEffect);
            explosion.transform.position = transform.position;
            
        } else
        {
            GameObject explosion = Instantiate(explosionEffect);
            explosion.transform.position = transform.position;
            Destroy(this.gameObject);
        }
    }
}
