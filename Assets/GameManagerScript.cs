using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManagerScript : MonoBehaviour {

    //NEEDS TO INCLUDE CRAB AND EGG SPAWNER

    public GameObject player;
    public GameObject enemy;
    public GameObject Seagull;
    public GameObject Crab;
    public GameObject CrabBurrow;
    public Camera camera;
    public Vector3 centre;
    public int diameter;
    public AudioSource audioSrc;
    public TextMeshProUGUI scoreValue;

    public int CurrentScore;
    public int TotalScore;

    Boolean startBattle;

    bool inBossFight;
    float countdown = 5f;

    void Start()
    {
        //SpawnCrab(new Vector3(0,transform.position.y,0));
    }

    void Update ()
    {
        scoreValue.text = CurrentScore.ToString();
        if (CurrentScore == 6) { StartBattle(); }
//        countdown -= Time.deltaTime;
//        if (countdown <= 0)
//        {
//            SpawnCrab();
//            countdown = Random.Range(5, 10);
//        }
    }

    void StartBattle()
    {
        if (!startBattle)
        {
            Seagull.GetComponent<SeagullController>().enabled = false;
            Seagull.GetComponent<SeagullBossController>().enabled = true;
            camera.GetComponent<ThirdPersonCameraController>().enabled = false;
            //camera.GetComponent<BossFightThirdPersonCameraController>().enabled = true;
            //player.GetComponent<bossControls>().enabled = true;
            player.GetComponent<movement>().enabled = false;
            startBattle = true;
        }
        if (!Seagull.GetComponent<SeagullBossController>().startBattle)
        {
            player.GetComponent<bossControls>().enabled = true;
            player.GetComponent<movement>().enabled = true;
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
