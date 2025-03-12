using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(EntityHealth), typeof(Animator), typeof(Rigidbody2D))]
public class SentinelMovement : Enemy
{
    Rigidbody2D rb;
    SpriteRenderer sr;
    public SpriteRenderer coneSr;  // Sichtfeld
    Animator anim;
    EntityHealth eh;

    [Header("Settings")]
    [Tooltip("Velocity Speed for wandering around")]
    public Vector2 speed = new Vector2(5f, 0f);
    public float chaseForce = 495f;
    public Vector2 directionChangeTimeRange = new Vector2(1f, 5f);
    public float maxHeadingChange = 30f;
    public float areaScanTickRate = 1f;
    public float viewDistance = 30f;
    public LayerMask playerLayer;
    [Tooltip("Max Chasing Speed")]
    public float maxVelocity = 30f;
    public float rotationSpeed = 10f;
    public float smoothTime = 0.5f;
    public float minVelocity = 5f;

    private Vector2 velocity;

    Coroutine wanderRoutine;
    Coroutine watchRoutine;

    [Tooltip("in degrees")]
    public float maxRotation = 45;

    float headingAngle;
    Quaternion currentRotation;
    internal bool chasing;
    bool startFlipX;    // state of SpriteRenderer Flip variable for x, at Start
    private Vector2 startSpeed;


    private Vector2 temporaryTargetPosition;
    private float temporaryTargetTime;
    private float timeRemainingForTemporaryTarget;
    private float minTimeForTemporaryTarget = 1.0f; // Minimum time to follow the temporary target
    private float maxTimeForTemporaryTarget = 2.0f; // Maximum time to follow the temporary target
    private float closestDistance = 15f;


    bool initialized;

    private void OnEnable()
    {
        StopChasing();
        startSpeed = speed;

        if (initialized)
        {
            if (sr.flipX != startFlipX)
                Flip();
            wanderRoutine = StartCoroutine(WanderAround());
            watchRoutine = StartCoroutine(WatchArea());
        }
    }
    private void OnDestroy()
    {
        if (eh != null)
            eh.onDamaged -= OnDamaged;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        eh = GetComponent<EntityHealth>();

        startFlipX = sr.flipX;

        eh.onDamaged += OnDamaged;

        wanderRoutine = StartCoroutine(WanderAround());
        watchRoutine = StartCoroutine(WatchArea());

        initialized = true;
    }

    private void FixedUpdate()
    {
        if (!chasing)
            return;

        ChaseMovement();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (chasing)
            return;

        if (collision.collider.CompareTag("Player") || collision.collider.CompareTag("Item"))   // item tag kann nur Shield sein wegen layer einstellungen
        {
            TriggerChase();
        }
    }

    private void StartChasing()
    {
        chasing = true;
        timeRemainingForTemporaryTarget = 0.0f;
    }

    private void StopChasing()
    {
        chasing = false;
    }

    private void UpdateTemporaryTargetPosition()
    {
        const float minDistanceToPlayer = 3.0f; // Minimum distance to maintain from the player
        const float maxDistanceToPlayer = 5.0f; // Maximum distance to maintain from the player
        const float minAngleOffset = 70.0f; // Minimum angle offset in degrees from the direction to the player
        const float maxAngleOffset = 110.0f; // Maximum angle offset in degrees from the direction to the player

        // Calculate the direction vector from the enemy to the player
        Vector2 directionToPlayer = PlayerManager.Instance.GetDirectionToPlayer(transform.position);
        directionToPlayer.Normalize();

        // Calculate the angle offset in degrees
        float angleOffset = Random.Range(minAngleOffset, maxAngleOffset);

        // Convert the angle offset to radians
        float angleOffsetRad = angleOffset * Mathf.Deg2Rad;

        // Calculate the perpendicular vector to the directionToPlayer
        Vector2 perpendicularVector = new Vector2(-directionToPlayer.y, directionToPlayer.x);

        // Rotate the perpendicular vector by the random angle offset
        float cosTheta = Mathf.Cos(angleOffsetRad);
        float sinTheta = Mathf.Sin(angleOffsetRad);
        Vector2 rotatedPerpendicular = new Vector2(
            perpendicularVector.x * cosTheta - perpendicularVector.y * sinTheta,
            perpendicularVector.x * sinTheta + perpendicularVector.y * cosTheta
        );

        // Calculate the temporary target position based on the rotated perpendicular vector
        temporaryTargetPosition = (Vector2)PlayerManager.Instance.playerPosition + rotatedPerpendicular * Random.Range(minDistanceToPlayer, maxDistanceToPlayer);

        // Reset the time for the temporary target
        timeRemainingForTemporaryTarget = Random.Range(minTimeForTemporaryTarget, maxTimeForTemporaryTarget);

        Debug.Log($"CurrentPos {transform.position} // TempTargetPos: {temporaryTargetPosition}");
    }


