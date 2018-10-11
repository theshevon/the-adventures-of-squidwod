using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameThrowerController : MonoBehaviour {

    public Transform source;
    public Transform sourceBody;

	// Update is called once per frame
	void Update () {
        transform.position = source.position;
        transform.rotation = sourceBody.rotation;
	}
}
