// Code to control the lasers fired by the Giant Seagull in the game developed 
// by Adam Turner, Amie Xie & Shevon Mendis for Project 02 of COMP30019.
//
// Written by Shevon Mendis, September 2018.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour {

    public Vector3 direction;
    const float velocity = 1.0f;

    void Update () {

        // move lasers in direction of target
        this.transform.Translate(direction * Time.deltaTime * velocity, Space.World);

        // dummy code to destroy laser objects
        if (this.transform.position.y < -12){
            Destroy(this.gameObject);
        }
    }

    //void OnTriggerEnter(Collider col)
    //{
    //    if (col.gameObject.tag == tagToDamage)
    //    {
    //        // Damage object with relevant tag
    //        HealthManager healthManager = col.gameObject.GetComponent<HealthManager>();
    //        healthManager.ApplyDamage(damageAmount);

    //        // Destroy self
    //        Destroy(this.gameObject);
    //    }
    //}
}
