using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCameraController : MonoBehaviour {

    public bool lockCursor;
    public Transform target;
    public float mouseSensitivity = 10;
    public float distanceFromTarget = 2;
    public float pitchMin = -40;
    public float pitchMax = 80;

    float pitch = 25;
    float yaw = 180;

    public float rotationSmoothTime = .12f;
    Vector3 rotationSmoothVelocity;
    Vector3 currentRotation = new Vector3(25, 180, 0);

    void Start () {
        //if (lockCursor)
        //{
        //    Cursor.lockState = CursorLockMode.Locked;
        //    Cursor.visible = false;
        //}
        transform.position = target.position - transform.forward * distanceFromTarget;
    }
    
    void LateUpdate () {
        // get yaw and pitch based on mouse movement
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;

        // ensure that the camera doesn't tumble over the player
        // or goes below ground
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);

        currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);
        transform.eulerAngles = currentRotation;

        transform.position = target.position - transform.forward * distanceFromTarget;
        
        RaycastHit hit;
        if (Physics.Linecast(target.position, transform.position, out hit))
        {
            Vector3 hitPoint = new Vector3(hit.point.x + hit.normal.x * 1f, hit.point.y, hit.point.z + hit.normal.z * 1f);
            transform.position = new Vector3(hitPoint.x, transform.position.y, hitPoint.z);
            //Debug.DrawLine(transform.position, hit.point, Color.green);
            //Debug.Log("hit terrain!");
        }

        
	}
}
