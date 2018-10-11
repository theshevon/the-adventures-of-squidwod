using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeagullBossController : MonoBehaviour
{
	public GameObject target;
    public GameObject fireSource;
    public GameObject grenadePrefab;
    public GameObject grenadePouch;
    public GameObject flameThrowerPrefab;

    SeagullHealthManager healthManager;
    LineRenderer laser;
    AudioSource audioSrc;

    bool movingDown;
    bool movingToCentre;
    bool isOnGround;
	float currentStep;
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
    float attackCountDown;

    Animator animator;
    Vector3 moveDirection;
    Vector3 centrePos;

    bool landAnimPlayed;

    // jump related
    public float rotationSpeed = 1.5f;
    Vector3 relativePosition;
    Quaternion targetRotation;
    float jumpHeightInc = 20;
    bool rotating;
    float rotationTime;
    const float maxAngle = 45;
    
    // attack related
    bool isAttacking;

    readonly Vector3 battlePosition = new Vector3(0,2,0);

	void Start ()
	{
        healthManager = GetComponent<SeagullHealthManager>();
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
        isOnGround = false;
        landAnimPlayed = false;
    }

    void Update () {

        delta = Time.deltaTime;
        //attackCountDown -= Time.deltaTime;
        
        if (isOnGround)
        {
            // turn to face player
            if (GetAngleBetween() > maxAngle && !isAttacking)
            {
                relativePosition = target.transform.position - transform.position;
                targetRotation = Quaternion.LookRotation(relativePosition);
                rotating = true;
                animator.SetTrigger("IdleToWingflap");
            }

            if (rotating){
                rotationTime += delta * rotationSpeed;
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationTime);
                if (rotationTime < 0.5){
                    //transform.position += new Vector3(0, jumpHeightInc, 0);
                    transform.position = Vector3.MoveTowards(transform.position, new Vector3(0, jumpHeightInc, 0), rotationTime*2);
                }
                else if (rotationTime < 1){
                    //transform.position -= new Vector3(0, jumpHeightInc, 0);
                    transform.position = Vector3.MoveTowards(transform.position, battlePosition, rotationTime*2);
                }else{
                    rotationTime = 0;
                    rotating = false;
                    transform.position = battlePosition;
                    //animator.SetTrigger("WingflapToIdle");
                }
            }

            // represents bird being hit by eggs for testing
            if (Input.GetKeyDown(KeyCode.G) && !isAttacking && !rotating){
                animator.SetTrigger("IdleToWingflap");
            }

            // represents bird throwing grenade for testing
            if (Input.GetKeyDown(KeyCode.F) && !isAttacking && !rotating){
                animator.SetTrigger("IdleToThrow");
                isAttacking = true;
                StartCoroutine(ExecuteAfterTime(0.5f, ThrowToIdle));
                StartCoroutine(ExecuteAfterTime(0.3f, ThrowGrenade));
            }

            // represents bird using flamethrower
            if (Input.GetKeyDown(KeyCode.H) && !isAttacking && !rotating){
                animator.SetTrigger("IdleToFire");
                StartCoroutine(ExecuteAfterTime(0.3f, ReleaseHell));
                StartCoroutine(ExecuteAfterTime(5.3f, CalmDown));
            }
            
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
            
        }
        else
        {
            if (!movingDown)
            {
                movingToCentre = true;
                centrePos = Vector3.zero;
                centrePos.y = 220;
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
                    isOnGround = true;
                    animator.SetTrigger("LandToIdle");
                }

            }
        }
    }

    float GetAngleBetween()
    {
        relativePosition = target.transform.position - transform.position;
        return Vector3.Angle(relativePosition, transform.forward);
    }

    public bool IsOnGround(){
        return isOnGround;
    }

    IEnumerator ExecuteAfterTime(float time, Action action)
    {
        yield return new WaitForSeconds(time);
        action();
    }

    void ThrowGrenade(){
        GameObject grenade = Instantiate(grenadePrefab, grenadePouch.transform.position, grenadePrefab.transform.rotation);
        grenade.GetComponent<GrenadeScript>().target = target;
    }

    void ReleaseHell(){
        GameObject ft = Instantiate(flameThrowerPrefab, fireSource.transform.position, transform.rotation);
        Destroy(ft, 5);
        isAttacking = true;
    }

    void CalmDown(){
        animator.SetTrigger("FireToIdle");
        isAttacking = false;
    }

    void WingFlapToIdle()
    {
        animator.SetTrigger("WingflapToIdle");
    }
    
    void ThrowToIdle()
    {
        animator.SetTrigger("ThrowToIdle");
        isAttacking = false;
    }
   
}
