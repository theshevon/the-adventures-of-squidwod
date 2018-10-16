using System;
using System.Collections;
using UnityEngine;

public class SeagullBossController : MonoBehaviour
{
    public GameObject laserPrefab;
    public GameObject flamePrefab;
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

    public Material gustMaterial;
    Material laserMaterial;
    public bool attacksLocked;

    LineRenderer lineRenderer;
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
    const int NUM_OF_ATTACKS = 4;
    int nextAttack;
    float nextAttackDelay;
    readonly float[] attackDelays = { 5.0f, 6.0f, 4.0f, 8.0f }; // index 1 is time to wait after first attack starts,
                                                                // index 2 is time to wait after second attack starts and so on...
    float attackCountdown;

    Animator animator;
    Vector3 moveDirection;
    Vector3 centrePos;

    bool landAnimPlayed;

    // turn related
    float rotationSpeed = 1.5f;
    const float maxRotationTime = 1.0f;
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
    readonly Vector3 offset = new Vector3(0, 15.5f, 0);
    float stepper = 0.1f;
    const float laserDuration = 2.0f;
    bool laserInUse;
    float laserUseTime;

    readonly Vector3 battlePosition = new Vector3(0, 2, 0);

    // gust related
    readonly Vector3 jumpPos = new Vector3(0,12,0);
    const float maxGustDuration = 4;
    const float jumpDuration = maxGustDuration / 2;
    const float initNVertices = 36;
    float gustDuration;
    bool usingGust;
    const int angleStepGust = 5;
    const float laserRingHeight = 5;

