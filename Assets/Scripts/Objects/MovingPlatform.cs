using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    // Settings
    public Vector2 velocity;
    public bool onlyMoveOnTouch = false;
    public bool returnToStart = false;
    public float maxDistance = 50f;

    public bool greyPlatform;
    public bool redPlatform;
    public bool bluePlatform;
    public bool yellowPlatform;

    // Cache
    bool moving = false;
    bool returning = false;
    float speed = 1f;       // multiplikator fuer velocity
    Vector2 startPosition;

    private void Start()
    {
        startPosition = transform.position;
        if (!onlyMoveOnTouch)
        {
            moving = true;
        }
    }

    private void FixedUpdate()
    {
        if (moving)
        {
            transform.position += (Vector3)(velocity * speed * Time.deltaTime);
            if (Vector2.Distance(transform.position, startPosition) > maxDistance)
            {
                returning = true;
                moving = false;
            }
        }
        else if (returning)
        {
            transform.position += (Vector3)(-velocity * speed * Time.deltaTime);
            if (Vector2.Distance(transform.position, startPosition) < .1f)
            {
                returning = false;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (redPlatform)
            {
                if (PlayerAura.Instance.GetAura() == 1)
                {
                    moving = true;
                    collision.collider.transform.SetParent(transform);
                }
            }
            else if (bluePlatform)
            {
                if (PlayerAura.Instance.GetAura() == 2)
                {
                    moving = true;
                    collision.collider.transform.SetParent(transform);
                }
            }
            else if (greyPlatform)
            {
                moving = true;
                collision.collider.transform.SetParent(transform);
            }
            else if (yellowPlatform)
            {
                if (PlayerAura.Instance.GetAura() == 3)
                {
                    moving = true;
                    collision.collider.transform.SetParent(transform);
                }
            }
        }

        if (collision.gameObject.CompareTag("Nautic") || collision.gameObject.CompareTag("Flame") || collision.gameObject.CompareTag("Aether"))
        {
            collision.collider.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.collider.transform.SetParent(null);
            DontDestroyOnLoad(collision.gameObject);

            if (onlyMoveOnTouch)
            {
                if (moving)
                {
                    moving = false;
                    if (returnToStart)
                    {
                        returning = true;
                    }
                }
            }
        }
    }
}
