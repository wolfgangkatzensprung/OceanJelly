using UnityEngine;

public class Shield : MonoBehaviour
{
    Rigidbody2D rb;
    Rigidbody2D playerRb;
    Vector2 targetPos;
    float radius = 18f;
    float speed = 2.3f;
    int damage = 3;
    internal float circularOffset = 0f;    // time offset for circle

    private void OnEnable()
    {
        ShieldManager.Instance.AddShield(this);
        gameObject.layer = LayerMask.NameToLayer("Shield");
    }

    private void OnDisable()
    {
#if UNITY_EDITOR
        return;
#endif
        gameObject.layer = LayerMask.NameToLayer("Item");
        ShieldManager.Instance.RemoveShield(this);
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerRb = PlayerManager.Instance.rb;
    }

    void Update()
    {
        targetPos = GetTargetPosition();
        Vector2 targetVelocity = (targetPos - (Vector2)transform.position) * speed;
        rb.transform.position = Vector2.SmoothDamp(transform.position, targetPos, ref targetVelocity, Time.deltaTime);
    }
    void FixedUpdate()
    {
        Quaternion dirRotation = Quaternion.FromToRotation(rb.position, targetPos);
        Quaternion deltaRotation = Quaternion.Euler(targetPos * Time.fixedDeltaTime);
        rb.MoveRotation(dirRotation * deltaRotation);
    }
    private Vector3 GetTargetPosition()
    {
        float x = Mathf.Cos(Time.time * circularOffset * speed) * radius;
        float y = Mathf.Sin(Time.time * circularOffset * speed) * radius;

        return new Vector2(playerRb.position.x + x, playerRb.position.y + y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Entity"))
        {
            if (collision.collider.TryGetComponent(out EntityHealth eh))
            {
                eh.ApplyDamage(damage);
            }
        }
    }
}
