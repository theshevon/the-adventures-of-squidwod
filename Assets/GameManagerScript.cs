using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour {

    //NEEDS TO INCLUDE CRAB AND EGG SPAWNER

    public GameObject player;
    public GameObject enemy;
    public GameObject Crab;
    public GameObject CrabBurrow;
    public Vector3 centre;
    public int diameter;

    bool inBossFight;
    float countdown = 5f;

    void Start()
    {
        //SpawnCrab(new Vector3(0,transform.position.y,0));
    }

    void Update () {
        countdown -= Time.deltaTime;
        if (countdown <= 0)
        {
            SpawnCrab();
            countdown = Random.Range(5, 10);
        }
    }

    // spawn a crab in a random location
    public void SpawnCrab()
    {
        Vector3 pos = centre + new Vector3(Random.Range(-diameter / 2, diameter / 2), transform.position.y, Random.Range(-diameter / 2, diameter / 2));

        Instantiate(CrabBurrow, pos, transform.rotation);
        StartCoroutine(WaitToSpawn(pos));

    }
    // spawn a crab in a specific location
    public void SpawnCrab(Vector3 pos)
    {
        Instantiate(CrabBurrow, pos, transform.rotation);
        StartCoroutine(WaitToSpawn(pos));
    }

    IEnumerator WaitToSpawn(Vector3 pos)
    {
        yield return new WaitForSeconds(5f);
        Instantiate(Crab, pos, Crab.transform.rotation);
    }
}
