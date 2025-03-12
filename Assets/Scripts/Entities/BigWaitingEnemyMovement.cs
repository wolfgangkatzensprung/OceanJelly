using UnityEngine;

public class BigWaitingEnemyMovement : MonoBehaviour
{
    Rigidbody2D rb;
    public Transform rayPoint;
    public Transform rayPoint2;

    public bool isPatroling;
    public bool isWaiting;

    [SerializeField] float speed = 1f;
    Vector3 velocity = Vector3.zero;
    float movementSmoothing = 0.5f;

    public bool movingRight = true;

    //Raycast
    Vector2 dir = new Vector2(1, 0);
    [SerializeField] float range;
    [SerializeField] float range2;
    public LayerMask raycastCanHit;
    Vector2 startPosition;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

    }

    private void Update()
    {
        //Raycast
        Debug.DrawRay(rayPoint.position, dir * range);
        RaycastHit2D hit = Physics2D.Raycast(rayPoint.position, dir, range, raycastCanHit);
        RaycastHit2D hit2 = Physics2D.Raycast(rayPoint2.position, dir, range, raycastCanHit);

        if (hit.collider)
        {
            Flip();
            speed *= -1;
            dir *= -1;
        }
        if (!hit2)
        {
            Flip();
            speed *= -1;
            dir *= -1;
        }
    }

    void FixedUpdate()
    {

        if (!isWaiting) // patroullieren
        {

            Vector3 targetVelocity = new Vector2(speed * 10f, rb.velocity.y);
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmoothing);
        }

    }

    void Flip()
    {
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
