using UnityEngine;

[RequireComponent(typeof(EntityHealth), typeof(Animator), typeof(Rigidbody2D))]
public class Enemy_DungeonSnapper : Enemy
{
    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator anim;
    EntityHealth eh;

    [Header("Settings")]
    public Vector2 speed = new Vector2(5f, 0f);
    public Vector2 directionChangeTimeRange = new Vector2(1f, 5f);
    public float maxHeadingChange = 30f;
    public float areaScanTickRate = 1f;
    public float viewDistance = 30f;
    public LayerMask playerLayer;
    public float maxVelocity = 30f;
    public float rotationSpeed = 10f;
    public float smoothTime = 0.5f;
    public float minVelocity = 5f;

    private Vector2 velocity;

    [Tooltip("in degrees")]
    public float maxRotation = 45;

    Quaternion currentRotation;
    bool chasing;
    bool startFlipX;    // state of SpriteRenderer Flip variable for x, at Start
    private Vector2 startSpeed;

    bool initialized;

    private void OnEnable()
    {
        chasing = false;
        startSpeed = speed;

        if (initialized)
        {
            if (sr.flipX != startFlipX)
                Flip();
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

        initialized = true;

        TriggerChase();
    }

    private void FixedUpdate()
    {
        if (!chasing)
            return;

        ChaseMovement();
    }

    private void ChaseMovement()
    {
        Vector2 direction = CombatManager.Instance.GetDirectionToPlayer(transform.position);
        direction.Normalize();

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.identity;

        if (direction.x < 0)
        {
            rot = Quaternion.AngleAxis(angle + 180, Vector3.forward);
            if (!sr.flipX)
            {
                sr.flipX = true;
            }
        }
        else if (direction.x > 0)
        {
            rot = Quaternion.AngleAxis(angle, Vector3.forward);
            if (sr.flipX)
            {
                sr.flipX = false;
            }
        }
        rb.SetRotation(rot);

        Vector2 newPosition = Vector2.SmoothDamp(rb.position, PlayerManager.Instance.playerPosition, ref velocity, smoothTime, maxVelocity);
        if (velocity.magnitude < minVelocity)
        {
            velocity = velocity.normalized * minVelocity;
        }
        rb.MovePosition(newPosition);
    }

    private void TriggerChase()
    {
        Debug.Log($"{gameObject} starts chasing");
        chasing = true;
        ResetSpriteRenderer();
        rb.velocity = Vector2.zero;
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
        speed *= -1;
        SetVelocity();
    }

    private void ResetSpriteRenderer()
    {
        sr.flipX = startFlipX;
        speed = startSpeed;
    }

    internal void OnDamaged()
    {
        Debug.Log($"{gameObject.name} damaged");
        anim.Play("Damaged");
    }

    private void OnDestroy()
    {
        eh.onDamaged -= OnDamaged;
    }
}