using UnityEngine;

public class ExplosionField : MonoBehaviour
{
    public float lifetime = 3f;
    public int damage = 6;
    float timer;
    public float radius = 11f;
    public float pushStrength = 10f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer > lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            CombatManager.Instance.DealDamageToPlayer(damage);
        }
        else if (collision.collider.TryGetComponent(out EntityHealth eh))
        {
            eh.ApplyDamage(damage);

        }
        if (collision.collider.TryGetComponent(out Rigidbody2D rb))
        {
            ExplosionForce(rb);
        }
    }

    void ExplosionForce(Rigidbody2D rb)
    {
        float x = radius - (rb.position.x - transform.position.x);
        float y = radius - (rb.position.y - transform.position.y);
        if (x > radius)
        {
            x = -radius - (rb.position.x - transform.position.x);
        }

        if (y > radius)
        {
            y = -radius - (rb.position.y - transform.position.y);
        }

        rb.AddForce(new Vector2(x, y) * pushStrength, ForceMode2D.Impulse);
    }
}