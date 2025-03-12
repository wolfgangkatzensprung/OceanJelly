using UnityEngine;

public class SlaveMasterMovement : MonoBehaviour
{
    public GameObject bananaPrefab;

    Rigidbody2D rb;
    Animator anim;

    bool waitForTrigger = true;
    float timer = 10f;
    Vector2 startPosition;
    [SerializeField] float driveDistance;

    [SerializeField] float speed = 10;
    float perlin;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        startPosition = transform.position;
    }

    private void Update()
    {
        if (waitForTrigger)
        {
            if (anim.GetBool("isTriggered"))
            {
                waitForTrigger = false;
            }
            return;
        }

        timer += Time.deltaTime;
        if (timer > 10f)
        {
            anim.SetBool("bananaAttack", false);
            anim.SetBool("drivebyVertical", false);
            anim.SetBool("drivebyHorizontal", false);

            int randomMove = Random.Range(3, 3);
            if (randomMove == 1)
            {
                anim.SetBool("bananaAttack", true);
            }
            else if (randomMove == 2)
            {
                anim.SetBool("drivebyVertical", true);
            }
            else if (randomMove == 3)
            {
                anim.SetBool("drivebyHorizontal", true);
            }
            timer = 0f;
        }
    }

    void FixedUpdate()
    {
        if (waitForTrigger)
        {
            return;
        }

        if (anim.GetBool("isTriggered"))
        {
            perlin = Mathf.PerlinNoise(transform.position.x, transform.position.y);
            if (anim.GetBool("bananaAttack"))
            {
                DoBananaAttack();
                Debug.Log("DoBananaAttack(");
            }
            else if (anim.GetBool("drivebyHorizontal"))
            {
                DoDrivebyHorizontal();
                Debug.Log("DoDrivebyHorizontal()");

            }
            else if (anim.GetBool("drivebyVertical"))
            {
                DoDrivebyVertical();
                Debug.Log("DoDrivebyVertical()");
            }
        }
    }

    void Turn()
    {
        speed *= -1;
        Flip();
    }

    void Flip()
    {
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void DoBananaAttack()
    {
        //Instantiate(bananaPrefab, transform);

    }
    void DoDrivebyHorizontal()
    {
        rb.velocity = Vector2.right * speed;
        if (Vector2.Distance(transform.position, startPosition) > driveDistance)
        {
            rb.velocity = Vector2.up * speed;
        }
    }
    void DoDrivebyVertical()
    {
        rb.velocity = Vector2.up * speed;
        if (Vector2.Distance(transform.position, startPosition) > driveDistance)
        {
            rb.velocity = Vector2.right * speed;
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Turn();
        }
    }
}
