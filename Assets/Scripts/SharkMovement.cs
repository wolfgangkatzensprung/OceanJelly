using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(EntityHealth), typeof(Animator), typeof(Rigidbody2D))]
public class SharkMovement : Enemy
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

    bool initialized;
    private float outOfScreenTimer; //descending timer
    private float maxOutOfScreenChaseTime = 10f;
    private bool outOfScreen;

    private void OnEnable()
    {
        chasing = false;
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
    private void OnBecameInvisible()
    {
        outOfScreen = true;
        outOfScreenTimer = maxOutOfScreenChaseTime;
    }

    private void OnBecameVisible()
    {
        outOfScreen = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (chasing)
            return;

        if (collision.collider.CompareTag("Player") || collision.collider.CompareTag("Item"))   // bei Collision mit Player oder Shield (= einziges Item dessen Layer collision erlaubt)
        {
            TriggerChase();
        }
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

    private void Update()
    {
        if (outOfScreen)
        {
            outOfScreenTimer -= Time.deltaTime;
            if (outOfScreenTimer <= 0)
            {
                outOfScreenTimer = 0f;
                gameObject.SetActive(false);
                Ocean_AssetSpawner.Instance.activeEnemies.Remove(gameObject);
                Ocean_AssetSpawner.Instance.inactiveEnemies.Add(gameObject);
            }
        }
    }

    private void FixedUpdate()
    {
        if (!chasing)
            return;

        ChaseMovement();
    }

    #region Idle Behaviour
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
    }
    #endregion Idle Behaviour

    #region Chase Behaviour
    internal void TriggerChase()
    {
        Debug.Log($"{gameObject} starts chasing");
        chasing = true;
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

    private void ChaseMovement()
    {
        Vector2 direction = CombatManager.Instance.GetDirectionToPlayer(transform.position);
        direction.Normalize();

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = GetChaseRotation(direction, angle);
        rb.SetRotation(rotation);

        Vector3 playerPos = PlayerManager.Instance.playerPosition;
        Vector2 newPosition = Vector2.SmoothDamp(rb.position, playerPos, ref velocity, smoothTime, maxVelocity);
        if (velocity.magnitude < minVelocity)   // Velocity Limiter
        {
            velocity = velocity.normalized * minVelocity;
        }
        rb.MovePosition(newPosition);
    }

    private Quaternion GetChaseRotation(Vector2 direction, float angle)
    {
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

        return rot;
    }

    #endregion Chase Behaviour

    private void SetVelocity()
    {
        rb.angularVelocity = 0f;
        rb.velocity = currentRotation * speed;
    }

    private void Flip()
    {
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