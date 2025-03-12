using UnityEngine;

public class EnemyIceFieldProjectile : MonoBehaviour
{
    Rigidbody2D rb;
    SpriteRenderer sr;

    public float initialScaleIncreaseRate = 0.01f;
    public float thresholdScale = 1f;

    private float scaleIncreaseRate;

    // Movement
    public float speed = 10f;   // how fast
    public float rotationSpeed = 10f;

    // Damage
    public int damage = 1;  // default dmg ist 1

    // Self Destruct
    public float maxLifeTime = 60f;   // nach (default = 60s) self destruct
    float lifeTimer = 0;    // timer geht von 0 bis maxLifeTime

    private bool isScalingDown = false;
    private float shrinkDuration = 1f;
    private float shrinkTimer = 0f;
    private Vector3 initialScale;   // when starting to shrink

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        rb.velocity = (PlayerManager.Instance.playerPosition - transform.position).normalized * speed;
        scaleIncreaseRate = initialScaleIncreaseRate;
        sr.color = new Color(1f, 1f, 1f, 0.1f);
    }

    private void Update()
    {
        Vector3 newScale = transform.localScale;
        newScale.x += scaleIncreaseRate;
        newScale.y += scaleIncreaseRate;
        transform.localScale = newScale;

        if (newScale.x >= thresholdScale && !isScalingDown)
        {
            scaleIncreaseRate -= initialScaleIncreaseRate;
            scaleIncreaseRate = Mathf.Max(scaleIncreaseRate, 0f);
            rotationSpeed += Time.deltaTime;

            // Start shrinking
            isScalingDown = true;
            initialScale = newScale;
        }

        if (isScalingDown)
        {
            shrinkTimer += Time.deltaTime;
            float t = Mathf.Clamp01(shrinkTimer / shrinkDuration);
            transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, t);
            rb.MoveRotation(rb.rotation + rotationSpeed * Time.deltaTime * -2f);

            if (shrinkTimer >= shrinkDuration)
            {
                CombatManager.playerInsideIceField = false;
                Destroy(gameObject);
            }
        }
        else
        {
            rb.MoveRotation(rb.rotation + rotationSpeed * Time.deltaTime);

            float transparency = Mathf.Lerp(0.5f, 1f, Mathf.Min(newScale.x, newScale.y) / thresholdScale);
            sr.color = new Color(1f, 1f, 1f, transparency);

            lifeTimer += Time.deltaTime;
            if (lifeTimer > maxLifeTime)
            {
                // Start shrinking
                isScalingDown = true;
                initialScale = transform.localScale;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log($"IceField trigger hit {col.name}");
        if (col.CompareTag("Player"))
        {
            Debug.Log($"IceField trigger hit Player successful: {col.name}");
            CombatManager.playerInsideIceField = true;
        }
        if (col.CompareTag("Interactable"))
        {
            // Calculate the direction from the current object to the collision point
            Vector2 direction = transform.position - col.transform.position;
            direction.Normalize();

            // Calculate the new velocity by reflecting the current velocity around the collision normal
            Vector2 newVelocity = Vector2.Reflect(rb.velocity, direction);

            // Apply the new velocity to the object's Rigidbody
            rb.velocity = newVelocity;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            CombatManager.playerInsideIceField = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CombatManager.Instance.DealAoEDamageToPlayer(damage);
        }
    }

    private void OnDisable()
    {
        if (CombatManager.Instance != null)
            CombatManager.playerInsideIceField = false;
    }
}
