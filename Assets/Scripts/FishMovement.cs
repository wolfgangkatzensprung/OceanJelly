using System.Collections;
using UnityEngine;

public class FishMovement : MonoBehaviour
{
    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator anim;

    public float movementSpeed = 10f;
    public float direction = 1f;

    Vector2 startPos;
    public float maxDistance = 100f;
    bool returningToStart;

    bool rdyForDirectionChange = true;
    float directionChangeCd = .5f;
    float distanceFromStartPos;

    void Start()
    {
        startPos = (Vector2)transform.position;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        distanceFromStartPos = transform.position.x - startPos.x;

        if ((distanceFromStartPos > maxDistance || distanceFromStartPos < -maxDistance) && rdyForDirectionChange)
        {
            StartCoroutine(DirectionChange());
        }
        else if (UnityEngine.Random.value > 0.99f)
        {
            if (UnityEngine.Random.value > .5f && rdyForDirectionChange)
            {
                StartCoroutine(DirectionChange());
            }
            else
                SpeedChange();
        }
    }

    private void ReturnToStart()
    {
        Vector2 returnVector = new Vector2(startPos.x - transform.position.x, 0).normalized * movementSpeed;
        rb.velocity = returnVector;
        StartCoroutine(ReturnToStartCooldown());
    }

    IEnumerator ReturnToStartCooldown()
    {
        returningToStart = true;
        yield return new WaitForSeconds(3f);
        returningToStart = false;
    }

    void FixedUpdate()
    {
        if (returningToStart)
        {
            ReturnToStart();
        }
        else
            Move();
    }

    void Move()
    {
        rb.velocity = new Vector2(direction, 0) * movementSpeed;
    }

    IEnumerator DirectionChange()
    {
        //Debug.Log("DirectionChange at " + Time.time);
        rdyForDirectionChange = false;
        direction *= -1;
        sr.flipX = !sr.flipX;
        yield return new WaitForSeconds(directionChangeCd);
        rdyForDirectionChange = true;
    }

    void SpeedChange()
    {
        float noise = Mathf.PerlinNoise(transform.position.x, transform.position.y);
        movementSpeed = noise * UnityEngine.Random.Range(1, 10);
        anim.speed = Mathf.Clamp(movementSpeed, .1f, 2f);
        //Debug.Log("Fish Movement Speed = " + movementSpeed);
    }
}
