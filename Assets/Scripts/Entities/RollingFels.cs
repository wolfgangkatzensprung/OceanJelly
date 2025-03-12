using UnityEngine;

public class RollingFels : MonoBehaviour
{
    Rigidbody2D rb;
    Transform playerTransform;
    ConstantForce2D cf;
    Animator anim;

    float direction = 1;
    float speed = 0;
    float distance;

    float startTimer = 0;
    bool starterFinished = false;

    float perlinNoise;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        cf = GetComponent<ConstantForce2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (anim.GetBool("isTriggered"))
        {
            if (!starterFinished)
            {
                RollingStarter();
            }
            cf.force = new Vector2(direction * speed, 0);
        }
        distance = playerTransform.position.x - transform.position.x;

        if (distance > 0)
        {
            direction = 1f;
        }
        else if (distance < 0)
        {
            direction = -1f;
        }

        if (starterFinished)
        {
            if (distance < 10f)
            {
                // keine constant force wenn nah am Spieler
                speed = 0;
            }
            if (distance > 10f)
            {
                // constant force speed um aufzuholen
                speed = distance * 0.1f;
                if (speed > 5f)
                {
                    // max constant force speed
                    speed = 5f;
                }
            }
            if (distance < 40)  // am screen rand
            {
                if (rb.velocity.x > 15f)
                {
                    // max velocity
                    perlinNoise = Mathf.PerlinNoise(transform.position.x, transform.position.y);
                    rb.velocity = new Vector2(14.8f + perlinNoise, rb.velocity.y);
                }
            }

        }

    }

    void RollingStarter()
    {
        speed = 5f;
        startTimer += Time.deltaTime;
        if (startTimer > 3f)
        {
            speed = 0f;
            starterFinished = true;
        }
    }
}
