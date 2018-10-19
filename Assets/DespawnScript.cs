using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DespawnScript : MonoBehaviour {

    const float MAX_TIME = 20;
    float time;
	
	void Update () {
        time += Time.deltaTime;
        if (time >= MAX_TIME) Destroy(gameObject);
	}
}
