using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour {


    public GameObject player;
    public GameObject enemy;
    public GameObject Crab;
    public GameObject CrabBurrow;
    public GameObject EggPrefab;
    public Vector3 centre;
    public int diameter;

    const float eggHeight = 7.37f;
    const float crabHeight = 2;
    bool inBossFight;
    float countdown = 5f;

    void Start()
    {
        if (!inBossFight){
            SpawnEgg(new Vector3(0, eggHeight, 0));
        }
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
        Vector3 pos = centre + new Vector3(Random.Range(-diameter / 2, diameter / 2), crabHeight, Random.Range(-diameter / 2, diameter / 2));

        Instantiate(CrabBurrow, pos + new Vector3(0,crabHeight,0), transform.rotation);
        StartCoroutine(WaitToSpawn(pos));

    }
    // spawn a crab in a specific location
    public void SpawnCrab(Vector3 pos)
    {
        Instantiate(CrabBurrow, pos + new Vector3(0, crabHeight, 0), transform.rotation);
        StartCoroutine(WaitToSpawn(pos));
    }

    IEnumerator WaitToSpawn(Vector3 pos)
    {
        yield return new WaitForSeconds(5f);
        Instantiate(Crab, pos, Crab.transform.rotation);
    }

    // spawn an egg in a random location
    public void SpawnEgg()
    {
        Vector3 pos = centre + new Vector3(Random.Range(-diameter / 2, diameter / 2), eggHeight, Random.Range(-diameter / 2, diameter / 2));
        Instantiate(EggPrefab, pos, Quaternion.identity);
    }
    // spawn an egg in a specific location
    public void SpawnEgg(Vector3 pos)
    {
        Instantiate(EggPrefab, pos, Quaternion.identity);
    }

    /*public void CollectEgg()
    {
        spinSpeed = Mathf.Pow(spinSpeed, 2.0f);
        this.transform.position += transform.up * Time.deltaTime;
    }*/
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
