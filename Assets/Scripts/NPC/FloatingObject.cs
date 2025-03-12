using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    Rigidbody2D rb;
    Collider2D triggerCol;

    Vector2 startPosition;
    Vector2 direction = new Vector2(0, 1);

    public float speed = 5f;
    public float maxDistance;

    [Tooltip("Collider wird komplementaer bewegt sodass er quasi stillsteht")]
    public bool compensateColliderMovement;
    Vector2 colliderOffset;

    private void Start()
    {
        startPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
        triggerCol = GetComponent<Collider2D>();
    }

    private void Update()
    {
        rb.velocity = new Vector2(0, speed) * direction;
        if (transform.position.y > startPosition.y + maxDistance)
        {
            direction = new Vector2(0, -1);
        }
        else if (transform.position.y < startPosition.y - maxDistance)
        {
            direction = new Vector2(0, 1);
        }

        if (compensateColliderMovement)
        {
            colliderOffset = startPosition - (Vector2)transform.position;
            triggerCol.offset = colliderOffset;
        }
    }
}
