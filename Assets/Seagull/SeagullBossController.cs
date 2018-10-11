using System;
using System.Collections;
using UnityEngine;

public class SeagullBossController : MonoBehaviour
{
    public GameObject grenadePrefab;
    public GameObject flameThrowerPrefab;
    public GameObject fireballPrefab;
    public GameObject explosionPrefab;

    public Transform target;
    public Transform fireSource;
    public Transform grenadePouch;
    public Transform leftEye;
    public Transform rightEye;
    public Transform leftLaserStart;
    public Transform rightLaserStart;
    public Transform leftLaserEnd;
    public Transform rightLaserEnd;

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
    const int NUM_OF_ATTACKS = 3;
    int nextAttack;
    float nextAttackDelay;
    readonly float[] attackDelays = { 5.0f, 6.0f, 4.0f }; // index 1 is time to wait after first attack starts,
                                                          // index 2 is time to wait after second attack starts and so on
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

    // laser related
    Vector3 leftEnd;
    Vector3 rightEnd;
    Vector3 explosionPos1;
    Vector3 explosionPos2;
    float stepper = 0.1f;
    const float laserDuration = 2.0f;
    bool laserInUse;
    float laserUseTime;

    readonly Vector3 battlePosition = new Vector3(0, 2, 0);

    void Start()
    {
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
        leftEnd = leftLaserStart.position;
        rightEnd = rightLaserStart.position;
    }

    void Update()
    {

        delta = Time.deltaTime;

        if (isOnGround)
        {
            attackCountdown -= delta;

            if (laserInUse)
            {
                laserUseTime += delta;

                if (laserUseTime >= laserDuration)
                {
                    rotationLocked = false;
                    laserInUse = false;
                    isAttacking = false;

                    laserUseTime = 0;
                    laser.positionCount = 0;
                    stepper = 0.1f;

                    leftEnd = leftLaserStart.position;
                    rightEnd = rightLaserStart.position;

                    animator.SetTrigger("LeanToIdle");
                }
                else
                {
                    UpdateLaserPositions();
                }
            }

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
            if (isAttacking && !rotationLocked && !rotating)
            {
                relativePosition = target.transform.position - transform.position;
                targetRotation = Quaternion.LookRotation(relativePosition);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, delta * rotationSpeed * 5);
                //animator.SetTrigger("ThrowTurnForward");
            }

            if (attackCountdown <= 0 && !rotating && !isAttacking)
            {

                isAttacking = true;
                nextAttackDelay = attackDelays[nextAttack];
                attackCountdown = nextAttackDelay;

                switch (nextAttack)
                {
                    case 0:
                        UseGrenade();
                        break;
                    case 1:
                        UseFlameThrower();
                        break;
                    case 2:
                        UseLaserBeam();
                        break;
                }

                nextAttack = UnityEngine.Random.Range(0, NUM_OF_ATTACKS);
            }
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

                if (transform.position.y < 100 && !landAnimPlayed)
                {
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

    public bool IsOnGround()
    {
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
        ftScript.sourceBody = gameObject.transform;
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

    void ThrowGrenade()
    {
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

    void UseLaserBeam()
    {
        animator.SetTrigger("IdleToLean");
        laserInUse = true;
        rotationLocked = true;
        laser.positionCount = 4;
        laser.SetPosition(0, leftEye.position);
        laser.SetPosition(3, rightEye.position);
        UpdateLaserPositions();
    }

    void UpdateLaserPositions()
    {
        float fraction = laserUseTime / laserDuration;
        leftEnd = Vector3.Lerp(leftLaserStart.position, leftLaserEnd.position, fraction);
        rightEnd = Vector3.Lerp(rightLaserStart.position, rightLaserEnd.position, fraction);
        laser.SetPosition(1, leftEnd);
        laser.SetPosition(2, rightEnd);
        if (fraction >= stepper){
            explosionPos1 = leftEnd;
            explosionPos2 = rightEnd;
            stepper += 0.1f;
            CreateExplosion();
        }
    }

    void CreateExplosion(){
        GameObject e1 = Instantiate(explosionPrefab, explosionPos1, transform.rotation);
        GameObject e2 = Instantiate(explosionPrefab, explosionPos2, transform.rotation);
        GameObject f1 = Instantiate(fireballPrefab, explosionPos1, transform.rotation);
        GameObject f2 = Instantiate(fireballPrefab, explosionPos2, transform.rotation);
        Destroy(e1, 2);
        Destroy(e2, 2);
        Destroy(f1, 5);
        Destroy(f2, 5);
    }

}
