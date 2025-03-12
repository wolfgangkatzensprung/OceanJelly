using UnityEngine;

public class EnemyPatrolMovement : MonoBehaviour
{
    public Transform rayPointHorizontal;
    public Transform groundCheck;

    [Tooltip("GroundCheck triggers Flip when there is no collision happening anymore")]
    public bool useGroundCheck = true;

    Rigidbody2D rb;
    Animator anim;

    float spawnTimer = 1f;
    public bool isPatroling = true;
    float raycastTimer = 0f;

    [SerializeField] float speed = 1f;
    [SerializeField] bool waitForTrigger = false;
    bool isChasing = false;
    Vector3 velocity = Vector3.zero;
    float movementSmoothing = 0.1f;

    [SerializeField] bool flipAtStart = true;

    public bool movingRight = true;

    //Raycast
    Vector2 dir = new Vector2(-1, 0);
    [SerializeField] float range = 3f;
    [SerializeField] float groundCheckRange = 3f;
    public float reactionTime = .3f;
    public LayerMask groundLayer;
    public LayerMask enemiesLayer;
    LayerMask combinedLayers;
    Vector2 startPosition;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (flipAtStart)
        {
            Flip();
        }

        combinedLayers = groundLayer | enemiesLayer;
    }

    private void Update()
    {
        if (waitForTrigger)
        {
            if (anim.GetBool("isAttacking"))
            {
                waitForTrigger = false;
            }
            return;
        }
        if (spawnTimer > 0)
        {
            spawnTimer -= Time.deltaTime;
        }
        else
        {
            Debug.DrawRay(rayPointHorizontal.position, dir * range);
            RaycastHit2D hit = Physics2D.Raycast(rayPointHorizontal.position, dir, range, combinedLayers);

            Collider2D groundCheckInFront = Physics2D.OverlapCircle(groundCheck.position, range, combinedLayers);

            if (hit.collider)
            {
                Turn();
            }
            if (useGroundCheck && !groundCheckInFront)
            {
                if (raycastTimer < reactionTime)
                {
                    raycastTimer += Time.deltaTime;
                    return;
                }
                else if (raycastTimer > reactionTime)
                {
                    raycastTimer = 0f;
                }
                Turn();
            }
        }
    }

    void FixedUpdate()
    {
        if (spawnTimer <= 0)
        {
            if (isPatroling)
            {
                if (waitForTrigger)
                {
                    return;
                }
                //Debug.Log("isPatroling");
                Vector3 targetVelocity = new Vector2(speed, rb.velocity.y);
                rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmoothing);
            }
            if (anim.GetBool("isAttacking"))
            {
                waitForTrigger = false;
                Vector3 targetVelocity = new Vector2(speed, rb.velocity.y);
                rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmoothing);
            }
        }
    }

    void Turn()
    {
        Flip();
        speed *= -1;
        dir *= -1;
    }
    public void TurnOnAttacked()
    {
        if (CombatManager.Instance.player.transform.position.x > transform.position.x && speed < 0)     // player ist rechts und entity bewegt sich nach links
        {
            Turn();
        }
        else if (CombatManager.Instance.player.transform.position.x < transform.position.x && speed > 0) // player ist links und entity bewegt sich nach rechts
        {
            Turn();
        }
    }

    void Flip()
    {
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
