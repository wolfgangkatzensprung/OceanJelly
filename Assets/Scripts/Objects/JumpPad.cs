using UnityEngine;

public class JumpPad : MonoBehaviour
{
    bool padActive = false;
    [Tooltip("Y Velocity")]
    public float jumpStrength;

    //Collider2D col;
    Rigidbody2D rb;

    private void Start()
    {
        rb = PlayerManager.Instance.GetRigidbody(); ;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            rb.velocity = new Vector2(0, jumpStrength);
            padActive = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            padActive = false;
        }
    }

}

