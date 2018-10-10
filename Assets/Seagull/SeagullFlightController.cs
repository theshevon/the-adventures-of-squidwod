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
    public int difficulty = 1;

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
            if (difficulty == 1) {Level1();}
            if (difficulty == 2) {Level2();}
            
        } else
        {
            if (transform.position == battlePosition)
            {
                //animator.SetTrigger("IdleToFly");
            }
            
            transform.position = Vector3.MoveTowards(transform.position, flyPosition, 50 * Time.deltaTime);
            
            if (transform.position == flyPosition)
            {
                isFlying = true;
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
        if (fireCountDown >= -1)
        {
            laserTarget = target.transform.position + target.transform.forward*30;
            laserTarget.y = 0;
            laserDirection = Random.insideUnitCircle;
            laserDirection.z = laserDirection.y;
            laserDirection.y = 0;
            startTime = Time.time;
            counterTime = Time.time;
        }
        if ((fireCountDown >= -3) & (fireCountDown <= -1))
        {
            ShootLongLaser();
        }
        if (fireCountDown <= -3)
        {
            audioSrc.Stop();
            laser.positionCount = 0;
            fireCountDown = fireDelay;
            ShootLaser();
            StartCoroutine(ShootFireballs(3));
        }
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
        direction = laserTarget - rightEye.position;
        laser = Instantiate(laserPrefab, rightEye.position, Quaternion.LookRotation(direction));
        //laser.transform.position = rightEye.position;
        //laser.transform.LookAt(Vector3.zero);
        laser.GetComponent<LaserController>().direction = direction;
    
        //Debug.DrawLine(Vector3.zero, -direction);
    }

    void ShootLongLaser()
    {
        if (!audioSrc.isPlaying) audioSrc.PlayOneShot(laserBeamSound, 0.8f);
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
        
        float counter = Time.time - counterTime;
        if (counter >= 0.25)
        {
            counterTime = Time.time;
            GameObject fire = Instantiate(flame);
            fire.transform.position = laserPosition;
        }
    }

    IEnumerator ShootFireballs(int count)
    {
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i<count; i++)
        {
            audioSrc.PlayOneShot(fireballSound, 0.4f);
            yield return new WaitForSeconds(0.5f);
            Vector3 direction = laserTarget - mouth.position;
            GameObject fire = Instantiate(fireball, mouth.position, 
                                          Quaternion.LookRotation(direction));
            fire.GetComponent<flameCollision>().direction = direction;
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
