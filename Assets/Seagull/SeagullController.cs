// Code to control the Giant Seagull in the game being developed for 
// Project 02 of COMP30019.
//
// Written by Shevon Mendis, September 2018.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeagullController : MonoBehaviour
{

    public GameObject laserPrefab;

    private GameObject target;
    private GameObject laser;
    private const float orbitSpeed = 1.5f;
    private const float heightChangeSpeed = 0.5f;
    private const float radiusOfOrbit = 120.0f;
    private const float height = 125.0f;
    private const float maxHeightChange = 25.0f;
    private const float bankAngle = -20.0f;
    private float fireFrequency = 2;
    //private const float frequencyStep = 0.25f;
    private Vector3[] laserPositions = new Vector3[] { new Vector3(3.0f, 34.5f, 20.0f), new Vector3(-3.0f, 34.5f, 20.0f) };

    private float totalTime = 0;
    private float delta;
    private float timeSinceFire = 0;

    void Start()
    {
        // set starting position & oritentation for seagull
        this.transform.position = new Vector3(0.0f, height, radiusOfOrbit);
        this.transform.Rotate(Vector3.up, 90, Space.Self);

        // bank towards the arena
        this.transform.Rotate(Vector3.right, bankAngle, Space.Self); //NEEDS TO BE FIXED

        // identify squidward as targer
        target = GameObject.Find("squidward");
    }

    void Update()
    {

        delta = Time.deltaTime;
        totalTime += delta;
        timeSinceFire += delta;

        // make the seagull move in an orbit over the arena
        this.transform.position = new Vector3(radiusOfOrbit * Mathf.Sin(totalTime * orbitSpeed), height, radiusOfOrbit * Mathf.Cos(totalTime * orbitSpeed));
        this.transform.position += new Vector3(0.0f, maxHeightChange * Mathf.Sin(totalTime * heightChangeSpeed), 0.0f);

        // orient the seagull to face the right direction
        this.transform.Rotate(Vector3.up, (delta / (Mathf.PI * 2)) * 360 * orbitSpeed, Space.Self);

        if (timeSinceFire >= fireFrequency)
        {
            timeSinceFire = 0;

            Debug.Log("Squidward's position: " + target.transform.position);

            for (int i = 0; i < laserPositions.Length; i++)
            {
                Vector3 laserPos = this.transform.position + laserPositions[i];
                Vector3 direction = target.transform.position - laserPos;
                laser = Instantiate<GameObject>(laserPrefab, laserPos, Quaternion.LookRotation(new Vector3(0,0,0)));
                laser.GetComponent<LaserController>().direction = direction;
            }
        }
    }
}