    private void ChaseMovement()
    {


        // If the time for the temporary target is over, continue chasing the player
        Vector2 direction = CombatManager.Instance.GetDirectionToPlayer(transform.position);
        direction.Normalize();

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.identity;

        if (direction.x < 0)
        {
            rot = Quaternion.AngleAxis(angle + 180, Vector3.forward);
            if (sr.flipX)
            {
                sr.flipX = false;
                coneSr.flipX = !sr.flipX;
            }
        }
        else if (direction.x > 0)
        {
            rot = Quaternion.AngleAxis(angle, Vector3.forward);
            if (!sr.flipX)
            {
                sr.flipX = true;
                coneSr.flipX = !sr.flipX;
            }
        }
        rb.SetRotation(rot);

        Vector2 newPosition = GetMovePos();
        if (velocity.magnitude < minVelocity)
        {
            velocity = velocity.normalized * minVelocity;
        }
        rb.MovePosition(newPosition);

        if (Vector2.Distance(rb.position, PlayerManager.Instance.playerPosition) < closestDistance)
        {
            UpdateTemporaryTargetPosition();
        }

    }

    private Vector2 GetMovePos()
    {
        Vector2 newPosition;
        if (timeRemainingForTemporaryTarget > 0)
        {
            newPosition = Vector2.SmoothDamp(rb.position, temporaryTargetPosition, ref velocity, smoothTime, maxVelocity);

            timeRemainingForTemporaryTarget -= Time.deltaTime;
        }
        else
        {

            newPosition = Vector2.SmoothDamp(rb.position, PlayerManager.Instance.playerPosition, ref velocity, smoothTime, maxVelocity);
        }

        return newPosition;
    }


    //private void ChaseMovement() alte chase method
    //{
    //    Vector2 direction = CombatManager.Instance.GetDirectionToPlayer(transform.position);
    //    direction.Normalize();

    //    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    //    Quaternion rot = Quaternion.identity;

    //    if (direction.x < 0)
    //    {
    //        rot = Quaternion.AngleAxis(angle + 180, Vector3.forward);
    //        if (sr.flipX)
    //        {
    //            sr.flipX = false;
    //            coneSr.flipX = !sr.flipX;
    //        }
    //    }
    //    else if (direction.x > 0)
    //    {
    //        rot = Quaternion.AngleAxis(angle, Vector3.forward);
    //        if (!sr.flipX)
    //        {
    //            sr.flipX = true;
    //            coneSr.flipX = !sr.flipX;
    //        }
    //    }
    //    rb.SetRotation(rot);

    //    Vector2 newPosition = Vector2.SmoothDamp(rb.position, PlayerManager.Instance.playerPosition, ref velocity, smoothTime, maxVelocity);
    //    if (velocity.magnitude < minVelocity)
    //    {
    //        velocity = velocity.normalized * minVelocity;
    //    }
    //    rb.MovePosition(newPosition);
    //}

    IEnumerator WanderAround()
    {
        while (!chasing)
        {
            //Debug.Log("WanderAround");
            UpdateWanderDirection();
            float directionChangeTime = Random.Range(directionChangeTimeRange.x, directionChangeTimeRange.y);
            yield return new WaitForSeconds(directionChangeTime);
        }
    }

