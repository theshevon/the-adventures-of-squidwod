using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour {

    public int rotateSpeed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            this.transform.position += this.transform.forward;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            this.transform.position -= this.transform.forward;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            this.transform.Rotate(Vector3.down * Time.deltaTime * rotateSpeed);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            this.transform.Rotate(Vector3.up * Time.deltaTime * rotateSpeed);
        }
    }
}
