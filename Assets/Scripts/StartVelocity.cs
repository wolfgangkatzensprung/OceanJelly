using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class StartVelocity : MonoBehaviour
{
    public Rigidbody2D rb;

    public Vector2 speed = new Vector2(40f, 0f);

    [Tooltip("Multiplies speed with playerFacingDirection")]
    public bool moveTowardsPlayer = true;

    private void Start()
    {
        if (moveTowardsPlayer)
        {
            float direction = PlayerManager.Instance.GetFacingDirection();
            rb.velocity = direction * speed;
        }
        else
        {
            rb.velocity = speed;
        }
    }
}
