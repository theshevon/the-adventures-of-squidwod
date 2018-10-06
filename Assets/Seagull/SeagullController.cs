// Code to control the Giant Seagull in the game developed by Adam Turner,
// Amie Xie & Shevon Mendis for Project 02 of COMP30019.
//
// Written by Shevon Mendis, September 2018.

using UnityEngine;

public class SeagullController : MonoBehaviour
{

    public GameObject target;
    public GameObject laserPrefab;
    public Transform leftEye;
    public Transform rightEye;
    public AudioSource audioSrc;
    public AudioClip clip;

    const float radiusOfOrbit = 120.0f;
    const float orbitSpeed = 1.5f;
    const float soarHeight = 125.0f;
    const float ySpeed = 0.5f;
    const float maxHeightChange = 25.0f;
    const float bankAngle = -20.0f;

    [Range(1,5)]
    public int difficulty = 1;
    public Material laserMaterial;
    private Vector3 laserTarget;
    private Vector3 laserDirection;
    public float laserMoveLength = 80f;
    private float startTime;

    private LineRenderer laser;
    
    //const float frequencyStepTime = 10f;
    //const float frequencyStep = 0.25f;

    float totalTime;
    float delta;
    float fireDelay = 2;
    float countdown;

    void Start()
    {
        // set starting position & oritentation for seagull
        this.transform.position = new Vector3(0.0f, soarHeight, radiusOfOrbit);
        this.transform.Rotate(Vector3.up, 90, Space.Self);

        countdown = fireDelay;
        audioSrc.clip = clip;
        
        laser = gameObject.AddComponent<LineRenderer>();
        laser.material = laserMaterial;
        laser.endWidth = 0.4f;
        laser.startWidth = 0.4f;
    }

    void Update()
    {

        delta = Time.deltaTime;
        totalTime += delta;
        countdown -= delta;

        // make the seagull move in an orbit over the arena
        this.transform.position = new Vector3(radiusOfOrbit * Mathf.Sin(totalTime * orbitSpeed), 
                                              soarHeight + maxHeightChange * Mathf.Sin(totalTime * ySpeed), 
                                              radiusOfOrbit * Mathf.Cos(totalTime * orbitSpeed));

        // orient the seagull to face the right direction
        this.transform.Rotate(Vector3.up, delta / (Mathf.PI * 2) * 360 * orbitSpeed, Space.Self);

        // fire lasers from the seagull's eyes
        if (difficulty == 1) { Level1(); }
        if (difficulty == 2) { Level2(); }
        

    }

    void Level1()
    {
        if (countdown <= 0)
        {
            countdown = fireDelay;
            ShootLaser();
        }
    }

    void Level2()
    {
        if (countdown >= 0)
        {
            laserTarget = target.transform.position + target.transform.forward*30;
            laserTarget.y = 0;
            laserDirection = Random.insideUnitCircle;
            laserDirection.z = laserDirection.y;
            laserDirection.y = 0;
            startTime = Time.time;
        }
        if ((countdown >= -2) & (countdown <= 0))
        {
            ShootLongLaser();
        }
        if ((countdown <= -2))
        {
            laser.positionCount = 0;
            countdown = fireDelay;
            ShootLaser();
        }
    }
    

    void ShootLaser()
    {
        audioSrc.Play();
        // left eye laser
        Vector3 direction = target.transform.position - leftEye.position;
        GameObject laser = Instantiate(laserPrefab, leftEye.position, Quaternion.LookRotation(direction));
        //laser.transform.position = leftEye.position;
        //laser.transform.LookAt(Vector3.zero);
        laser.GetComponent<LaserController>().direction = direction;
    
        //Debug.DrawLine(Vector3.zero, -direction);
    
        // right eye laser
        direction = target.transform.position - rightEye.position;
        laser = Instantiate(laserPrefab, rightEye.position, Quaternion.LookRotation(direction));
        //laser.transform.position = rightEye.position;
        //laser.transform.LookAt(Vector3.zero);
        laser.GetComponent<LaserController>().direction = direction;
    
        //Debug.DrawLine(Vector3.zero, -direction);
    }

    void ShootLongLaser()
    {
        laserDirection.Normalize();
        float length = Vector3.Distance(laserTarget, laserDirection * laserMoveLength);
        float disCovered = (Time.time - startTime) * 25;
        Vector3 laserPosition = Vector3.Lerp(laserTarget, laserDirection * laserMoveLength, disCovered / length);
        laser.positionCount = 4;
        //laser.SetPosition(0, laserPos + (laserDirection * Time.deltaTime * 10));
        laser.SetPosition(0, laserPosition);
        laser.SetPosition(1, rightEye.position);
        laser.SetPosition(2, leftEye.position);
        laser.SetPosition(3, laserPosition);
    }
}
