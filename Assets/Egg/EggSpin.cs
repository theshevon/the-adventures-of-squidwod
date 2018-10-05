using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggSpin : MonoBehaviour {

    public float spinSpeed = 150;

	// Update is called once per frame
	void Update () {
        this.transform.localRotation *= Quaternion.AngleAxis(Time.deltaTime * spinSpeed, new Vector3(0.0f, 1.0f, 0.0f));
    }
}
