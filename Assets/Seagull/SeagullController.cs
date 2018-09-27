// Code to control the Giant Seagull in the game developed by Adam Turner,
// Amie Xie & Shevon Mendis for Project 02 of COMP30019.
//
// Written by Shevon Mendis, September 2018.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeagullController : MonoBehaviour
{

    public GameObject laserPrefab;
    public Transform leftEye;
    public Transform rightEye;
    public AudioSource audioSrc;
    public AudioClip clip;

    const float radiusOfOrbit = 120.0f;
    const float orbitSpeed = 1.5f;
    const float soarHeight = 125.0f;
    const float ySpeed = 0.5f;
    const float maxHeightChange = 25.0f;
    const float bankAngle = -20.0f;
    //const float frequencyStepTime = 10f;
    //const float frequencyStep = 0.25f;

    GameObject target;
    float totalTime;
    float delta;
    float timeSinceFire;
    float fireFrequency = 2;

    void Start()
    {
        // set starting position & oritentation for seagull
        this.transform.position = new Vector3(0.0f, soarHeight, radiusOfOrbit);
        this.transform.Rotate(Vector3.up, 90, Space.Self);

        // identify squidward as targer
        target = GameObject.Find("squidward");

        audioSrc.clip = clip;
    }

    void Update()
    {

        delta = Time.deltaTime;
        totalTime += delta;
        timeSinceFire += delta;

        // make the seagull move in an orbit over the arena
        this.transform.position = new Vector3(radiusOfOrbit * Mathf.Sin(totalTime * orbitSpeed), 
                                              soarHeight + maxHeightChange * Mathf.Sin(totalTime * ySpeed), 
                                              radiusOfOrbit * Mathf.Cos(totalTime * orbitSpeed));

        // orient the seagull to face the right direction
        this.transform.Rotate(Vector3.up, delta / (Mathf.PI * 2) * 360 * orbitSpeed, Space.Self);

        // fire lasers from the seagull's eyes
        if (timeSinceFire >= fireFrequency)
        {
            audioSrc.Play();

            timeSinceFire = 0;

            Debug.Log("Squidward's position: " + target.transform.position);

            // left eye laser
            Vector3 direction = target.transform.position - leftEye.position;
            GameObject laser = Instantiate(laserPrefab);
            laser.transform.position = leftEye.position;
            //laser.transform.LookAt(Vector3.zero);
            laser.GetComponent<LaserController>().direction = direction;

            //Debug.Log("Laser position: " + leftEye.position);
            //Debug.DrawLine(Vector3.zero, -direction);

            // right eye laser
            direction = target.transform.position - rightEye.position;
            laser = Instantiate(laserPrefab);
            laser.transform.position = rightEye.position;
            //laser.transform.LookAt(Vector3.zero);
            laser.GetComponent<LaserController>().direction = direction;

            //Debug.Log("Laser position: " + rightEye.position);
            //Debug.DrawLine(Vector3.zero, -direction);
        }
    }
}
