using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameThrowerController : MonoBehaviour {

    public GameObject source;
    public GameObject sourceBody;

	// Update is called once per frame
	void Update () {
        transform.position = source.transform.position;
        transform.rotation = sourceBody.transform.rotation;
	}
}
