using UnityEngine;

public class Rettungsring : MonoBehaviour
{
    Rigidbody2D rb;
    public Vector2 velocity;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(velocity, ForceMode2D.Impulse);
    }

    private void Update()
    {
        if (rb.gravityScale > -5)
            rb.gravityScale -= Time.deltaTime;
    }


}
