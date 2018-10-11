using System;
using System.Collections;
using UnityEngine;

public class SeagullBossController : MonoBehaviour
{
    public GameObject target;
    public GameObject fireSource;
    public GameObject grenadePrefab;
    public GameObject grenadePouch;
    public GameObject flameThrowerPrefab;
    public AudioClip laserSound;
    public AudioClip laserBeamSound;
    public AudioClip fireballSound;
    public GameObject flame;
    public GameObject fireball;
    public GameObject Player;
    public Material laserMaterial;

    SeagullHealthManager healthManager;
    LineRenderer laser;
    AudioSource audioSrc;

    bool movingDown;
    bool isOnGround;
    float delta;

    // orbit related 
    const float radiusOfOrbit = 120.0f;
    const float orbitSpeed = 1.5f;
    const float soarHeight = 125.0f;
    const float ySpeed = 0.5f;
    const float maxHeightChange = 25.0f;

    // boss fight animation params 
    const int NUM_OF_ATTACKS = 2;
    int nextAttack;
    float nextAttackDelay;
    readonly float[] attackDelays = { 5.0f, 6.0f }; // index 1 is time to wait after first attack starts
                                                    // index 2 is time to wait after second attack starts
    float attackCountdown;

    Animator animator;
    Vector3 moveDirection;
    Vector3 centrePos;

    bool landAnimPlayed;

    // jump related
    public float rotationSpeed = 1.5f;
    Vector3 relativePosition;
    Quaternion targetRotation;
    bool rotating;
    float rotationTime;
    const float maxAngle = 45;

    // attack related
    bool isAttacking;
    bool rotationLocked;
    bool rotatingWhileAttacking;

    readonly Vector3 battlePosition = new Vector3(0,2,0);

	void Start ()
	{
        healthManager = GetComponent<SeagullHealthManager>();
		laser = GetComponent<LineRenderer>();
		audioSrc = GetComponent<AudioSource>();
		animator = GetComponent<Animator>();
		laser.positionCount = 0;
        audioSrc.Stop();
	}

	void OnEnable()
	{
        movingDown = false;
        isOnGround = false;
        landAnimPlayed = false;
        nextAttackDelay = 4;
    }

    void Update () {

        delta = Time.deltaTime;

        if (isOnGround)
        {
            attackCountdown -= delta;

            // rotate seagull to face player while not attacking
            if (GetAngleBetween() >= maxAngle && !isAttacking)
            {
                animator.SetTrigger("IdleToWalk");
                relativePosition = target.transform.position - transform.position;
                targetRotation = Quaternion.LookRotation(relativePosition);
                rotating = true;
            }

            if (rotating)
            {
                rotationTime += delta * rotationSpeed;
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationTime);
                if (rotationTime >= 1)
                {
                    rotating = false;
                    animator.SetTrigger("WalkToIdle");
                }
            }

            // follow player around while spewing flamethrower
            if (isAttacking && !rotationLocked && !rotating){
                relativePosition = target.transform.position - transform.position;
                targetRotation = Quaternion.LookRotation(relativePosition);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, delta * rotationSpeed * 5);
                //animator.SetTrigger("ThrowTurnForward");
            }

            if (attackCountdown <= 0 && !rotating && !isAttacking){

                isAttacking = true;
                nextAttackDelay = attackDelays[nextAttack];
                attackCountdown = nextAttackDelay;

                switch (nextAttack){
                    case 0:
                        UseGrenade();
                        break;
                    case 1:
                        UseFlameThrower();
                        break;
                }

                nextAttack = UnityEngine.Random.Range(0, NUM_OF_ATTACKS);
            }

            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        }
        else
        {
            if (!movingDown)
            {
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
                    attackCountdown = nextAttackDelay;
                    nextAttack = UnityEngine.Random.Range(0, NUM_OF_ATTACKS);
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

    void UseFlameThrower()
    {
        animator.SetTrigger("IdleToFire");
        StartCoroutine(ExecuteAfterTime(0.3f, ReleaseHell));
        StartCoroutine(ExecuteAfterTime(5.3f, CalmDown));
    }

    void ReleaseHell()
    {
        GameObject ft = Instantiate(flameThrowerPrefab, fireSource.transform.position, transform.rotation);
        FlameThrowerController ftScript = ft.GetComponent<FlameThrowerController>();
        ftScript.source = fireSource;
        ftScript.sourceBody = gameObject;
        Destroy(ft, 5);
        isAttacking = true;
    }

    void CalmDown()
    {
        animator.SetTrigger("FireToIdle");
        isAttacking = false;
    }

    void UseGrenade()
    {
        rotationLocked = true;
        animator.SetTrigger("IdleToThrow");
        StartCoroutine(ExecuteAfterTime(0.3f, ThrowGrenade));
        StartCoroutine(ExecuteAfterTime(0.5f, ThrowToIdle));
    }

    void ThrowGrenade(){
        GameObject grenade = Instantiate(grenadePrefab, grenadePouch.transform.position, grenadePrefab.transform.rotation);
        grenade.GetComponent<GrenadeScript>().target = target;
    }

    void ThrowToIdle()
    {
        animator.SetTrigger("ThrowToIdle");
        isAttacking = false;
        rotationLocked = false;
    }


    void WingFlapToIdle()
    {
        animator.SetTrigger("WingflapToIdle");
    }
    
   

   

    //void ShootLongLaser()
    //{
    //    laserDirection.Normalize();
    //    float length = Vector3.Distance(laserTarget, laserDirection * laserMoveLength);
    //    float disCovered = (Time.time - startTime) * 25;
    //    Vector3 laserPosition = Vector3.Lerp(laserTarget, laserDirection * laserMoveLength, disCovered / length);
    //    laser.positionCount = 4;
    //    //laser.SetPosition(0, laserPos + (laserDirection * Time.deltaTime * 10));
    //    laser.SetPosition(0, laserPosition);
    //    laser.SetPosition(1, rightEye.position);
    //    laser.SetPosition(2, leftEye.position);
    //    laser.SetPosition(3, laserPosition);

    //    float counter = Time.time - counterTime;
    //    if (counter >= 0.25)
    //    {
    //        counterTime = Time.time;
    //        GameObject fire = Instantiate(flame);
    //        fire.transform.position = laserPosition;
    //    }
    //}

    //IEnumerator ShootFireballs(int count)
    //{
    //    yield return new WaitForSeconds(0.5f);
    //    for (int i = 0; i < count; i++)
    //    {
    //        audioSrc.PlayOneShot(fireballSound, 0.4f);
    //        yield return new WaitForSeconds(0.5f);
    //        Vector3 direction = laserTarget - mouth.position;
    //        GameObject fire = Instantiate(fireball, mouth.position,
    //                                      Quaternion.LookRotation(direction));
    //        fire.GetComponent<flameCollision>().direction = direction;
    //    }
    //}
}
