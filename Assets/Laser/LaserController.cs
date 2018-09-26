using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour {

    public Vector3 direction;
    private float velocity = 1.0f;

    void Update () {
        this.transform.Translate(direction * Time.deltaTime * velocity);

        // dummy code to destroy laser objects
        if (this.transform.position.y < -12){
            Destroy(this.gameObject);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        //if (col.gameObject.tag == tagToDamage)
        //{
        //    // Damage object with relevant tag
        //    HealthManager healthManager = col.gameObject.GetComponent<HealthManager>();
        //    healthManager.ApplyDamage(damageAmount);

        //    // Destroy self
        //    Destroy(this.gameObject);
        //}
    }
}
