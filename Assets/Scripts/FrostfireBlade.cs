using UnityEngine;

public class FrostfireBlade : MonoBehaviour
{
    public float movementSpeed = 5f;
    public float rotationSpeed = 100f;
    public float swingDuration = 1f;
    public float dangerRadius = 10f;
    public LayerMask enemyLayer;

    private Transform playerTransform;
    private Rigidbody2D rb;
    private bool isSwinging = false;
    private Transform[] targetEnemies;
    private float swingTimer = 0f;
    public float damage = 3f;

    private void OnEnable()
    {
        gameObject.layer = LayerMask.NameToLayer("Shield");
        PlayerHealth.Instance.onDeath += DestroyThis;
    }
    private void OnDisable()
    {
        PlayerHealth.Instance.onDeath -= DestroyThis;
    }

    private void Start()
    {
        InitializeReferences();

    }
    private void DestroyThis() => Destroy(gameObject);


    private void Update()
    {
        CheckForTargetEnemies();

        if (isSwinging)
        {
            SwingSword();
        }
    }
    private void LateUpdate()
    {
        if (!isSwinging)
        {
            MoveWithPlayer();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isSwinging && collision.gameObject.CompareTag("Entity"))
        {
            AttackEnemy(collision.gameObject);
        }
    }

    private void InitializeReferences()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
    }

    private void CheckForTargetEnemies()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(playerTransform.position, dangerRadius, enemyLayer);

        targetEnemies = new Transform[enemies.Length];
        for (int i = 0; i < enemies.Length; i++)
        {
            targetEnemies[i] = enemies[i].transform;
        }

        if (targetEnemies.Length > 0 && !isSwinging)
        {
            StartSwingAttack();
        }
    }

    private void StartSwingAttack()
    {
        isSwinging = true;
        swingTimer = 0f;
        rb.isKinematic = false;
    }

    private void SwingSword()
    {
        swingTimer += Time.deltaTime;

        if (swingTimer >= swingDuration)
        {
            isSwinging = false;
            rb.isKinematic = true;
            return;
        }

        if (targetEnemies.Length > 0)
        {
            Vector2 direction = (targetEnemies[0].position - transform.position).normalized;
            rb.velocity = direction * movementSpeed;

            float rotationAngle = Mathf.PingPong(Time.time * rotationSpeed, 360f);
            transform.rotation = Quaternion.Euler(0f, 0f, rotationAngle);
        }
    }
    private const float radius = 10f;
    private Vector2 currentVelocity;
    private Vector2 currentDirectionVelocity;
    public float rotationSmoothness = 0.05f;
    public float smoothTime = 0.05f;

    public float transitionTime = 0.5f; // Time threshold in seconds

    private float transitionTimer = 0f;
    private bool isTransitioning = false;

    private void MoveWithPlayer()
    {
        Vector2 direction;
        float distance = Vector2.Distance(transform.position, playerTransform.position);

        if (isTransitioning)
        {
            transitionTimer += Time.deltaTime;

            if (transitionTimer >= transitionTime)
            {
                isTransitioning = false;
                transitionTimer = 0f;
            }
            else
            {
                return;
            }
        }

        if (distance < radius)
        {
            Transform followTrans = PlayerAura.Instance.shieldHolder;
            direction = (followTrans.position + followTrans.up * 10f - transform.position).normalized;

            // Smoothly adjust the velocity towards the desired direction
            Vector2 targetVelocity = direction * movementSpeed;
            rb.velocity = Vector2.SmoothDamp(rb.velocity, targetVelocity, ref currentVelocity, smoothTime);
            //Debug.Log($"Frostfire Blade in Radius with {distance} distance");

            float adjustedRotationSpeed = rotationSpeed * 2f;
            float rotationAngle = Mathf.PingPong(Time.time * adjustedRotationSpeed, 360f);
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, rotationAngle);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothness * Time.deltaTime);
        }
        else
        {
            // Sword is far away, catch up to the player
            direction = (playerTransform.position - transform.position).normalized;

            // Adjust movement speed based on distance from the player
            float adjustedMovementSpeed = movementSpeed;
            float speedMultiplier = Mathf.Clamp(distance / radius, 1f, 3f); // Increase speed multiplier based on distance
            adjustedMovementSpeed *= speedMultiplier;

            // Smoothly adjust the direction towards the desired direction
            direction = Vector2.SmoothDamp(rb.velocity.normalized, direction.normalized, ref currentDirectionVelocity, smoothTime).normalized;

            // Smoothly adjust the velocity towards the desired direction
            Vector2 targetVelocity = direction * adjustedMovementSpeed;
            rb.velocity = Vector2.SmoothDamp(rb.velocity, targetVelocity, ref currentVelocity, smoothTime);

            // Smoothly adjust the rotation towards the desired direction
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothness * Time.deltaTime);

            //Debug.Log($"Frostfire Blade outside the Radius with {distance} distance. Speed: {adjustedMovementSpeed}");
        }
    }

    private void AttackEnemy(GameObject enemy)
    {
        Debug.Log("Attacking enemy: " + enemy.name);
        if (enemy.TryGetComponent(out EntityHealth eh))
        {
            eh.ApplyDamage(damage);
        }
    }
}