    // ring of fire
    const float ringRadius = 25;
    const float maxRingIncrease = 60;
    const int angleStepFire = 10;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        laserMaterial = lineRenderer.material;
        audioSrc = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        lineRenderer.positionCount = 0;
        audioSrc.Stop();
    }

    void OnEnable()
    {
        attacksLocked = false;
        movingDown = false;
        isOnGround = false;
        landAnimPlayed = false;
        nextAttackDelay = 4;
        leftEnd = leftLaserStart.position;
        rightEnd = rightLaserStart.position;
    }

    void OnDisable()
    {
        isOnGround = false;
        DestroyFireballs();
    }

    void DestroyFireballs()
    {

        GameObject[] ringFireballs = GameObject.FindGameObjectsWithTag("Fireball");
        for (int i = 0; i < ringFireballs.Length; i++)
        {
            Destroy(ringFireballs[i], 2);
        }
    }

    void Update()
    {

        delta = Time.deltaTime;

        if (isOnGround)
        {

            attackCountdown -= delta;

            // laser updates
            if (laserInUse)
            {
                laserUseTime += delta;

                if (laserUseTime >= laserDuration)
                {
                    rotationLocked = false;
                    laserInUse = false;
                    isAttacking = false;

                    laserUseTime = 0;
                    lineRenderer.positionCount = 0;
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

            // gust updates
            if (usingGust)
            {
                gustDuration += delta;
                if (gustDuration < jumpDuration / 2)
                {
                    transform.position = Vector3.Lerp(battlePosition, jumpPos, gustDuration / (jumpDuration/2));
                }
                else if (gustDuration < jumpDuration)
                {
                    transform.position = Vector3.Lerp(jumpPos, battlePosition, (gustDuration - jumpDuration/2) / (jumpDuration/2));
                }
                else if (gustDuration < maxGustDuration){
                    float factor = (gustDuration - maxGustDuration / 2) / (maxGustDuration / 2);
                    MakeLaserRing(ringRadius + maxRingIncrease*factor);
                }
                else {
                    gustDuration = 0;
                    usingGust = false;
                    rotationLocked = false;
                    isAttacking = false;
                    animator.SetTrigger("WingflapToIdle");
                    lineRenderer.positionCount = 0;
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
                rotationTime += delta;
                float fraction = rotationTime / maxRotationTime;
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, fraction);
                if (fraction >= 1){
                    rotating = false;
                    rotationTime = 0;
                }
            }

            // follow player around while spewing flamethrower
            if (isAttacking && !rotationLocked && !rotating)
            {
                relativePosition = target.transform.position - transform.position;
                targetRotation = Quaternion.LookRotation(relativePosition);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, delta * rotationSpeed * 5);
            }

            if (attackCountdown <= 0 && !rotating && !isAttacking && !attacksLocked)
            {

                isAttacking = true;
                nextAttackDelay = attackDelays[nextAttack];
                attackCountdown = nextAttackDelay;

                switch (nextAttack)
                {
                    case 0:
                        UseGust();
                        break;
                    case 1:
                        UseGrenade();
                        break;
                    case 2:
                        UseLaserBeam();
                        break;
                    case 3:
                        UseFlameThrower();
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

                    // fire laser shot at centre
                    for (int i = 0; i < 3; i++)
                    {
                        ShootLaserAtCentre();
                    }
                    ExecuteAfterTime(0.01f, ShootLaserAtCentre);

                    // animate
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

                if (transform.position.y <= battlePosition.y)
                {
                    transform.position = battlePosition;
                    isOnGround = true;
                    MakeRingOfFire();
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
        lineRenderer.material = laserMaterial;
        laserInUse = true;
        rotationLocked = true;
        lineRenderer.positionCount = 4;
        lineRenderer.SetPosition(0, leftEye.position);
        lineRenderer.SetPosition(3, rightEye.position);
        UpdateLaserPositions();
    }

    void UpdateLaserPositions()
    {
        float fraction = laserUseTime / laserDuration;
        leftEnd = Vector3.Lerp(leftLaserStart.position, leftLaserEnd.position, fraction);
        rightEnd = Vector3.Lerp(rightLaserStart.position, rightLaserEnd.position, fraction);
        lineRenderer.SetPosition(1, leftEnd);
        lineRenderer.SetPosition(2, rightEnd);
        if (fraction >= stepper)
        {
            explosionPos1 = leftEnd;
            explosionPos2 = rightEnd;
            stepper += 0.1f;
            CreateExplosion();
        }
    }

    void CreateExplosion()
    {
        GameObject e1 = Instantiate(explosionPrefab, explosionPos1 + offset, transform.rotation);
        GameObject e2 = Instantiate(explosionPrefab, explosionPos2 + offset, transform.rotation);
        GameObject f1 = Instantiate(fireballPrefab, explosionPos1 + offset, transform.rotation);
        GameObject f2 = Instantiate(fireballPrefab, explosionPos2 + offset, transform.rotation);
        Destroy(e1, 2);
        Destroy(e2, 2);
        Destroy(f1, 5);
        Destroy(f2, 5);
    }

    void UseGust()
    {
        usingGust = true;
        rotationLocked = true;
        animator.SetTrigger("IdleToWingflap");
        lineRenderer.material = gustMaterial;
        lineRenderer.positionCount = 360/angleStepGust;
    }

    public void Die()
    {
        animator.SetTrigger("IdleToDie");
    }

    void ShootLaserAtCentre()
    {
        //Debug.Log("Shooting");
        Vector3 direction = Vector3.zero - leftEye.position;
        GameObject laserShot = Instantiate(laserPrefab, leftEye.position, Quaternion.LookRotation(direction));
        laserShot.GetComponent<LaserController>().direction = direction;
        laserShot.GetComponent<LaserController>().explosionType = 1;
        direction = Vector3.zero - rightEye.position;
        laserShot = Instantiate(laserPrefab, rightEye.position, Quaternion.LookRotation(direction));
        laserShot.GetComponent<LaserController>().direction = direction;
        laserShot.GetComponent<LaserController>().explosionType = 1;
    }

    void MakeRingOfFire()
    {
        //Debug.Log("Making ring of fire");
        for (int angle = 0, i = 0; angle < 360; angle += angleStepFire, i++)
        {
            Vector3 position = new Vector3(Mathf.Sin(angle), 0.1f, Mathf.Cos(angle)) * ringRadius;
            GameObject fireball = Instantiate(flamePrefab, position, transform.rotation);
            if (i % 2 == 0)
            {
                fireball.transform.localScale = new Vector3(2, 2, 2);
            }
            else
            {
                fireball.transform.localScale = new Vector3(3, 3, 3);
            }
        }
    }

    void MakeLaserRing(float radius)
    {

        for (int i = 0, angle = 0; i < 360/angleStepGust; i++, angle += angleStepGust)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            float z = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

            lineRenderer.SetPosition(i, new Vector3(x, laserRingHeight, z));
            //Debug.DrawLine(Vector3.zero, new Vector3(x, y, z));
        }
    }

    public void TakeOff(){
        animator.SetTrigger("IdleToTakeoff");
    }

    public bool IsIdle(){
        return !isAttacking && attacksLocked;
    }
}

