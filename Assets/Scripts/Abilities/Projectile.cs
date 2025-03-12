using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject splatParticles;   // Splatter Effect Prefab

    public Rigidbody2D rb;
    public float speed = 10f;

    float xAxis;
    float yAxis;

    int projectileType = 0;     // 0 = Fire // 1 = Water // 2 = Aether // 3 = Envoy

    float lifeTimer = 0;
    float maxLifeTime = 5f;   // nach 5s self destruct

    private void Awake()
    {
        GetProjectileType();
    }

    private void Start()
    {
        xAxis = Input.GetAxis("Horizontal");
        yAxis = Input.GetAxis("Vertical");

        if (xAxis == 0 && yAxis == 0)
        {
            Vector2 fireDirection = PlayerManager.Instance.firePointPosition - PlayerManager.Instance.playerPosition;
            //Debug.Log("FireDirection: " + fireDirection);
            rb.velocity = new Vector2(fireDirection.x, fireDirection.y).normalized * speed;
            return;
        }
        rb.velocity = new Vector2(xAxis, yAxis).normalized * speed; // wird nicht ausgeführt wenn player still steht
    }

    private void Update()
    {
        lifeTimer += Time.deltaTime;
        if (lifeTimer > maxLifeTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Entity"))
        {
            collision.gameObject.GetComponent<EntityHealth>().TakeProjectileDamage(CombatManager.Instance.projectileDamage, transform, speed);
        }
        else
        {
            Collider2D col = Physics2D.OverlapCircle(transform.position, 1f, EnvironmentManager.Instance.whatIsGround);
            if (col != null)
            {
                Debug.Log("Projectile collided with ground.");
                Instantiate(splatParticles, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }

    private void GetProjectileType()
    {
        if (gameObject.name.Contains("Fireball"))
        {
            projectileType = 0;
        }
        else if (gameObject.name.Contains("Waterball"))
        {
            projectileType = 1;
        }
    }
}
