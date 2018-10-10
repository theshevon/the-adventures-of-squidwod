using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeagullBossController : MonoBehaviour
{
	public GameObject target;

	bool movingDown;
    bool movingToCentre;
    public bool onGround;
	LineRenderer laser;
	AudioSource audioSrc;
	float currentStep;
    public float totalTime;
    float delta;

    // orbit related 
    const float radiusOfOrbit = 120.0f;
    const float orbitSpeed = 1.5f;
    const float soarHeight = 125.0f;
    const float ySpeed = 0.5f;
    const float maxHeightChange = 25.0f;

    // boss fight animation params 
    const float attackDelay = 2;
    const int NUM_OF_ATTACKS = 2;
    const int FLYING_TO_DIVE = 1;
    const int DIVE_TO_LAND = 2;
    const int LAND_TO_IDLE = 3;
    const int IDLE_TO_FLAP = 4;
    const int FLAP_TO_IDLE = 5;
    const int IDLE_TO_THROW = 6;
    const int THROW_TO_IDLE = 7;
    const int IDLE_TO_TAKEOFF = 8;
    const int TAKEOFF_TO_FLYING = 9;
    const int IDLE_TO_DIE = 10;
    float attackCountDown;

    Animator animator;
    Vector3 entryPoint;
    Vector3 moveDirection;
    Vector3 centrePos;
    bool landAnimPlayed;
    float speed = 0.01f;

    // jump related
    Vector3 lookDir;
    const float maxAngle = 40;

    readonly Vector3 battlePosition = new Vector3(0,2,0);

	void Start ()
	{
		laser = GetComponent<LineRenderer>();
		audioSrc = GetComponent<AudioSource>();
		animator = GetComponent<Animator>();
		laser.positionCount = 0;
        attackCountDown = attackDelay;
        audioSrc.Stop();
	}

	void OnEnable()
	{
        movingDown = false;
        movingToCentre = false;
        onGround = false;
	}

    void OnDisable()
    {
        onGround = false;
    }


	// Update is called once per frame
	void Update () {

        delta = Time.deltaTime;
        //attackCountDown -= Time.deltaTime;


        if (onGround)
        {
            // turn to face player
            if (GetAngleBetween() > maxAngle)
            {
                int i = (target.transform.position - transform.position).x > 0 ? 1 : -1;
                transform.Rotate(Vector3.up, GetAngleBetween() * i, Space.Self);
            }
        }
        else
        {
            if (!movingDown)
            {
                movingToCentre = true;
                centrePos = Vector3.zero;
                centrePos.y = 250;
                transform.position = Vector3.MoveTowards(transform.position, centrePos, 50 * Time.deltaTime);
                transform.LookAt(centrePos);
                if (transform.position == centrePos)
                {
                    movingDown = true;
                    animator.SetTrigger("FlyToDive");
                }
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, battlePosition, 90 * Time.deltaTime);
                transform.LookAt(target.transform);
                transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);

                if (transform.position.y < 100 && !landAnimPlayed) { 
                    animator.SetTrigger("DiveToLand");
                    landAnimPlayed = true;
                }

                if (transform.position == battlePosition)
                {
                    onGround = true;
                    animator.SetTrigger("LandToIdle");
                    
                    //transform.LookAt(target.transform);
                    //transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
                }

            }
        }

    }

    float GetAngleBetween()
    {
        lookDir = target.transform.position - transform.position;
        return Vector3.Angle(lookDir, transform.forward);
    }
}
