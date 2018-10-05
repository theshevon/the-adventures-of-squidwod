using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggSpawner : MonoBehaviour {

    public GameObject EggPrefab;

    public Vector3 centre;
    public int diameter;

    // Use this for initialization
	void Start () {
        SpawnEgg();
    }

    private void Update()
    {

    }
    // spawn an egg in a random location
    public void SpawnEgg()
    {
        Vector3 pos = centre + new Vector3(Random.Range(-diameter/2, diameter/2), transform.position.y,  Random.Range(-diameter/ 2, diameter/ 2));
        Instantiate(EggPrefab, pos, Quaternion.identity);
    }
    // spawn an egg in a specific location
    public void SpawnEgg(Vector3 pos)
    {
        Instantiate(EggPrefab, pos, Quaternion.identity);
    }
    /*
    private void OnCollisionEnter(Collision col)
    {
        Debug.Log("Egg has collided!");
        if (col.gameObject.tag == "Player")
        {
            SpawnEgg();
            Destroy(gameObject);
            
        }
    }*/
}
