// Code to control the Giant Seagull in the game developed by Adam Turner,
// Amie Xie & Shevon Mendis for Project 02 of COMP30019.
//
// Written by Shevon Mendis & Adam Turner September 2018.

using System.Collections;
using UnityEngine;

public class SeagullFlightController : MonoBehaviour
{

    public GameObject target;
    public GameObject laserPrefab;
    public Transform leftEye;
    public Transform rightEye;
    public Transform mouth;
    public AudioSource audioSrc;
    public AudioClip laserSound;
    public AudioClip laserBeamSound;
    public AudioClip fireballSound;
    public GameObject flame;
    public GameObject fireball;
    public GameObject Player;
    public Material laserMaterial;

    [Range(1, 5)]
    public int currentLevel = 1;

    // orbit related 
    const float radiusOfOrbit = 120.0f;
    const float orbitSpeed = 1.5f;
    const float soarHeight = 125.0f;
    const float ySpeed = 0.5f;
    const float maxHeightChange = 25.0f;

    // shooting related
    float fireDelay = 2;
    float fireCountDown;

    // update related
    public float totalTime;
    float delta;
    
    // transition related
    Animator animator;
    public bool isFlying = true;
    private Vector3 flyPosition = new Vector3(0, 150, 120);
    readonly Vector3 battlePosition = new Vector3(0,2,0);
    
    // miscellaneous
    Vector3 laserTarget;
    Vector3 laserDirection;
    float laserMoveLength = 80f;
    float startTime;
    float counterTime;
    LineRenderer laser;
    movement movement;

    private int attackNumber;

    void Start()
    {
        animator = GetComponent<Animator>();
        // set starting position & oritentation for seagull
        transform.position = new Vector3(0.0f, soarHeight, radiusOfOrbit);
        transform.Rotate(Vector3.up, 90, Space.Self);

        fireCountDown = fireDelay;
        
        laser = gameObject.AddComponent<LineRenderer>();
        laser.material = laserMaterial;
        laser.endWidth = 0.4f;
        laser.startWidth = 0.4f;

        movement = Player.GetComponent<movement>();
        audioSrc = GetComponent<AudioSource>();
    }
    
    void OnDisable()
    {
        isFlying = false;
        laser.positionCount = 0;
    }

    void Update()
    {
        delta = Time.deltaTime;
        totalTime += delta;
        fireCountDown -= delta;
        
        if (isFlying)
        {
            // make the seagull move in an orbit over the arena
            transform.position = new Vector3(radiusOfOrbit * Mathf.Sin(totalTime * orbitSpeed),
                soarHeight + maxHeightChange * Mathf.Sin(totalTime * ySpeed),
                radiusOfOrbit * Mathf.Cos(totalTime * orbitSpeed));

            // orient the seagull to face the right direction
            transform.Rotate(Vector3.up, delta / (Mathf.PI * 2) * 360 * orbitSpeed, Space.Self);
            
            // fire lasers from the seagull's eyes
            switch (currentLevel)
            {
                case 1:
                    Level1();
                    break;
                case 2:
                    Level2();
                    break;
                case 3:
                    Level3();
                    break;
                case 4:
                    Level4();
                    break;
                case 5:
                    Level5();
                    break;
            }
            
        } else
        {
            //if (transform.position == battlePosition) animator.SetTrigger("IdleToFly");
            transform.position = Vector3.MoveTowards(transform.position, flyPosition, 50 * Time.deltaTime);

            
            if (transform.position == flyPosition)
            {
                isFlying = true;
                totalTime = 0;
                transform.Rotate(Vector3.up, 90, Space.Self);
            }
        }     
    }

    void Level1()
    {
        if (fireCountDown <= 0)
        {
            fireCountDown = fireDelay;
            ShootLaser();
        }
    }

    void Level2()
    {
        if (fireCountDown >= 0)
        {
            GetLongLaserTarget();
        }
        if ((fireCountDown >= -2) & (fireCountDown <= 0))
        {
            // during the specified countdown during the laser should fire, call the method
            ShootLongLaser();
        }
        if (fireCountDown <= -2)
        {
            // at the end of the duration
            // stop the sound effect
            audioSrc.Stop();
            // set the number of line positions to 0, removing the line
            laser.positionCount = 0;
            // reset the countdown
            fireCountDown = fireDelay;
            ShootLaser();
        }
    }
    
    void Level3()
    {
        if (fireCountDown >= -1)
        {
            GetLongLaserTarget();
        }
        if ((fireCountDown >= -3) & (fireCountDown <= -1))
        {
            // during the specified countdown during the laser should fire, call the method
            ShootLongLaser();
        }
        if (fireCountDown <= -3)
        {
            // at the end of the duration
            // stop the sound effect
            audioSrc.Stop();
            // set the number of line positions to 0, removing the line
            laser.positionCount = 0;
            // reset the countdown
            fireCountDown = fireDelay;
            ShootLaser();
            StartCoroutine(ShootFireballs(3, 0.5f, 0.5f));
        }
    }
    
    void Level4()
    {
        if (fireCountDown >= -1)
        {
            GetLongLaserTarget();
        }
        if ((fireCountDown >= -3) & (fireCountDown <= -1))
        {
            // during the specified countdown during the laser should fire, call the method
            ShootLongLaser();
        }
        if (fireCountDown <= -3)
        {
            // at the end of the duration
            // stop the sound effect
            audioSrc.Stop();
            // set the number of line positions to 0, removing the line
            laser.positionCount = 0;
            // reset the countdown
            fireCountDown = fireDelay;
            StartCoroutine(ShootLasers(3, 0.2f));
            StartCoroutine(ShootFireballs(3, 0.3f, 1.0f));
        }
    }

