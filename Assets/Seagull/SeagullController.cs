// Code to control the Giant Seagull in the game being developed for 
// Project 02 of COMP30019.
//
// Written by Shevon Mendis, September 2018.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeagullController : MonoBehaviour {

    private const float orbitSpeed = 1.5f;
    private const float heightChangeSpeed = 0.5f;
    private const float radiusOfOrbit = 120.0f;
    private const float height = 125.0f;
    private const float maxHeightChange = 25.0f;
    private const float bankAngle = -20.0f;

    private float totalTime = 0;
    private float delta;

    void Start()
    {
        // set starting position & oritentation for seagull
        this.transform.position = new Vector3(0.0f, height, radiusOfOrbit);
        this.transform.Rotate(Vector3.up, 90, Space.Self);

        // bank towards the arena
        this.transform.Rotate(Vector3.right, bankAngle, Space.Self); //NEEDS TO BE FIXED

    }

    void Update () {

        delta = Time.deltaTime;
        totalTime += delta;

        // make the seagull move in an orbit over the arena
        this.transform.position = new Vector3(radiusOfOrbit * Mathf.Sin(totalTime * orbitSpeed), height, radiusOfOrbit * Mathf.Cos(totalTime * orbitSpeed));
        this.transform.position += new Vector3(0.0f, maxHeightChange * Mathf.Sin(totalTime * heightChangeSpeed), 0.0f);

        // orient the seagull to face the right direction
        this.transform.Rotate(Vector3.up, (delta/ (Mathf.PI *2)) * 360 * orbitSpeed, Space.Self);

    }
}
