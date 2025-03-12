using UnityEngine;

public class EnemyFly : MonoBehaviour
{
    Rigidbody2D rb;
    float flyingTimer = 0f;
    float stayTime = 5f;    // stay on velocity for 5 seconds until random change occurs
    public Vector2 startVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = startVelocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Ping oder Pong je nachdem");
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            rb.velocity = -rb.velocity;
        }
    }

    void Update()
    {
        flyingTimer += Time.deltaTime;
        if (flyingTimer > stayTime)
        {
            int randomAddX = Random.Range(-5, 5);
            int randomAddY = Random.Range(-5, 5);
            rb.velocity = new Vector2(rb.velocity.x + randomAddX, rb.velocity.y + randomAddY);
            flyingTimer = 0f;
        }
    }
}