    void Level5()
    {
        if (fireCountDown >= -1)
        {
            GetLongLaserTarget();
        }
        if ((fireCountDown >= -3) & (fireCountDown <= -1))
        {
            // during the specified countdown during the laser should fire, call the method
            ShootLongLaser();
        }
        if ((fireCountDown >= -3.5) & (fireCountDown <= -3))
        {
            GetLongLaserTarget();
            // stop the sound effect
            audioSrc.Stop();
            // set the number of line positions to 0, removing the line
            laser.positionCount = 0;
        }
        if ((fireCountDown >= -5.5) & (fireCountDown <= -3.5))
        {
            // during the specified countdown during the laser should fire, call the method
            ShootLongLaser();
        }
        if (fireCountDown <= -5.5)
        {
            // at the end of the duration
            // stop the sound effect
            audioSrc.Stop();
            // set the number of line positions to 0, removing the line
            laser.positionCount = 0;
            // reset the countdown
            fireCountDown = fireDelay;
            StartCoroutine(ShootLasers(5, 0.2f));
            StartCoroutine(ShootFireballs(3, 0.3f, 1.0f));
        }
    }

    void GetLongLaserTarget()
    {
        // pick the initial start point of the laser
        // slightly in front of the players forward direction
        laserTarget = target.transform.position + target.transform.forward*30;
        laserTarget.y = 0;
        // pick a random direction
        laserDirection = Random.insideUnitCircle;
        // this value is in 2d, need to swap y and z
        laserDirection.z = laserDirection.y;
        laserDirection.y = 0;
        // set the start and counter to the current time
        startTime = Time.time;
        counterTime = Time.time;
    }
    void ShootLaser()
    {
        audioSrc.PlayOneShot(laserSound, 0.6f);
        // left eye laser
        //Vector3 direction = target.transform.position - leftEye.position;
        if (movement.inputDir.magnitude > 0)
        {
            laserTarget = target.transform.position + target.transform.forward*40;
        }
        else
        {
            laserTarget = target.transform.position;
        }
        
        Vector3 direction = laserTarget - leftEye.position;
        GameObject laser = Instantiate(laserPrefab, leftEye.position, Quaternion.LookRotation(direction));
        //laser.transform.position = leftEye.position;
        //laser.transform.LookAt(Vector3.zero);
        laser.GetComponent<LaserController>().direction = direction;
    
        //Debug.DrawLine(Vector3.zero, -direction);
    
        // right eye laser
        //direction = target.transform.position - rightEye.position;
//        direction = laserTarget - rightEye.position;
//        laser = Instantiate(laserPrefab, rightEye.position, Quaternion.LookRotation(direction));
//        //laser.transform.position = rightEye.position;
//        //laser.transform.LookAt(Vector3.zero);
//        laser.GetComponent<LaserController>().direction = direction;
    
        //Debug.DrawLine(Vector3.zero, -direction);
    }

    void ShootLongLaser()
    {
        // play sound effect
        if (!audioSrc.isPlaying) audioSrc.PlayOneShot(laserBeamSound, 0.8f);
        // normalise the direction vector
        laserDirection.Normalize();
        // get the length between the starting point, and end point of the laser
        float length = Vector3.Distance(laserTarget, laserDirection * laserMoveLength);
        // get the total distance covered, this is the speed effectively
        float disCovered = (Time.time - startTime) * 25;
        // lerp between the two values, dividing the covered distance by the total distance to lerp over a percentage
        // from 0 to 1
        Vector3 laserPosition = Vector3.Lerp(laserTarget, laserDirection * laserMoveLength, disCovered / length);
        // set number of line vertices
        laser.positionCount = 4;
        laser.material = laserMaterial;
        // draw a line between these points
        laser.SetPosition(0, laserPosition);
        laser.SetPosition(1, rightEye.position);
        laser.SetPosition(2, leftEye.position);
        laser.SetPosition(3, laserPosition);
        
        // create a flame trail behind the laser every quarter second so there aren't too many particles
        float counter = Time.time - counterTime;
        if (counter >= 0.25)
        {
            counterTime = Time.time;
            GameObject fire = Instantiate(flame);
            fire.transform.position = laserPosition;
        }
    }
    

    IEnumerator ShootFireballs(int count, float delay, float initialDelay)
    {
        yield return new WaitForSeconds(initialDelay);
        for (int i = 0; i<count; i++)
        {
            yield return new WaitForSeconds(delay);
            audioSrc.PlayOneShot(fireballSound, 0.4f);
            Vector3 direction = laserTarget - mouth.position;
            GameObject fire = Instantiate(fireball, mouth.position, 
                                          Quaternion.LookRotation(direction));
            fire.GetComponent<flameCollision>().direction = direction;
        }
    }

    IEnumerator ShootLasers(int count, float delay)
    {
        for (int i = 0; i<count; i++)
        {
            yield return new WaitForSeconds(delay);
            ShootLaser();
        }
    }

    IEnumerator IdleToSky()
    {
        Debug.Log("test");
        animator.SetTrigger("IdleToTakeOff");
        yield return new WaitForSeconds(0.5f);
        animator.SetTrigger("TakeOffToFly");
    }
}
