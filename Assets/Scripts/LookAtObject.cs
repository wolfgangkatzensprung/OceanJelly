using System.Collections;
using UnityEngine;

public class LookAtObject : MonoBehaviour
{
    Quaternion startRotation;
    [Header("References")]
    public Transform targetTransform;
    Transform playerTransform;

    [Header("Settings")]

    [Tooltip("Stops looking OnTriggerExit")]
    public bool stopLookingOnExit;

    [Tooltip("Schaut immer Player an")]
    public bool lookAtPlayer = true;

    Vector3 startScale;
    [Tooltip("Ob der Sprite so geflippt werden soll, dass er zu Player schaut")]
    public bool flip = true;

    [Tooltip("Wartet bis Player in Range ist, bevor Target angeschaut wird")]
    public bool waitForPlayer = true;

    public float offset = 0;

    public bool returnToStartRotation = false;
    public float rotationSpeed = 1;
    float startRotationSpeed;

    bool lookAtTarget = false;
    bool facingRight = false;

    // Use this for initialization
    void Start()
    {
        if (lookAtPlayer)
        {
            playerTransform = PlayerManager.Instance.GetPlayerTransform();
            targetTransform = playerTransform;
        }
        startRotationSpeed = rotationSpeed;
        startRotation = Quaternion.identity;
        startScale = transform.localScale;

        if (!waitForPlayer)
        {
            lookAtTarget = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            lookAtTarget = true;
        }
    }
    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Player"))
    //    {
    //            lookAtTarget = true;
    //    }
    //}

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (stopLookingOnExit)
                lookAtTarget = false;
            if (returnToStartRotation)
                transform.rotation = startRotation;
        }
    }

    public void SetLookAtTargetBool(bool b)
    {
        lookAtTarget = b;
    }

    private void FixedUpdate()
    {
        if (targetTransform == null)
        {
            return;
        }

        if (lookAtTarget)
        {
            LockOnTarget();
        }
    }

    void LockOnTarget()
    {
        if (flip)
        {
            Vector3 dir = targetTransform.position - transform.position;
            //Debug.Log("dir: " + dir);
            Quaternion lookRotation = Quaternion.LookRotation(dir, Vector3.up);
            Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed).eulerAngles;
            transform.rotation = Quaternion.Euler(0f, 0f, rotation.z);

            if (targetTransform.position.x >= transform.position.x && !facingRight)
            {
                FlipR();
                facingRight = !facingRight;
                StartCoroutine(flipSpeed());
            }
            else if (targetTransform.position.x <= transform.position.x && facingRight)
            {
                FlipL();
                facingRight = !facingRight;
                StartCoroutine(flipSpeed());
            }
        }
        else
        {
            Vector2 dir = targetTransform.position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + offset;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, startRotationSpeed * Time.deltaTime);
        }
    }

    void FlipR()
    {
        transform.localScale = new Vector3(-startScale.x, startScale.y, startScale.z);
    }

    void FlipL()
    {
        transform.localScale = startScale;
    }

    IEnumerator flipSpeed() // erhöht flip speed temporär damit npc weiterhin auf target lockt
    {
        rotationSpeed = 100;
        yield return new WaitForSeconds(1f);
        rotationSpeed = startRotationSpeed;
    }

    public void LookAtPlayer()
    {
        targetTransform = playerTransform;
    }

    public void StopLooking()
    {
        lookAtTarget = false;
    }
}