using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NOTE: SQUIDWARDS CONTROLLER MUST TURN ALONG WITH CAMERA

public class BossFightThirdPersonCameraController : MonoBehaviour {

    public bool lockCursor;
    public Transform target;
    public Transform rotationTarget;
    public float mouseSensitivity = 10;
    public float distanceFromTarget = 2;
    public float pitchMin = -40;
    public float pitchMax = 80;
    public GameObject Seagull;
    float pitch;
    float yaw;

    public float rotationSmoothTime = .12f;
    private Transform prevPos;
    Vector3 rotationSmoothVelocity;
    Vector3 currentRotation;

    void OnEnable () {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    void LateUpdate () {

        // get yaw and pitch based on mouse movement
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;

        // ensure that the camera doesn't tumble over the player
        // or goes below ground
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);

        transform.rotation = Quaternion.LookRotation(rotationTarget.position - target.position);
        // lock the cameras z axis to 0
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
        transform.position = target.position - rotationTarget.position - transform.forward * distanceFromTarget;
        
        // cast line from seagull position to the camera and check for collisons
        RaycastHit hit;
        if (Physics.Linecast(target.position, transform.position, out hit))
        {
            // if line hits the terrain, need to move the camera so it doesn't clip through
            if (hit.transform.gameObject.CompareTag("Terrain"))
            {
                Vector3 hitPoint = new Vector3(hit.point.x + hit.normal.x * 0.5f, hit.point.y + hit.normal.y * 0.5f,
                    hit.point.z + hit.normal.z * 0.5f);
                transform.position = new Vector3(hitPoint.x, hitPoint.y, hitPoint.z);
            }
        }
	}
    
}
