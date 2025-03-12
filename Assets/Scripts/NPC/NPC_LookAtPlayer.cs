using System.Collections;
using UnityEngine;

public class NPC_LookAtPlayer : MonoBehaviour
{
    Quaternion startRotation;
    Transform targetTransform;
    Transform playerTransform;

    public GameObject objectToFocus;    // wenn SetObjectToFocus gecallt wird, fokussiert der NPC nur noch dieses Object
    [SerializeField] float turnSpeed = 1;
    [SerializeField] bool flip = true;
    [SerializeField] float offset = 0;
    public bool returnToStartRotation = false;
    public bool krakenMode;     // muss true sein auf kreiselkraken

    float startTurnSpeed;
    bool lookAtTarget = false;
    bool facingRight = false;

    // Use this for initialization
    void Start()
    {
        playerTransform = PlayerManager.Instance.GetPlayerTransform();
        targetTransform = playerTransform;
        startTurnSpeed = turnSpeed;
        startRotation = Quaternion.identity;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (krakenMode)
            return;
        if (collision.CompareTag("Player"))
        {
            lookAtTarget = true;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (krakenMode)
            return;
        if (collision.CompareTag("Player"))
        {
            if (!lookAtTarget)
                lookAtTarget = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (krakenMode)
            return;
        if (collision.CompareTag("Player"))
        {
            //lookAtTarget = false;
            if (returnToStartRotation)
                transform.rotation = startRotation;
        }
    }

    public void SetLookAtTarget(bool b)
    {
        lookAtTarget = b;
    }

    private void FixedUpdate()
    {
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
            Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
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
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, startTurnSpeed * Time.deltaTime);
        }
    }

    void FlipR()
    {
        transform.localScale = new Vector3(-1f, 1f, 1f);
    }

    void FlipL()
    {
        transform.localScale = new Vector3(1f, 1f, 1f);
    }

    IEnumerator flipSpeed() // erhöht flip speed temporär damit npc weiterhin auf target lockt
    {
        turnSpeed = 100;
        yield return new WaitForSeconds(1f);
        turnSpeed = startTurnSpeed;
    }

    public void SetObjectToFocus(GameObject go)
    {
        objectToFocus = go;
        // focus it
    }
}