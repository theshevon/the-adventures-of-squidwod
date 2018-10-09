using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManagerScript : MonoBehaviour {


    public GameObject player;
    public GameObject enemy;
    public GameObject Seagull;
    public GameObject Crab;
    public GameObject CrabBurrow;
    public Camera camera;
    public GameObject EggPrefab;

    public Vector3 centre;
    public int diameter;
    public AudioSource audioSrc;
    public TextMeshProUGUI scoreValue;

    public int CurrentScore;
    public int TotalScore;

    const float eggHeight = 7.37f;
    const float crabHeight = 2;
    const int fightThreshold = 6;
    bool inBossFight;
    float countdown = 5f;

    void Start()
    {
        if (!inBossFight){
            SpawnEgg(new Vector3(0, eggHeight, 0));
        }
    }

    void Update ()
    {

        scoreValue.text = CurrentScore.ToString();

        // start boss battle if score threshold satisfied
        // hotkey added for testing purposes
        if (((CurrentScore > 0 && CurrentScore % fightThreshold == 0) || Input.GetKeyDown(KeyCode.B)) && !inBossFight) {
            inBossFight = true;
            StartBattle();
        }

        // randomly spawn crabs
        countdown -= Time.deltaTime;
        if (countdown <= 0)
        {
            SpawnCrab();
            countdown = Random.Range(5, 10);
        }
    }

    void StartBattle()
    {

        SeagullFlightController seagullFlight = Seagull.GetComponent<SeagullFlightController>();
        seagullFlight.enabled = false;
        SeagullBossController seagullBoss = Seagull.GetComponent<SeagullBossController>();
        seagullBoss.enabled = true;
        seagullBoss.totalTime = seagullFlight.totalTime;

        camera.GetComponent<ThirdPersonCameraController>().enabled = false;
        camera.GetComponent<BossFightThirdPersonCameraController>().enabled = true;

        player.transform.position = new Vector3(0, 2, 75);
        player.transform.LookAt(new Vector3(0, player.transform.position.y, 0));
        player.GetComponent<bossControls>().enabled = true;
        player.GetComponent<movement>().enabled = false;
    }

    // spawn a crab in a random location
    public void SpawnCrab()
    {
        Vector3 pos = centre + new Vector3(Random.Range(-diameter / 2, diameter / 2), crabHeight, Random.Range(-diameter / 2, diameter / 2));
        Quaternion rotation = Quaternion.identity;
        rotation.eulerAngles = new Vector3(-90, 0, 0);
        Instantiate(CrabBurrow, pos + new Vector3(0,crabHeight,0), rotation);
        StartCoroutine(WaitToSpawn(pos));
    }

    // spawn a crab in a specific location
    public void SpawnCrab(Vector3 pos)
    {
        Instantiate(CrabBurrow, pos + new Vector3(0, crabHeight, 0), Quaternion.LookRotation(transform.position, Vector3.up));
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
}
