using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collectEgg : MonoBehaviour {

    private void OnCollisionEnter(Collision collision)
    { 
        if (collision.collider.tag == "Egg")
        {
            Debug.Log("Squid hit egg!");
        }
    }
}
