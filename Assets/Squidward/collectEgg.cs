using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collectEgg : MonoBehaviour {

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Squid has collided!");
        if (collision.collider.tag == "Egg")
        {
            
        }
    }
}
