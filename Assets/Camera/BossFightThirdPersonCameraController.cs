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
    private SeagullBossController seagullController;
    float pitch;
    float yaw;

    public float rotationSmoothTime = .12f;
    private Transform prevPos;
    Vector3 rotationSmoothVelocity;
    Vector3 currentRotation;

    void Start () {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        seagullController = Seagull.GetComponent<SeagullBossController>();
    }
    
    void LateUpdate () {

        // get yaw and pitch based on mouse movement
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;

        // ensure that the camera doesn't tumble over the player
        // or goes below ground
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);

        //currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);
        //transform.eulerAngles = currentRotation;

        //transform.LookAt(rotationTarget.position, Vector3.up);

        transform.rotation = Quaternion.LookRotation(rotationTarget.position - target.position);
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
        transform.position = target.position - rotationTarget.position- transform.forward * distanceFromTarget;


        
        //transform.rotation = Quaternion.LookRotation(rotationTarget.position - target.position);
        
        
        //transform.Rotate(Vector3.up * -5);
        
      
        
        
        RaycastHit hit;
        if (Physics.Linecast(target.position, transform.position, out hit))
        {
            if (hit.transform.gameObject.CompareTag("Terrain"))
            {
                Vector3 hitPoint = new Vector3(hit.point.x + hit.normal.x * 0.5f, hit.point.y + hit.normal.y * 0.5f,
                    hit.point.z + hit.normal.z * 0.5f);
                transform.position = new Vector3(hitPoint.x, hitPoint.y, hitPoint.z);
            }

            //Debug.DrawLine(transform.position, hit.point, Color.green);
            //Debug.Log("hit terrain!");
        }
	}
    
}