    IEnumerator WatchArea()
    {
        while (!chasing)
        {
            //Debug.Log("WatchArea");
            AreaScan();
            yield return new WaitForSeconds(areaScanTickRate);
        }
    }

    private void AreaScan()
    {
        RaycastHit2D[] hits = new RaycastHit2D[]
        {
            Physics2D.Raycast(transform.position, transform.rotation * speed.normalized, viewDistance, playerLayer),
            Physics2D.Raycast(transform.position, Quaternion.Euler(0, 0, 15) * (transform.rotation * speed.normalized), viewDistance, playerLayer),
            Physics2D.Raycast(transform.position, Quaternion.Euler(0, 0, -15) * (transform.rotation * speed.normalized), viewDistance, playerLayer),
            Physics2D.Raycast(transform.position, Quaternion.Euler(0, 0, 30) * (transform.rotation * speed.normalized), viewDistance, playerLayer),
            Physics2D.Raycast(transform.position, Quaternion.Euler(0, 0, -30) * (transform.rotation * speed.normalized), viewDistance, playerLayer)
        };

        CheckForPlayer(hits);
    }

    private void CheckForPlayer(RaycastHit2D[] hits)
    {
        foreach (RaycastHit2D hit in hits)
        {
            if (hit)
            {
                if (hit.collider.CompareTag("Player") && !chasing)
                {
                    TriggerChase();
                    return;
                }
            }
        }
    }

    internal void TriggerChase()
    {
        Debug.Log($"{gameObject} starts chasing");
        StartChasing();
        anim.SetTrigger("Chase");
        ResetSpriteRenderer();
        coneSr.enabled = false;
        rb.velocity = Vector2.zero;
        StopRoutines();

        if (TryGetComponent(out EnemyShoot es))
        {
            es.isChasing = true;
        }
    }

    private void UpdateWanderDirection()
    {
        float floor = Mathf.Clamp(headingAngle - maxHeadingChange, 0, 300);
        float ceil = Mathf.Clamp(headingAngle + maxHeadingChange, 0, 300);
        headingAngle = Random.Range(floor, ceil);
        rb.SetRotation(headingAngle);
        if (Random.Range(0, 5) > 3)
        {
            Flip();
        }
        currentRotation = Quaternion.Euler(0, 0, headingAngle);
        SetVelocity();
        //Debug.Log("Update Wander Direction");
    }

    private void SetVelocity()
    {
        rb.angularVelocity = 0f;
        rb.velocity = currentRotation * speed;
    }

    private void Flip()
    {
        //Debug.Log("Flip Shark");
        sr.flipX = !sr.flipX;
        coneSr.flipX = !sr.flipX;
        speed *= -1;
        SetVelocity();
    }

    private void ResetSpriteRenderer()
    {
        sr.flipX = startFlipX;
        coneSr.flipX = !sr.flipX;
        coneSr.enabled = true;
        speed = startSpeed;
    }

    internal void OnDamaged()
    {
        Debug.Log($"{gameObject.name} damaged");
        anim.Play("Damaged");
    }

    private void StopRoutines()
    {
        StopCoroutine(wanderRoutine);
        StopCoroutine(watchRoutine);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3[] directions = new Vector3[]
        {
            transform.rotation * speed.normalized * viewDistance,
            Quaternion.Euler(0, 0, 15) * (transform.rotation * speed.normalized * viewDistance),
            Quaternion.Euler(0, 0, -15) * (transform.rotation * speed.normalized * viewDistance),
            Quaternion.Euler(0, 0, 30) * (transform.rotation * speed.normalized * viewDistance),
            Quaternion.Euler(0, 0, -30) * (transform.rotation * speed.normalized * viewDistance),
        };
        foreach (Vector2 direction in directions)
        {
            Gizmos.DrawRay(transform.position, direction);
        }
    }
}