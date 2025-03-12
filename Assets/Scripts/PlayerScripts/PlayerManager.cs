using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager _instance;
    public static PlayerManager Instance { get { return _instance; } }

    [Header("References")]
    public GameObject player;
    public GameObject playerAttack;
    public Transform playerTrans;

    public Rigidbody2D rb;
    public Animator anim;
    public Animator attackAnim;
    public SpriteRenderer sr;
    public CapsuleCollider2D collider;

    public Transform firePoint;
    public Vector3 firePointPosition;

    public Vector3 playerPosition { get { return player.transform.position; } private set { } }
    float playerHeight;

    float xAxis;
    float xAxisRaw;
    float yAxis;
    float yAxisRaw;
    bool isMovingX;     // check ob Player sich horizontal bewegen WILL
    bool isMovingY;     // check ob Player sich vertikal bewegen will
    bool isMoving;      // true wenn axis != 0 ist
    bool isMovingRight; // check ob Player sich tatsaechlich nach rechts bewegt
    bool isMovingLeft;  // etc
    bool isLookingRight;
    bool isLookingLeft;

    float startGravity; // start Gravity wenn eine neue Scale gesetzt wird

    float rememberY;    // Y Positionswert des letzten Frames
    bool falling;

    [Header("Settings")]
    [Tooltip("Raycast fuer GroundPlayer()")]
    public float raycastDistance = 50f;

    public delegate void RespawnDelegate();
    public RespawnDelegate onRespawn;

    private void Awake()
    {
        if (_instance == null) _instance = this;
    }

    private void Start()
    {
        player = GameObject.Find("PLAYER");
        rb = player.GetComponent<Rigidbody2D>();
        anim = player.GetComponentInChildren<Animator>();
        attackAnim = player.GetComponentsInChildren<Animator>()[1];
        sr = player.GetComponentInChildren<SpriteRenderer>();

        rememberY = player.transform.position.y;

        playerHeight = collider.size.y;
        DontDestroyOnLoad(player);

        if (SceneManagerScript.Instance.IsSceneActive("Menu"))
        {
            player.SetActive(false);
        }
    }

    private void Update()
    {
        if (player != null)
            if (player.activeSelf)
            {
                ResetBools();
                firePointPosition = firePoint.position;

                //xAxis = Input.GetAxis("Horizontal");
                //yAxis = Input.GetAxis("Vertical");
                //xAxisRaw = Input.GetAxisRaw("Horizontal");
                //yAxisRaw = Input.GetAxisRaw("Vertical");

                //if (xAxisRaw > 0.001f)
                //{
                //    isMoving = true;
                //    isMovingRight = true;
                //    isMovingX = true;
                //    isLookingRight = true;
                //    isLookingLeft = false;
                //}
                //else if (xAxisRaw < -0.001f)
                //{
                //    isMoving = true;
                //    isMovingLeft = true;
                //    isMovingX = true;
                //    isLookingLeft = true;
                //    isLookingRight = false;
                //}

                //if (yAxisRaw > 0.001f || yAxisRaw < -0.001f)
                //{
                //    isMovingY = true;
                //}

                //if (isMoving)
                //{
                //    //Debug.Log("isMoving");
                //    if (!anim.GetBool("isWalking"))
                //        anim.SetBool("isWalking", true);
                //}
                //else if (anim.GetBool("isWalking"))
                //{
                //    anim.SetBool("isWalking", false);
                //}
                //rememberY = player.transform.position.y;
            }
    }

    internal Vector3 GetDirectionToPlayer(Vector2 otherPos)
    {
        return (Vector2)playerPosition - otherPos;
    }

    private void ResetBools()
    {
        isMovingX = false;
        isMovingY = false;
        isMovingRight = false;
        isMovingLeft = false;
        isMoving = false;
    }

    #region Player Information

    public float GetDistanceToPlayer(Vector2 otherPosition)
    {
        float returnFloat = Vector2.Distance(player.transform.position, otherPosition);
        return returnFloat;
    }

    public bool CheckIfRightFromPlayer(Vector2 otherPosition)
    {
        if (player.transform.position.x < otherPosition.x)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsFalling()
    {
        StartCoroutine(FallCheck());
        return falling;

    }
    IEnumerator FallCheck()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForFixedUpdate();
        if (rememberY > player.transform.position.y)
        {
            falling = true;
        }
        else falling = false;

    }

    public Vector2 GetPlayerVelocity()
    {
        return rb.velocity;
    }


    public float GetFacingDirection()
    {
        if (isLookingLeft)
            return -1;
        else
            return 1;
    }

    public Transform GetPlayerTransform()
    {
        if (player == null)
            player = GameObject.Find("Player");
        return player.transform;
    }
    public Vector2 GetPlayerPos()
    {
        if (player == null)
            player = GameObject.Find("Player");
        return player.transform.position;
    }
    #endregion

    #region Death, Respawn
    // Called by PlayerHealth on Death
    public void Die()
    {
        UIManager.Instance.FadeOut();
        DisablePlayer();
        GameController.Instance.PauseGame(freeze: true);
        PlayerAura.Instance.SetAura(0);

        if (ItemCollector.Instance.GetIsHoldingItem())
        {
            ItemCollector.Instance.DestroyItem();
        }
    }

    public void Respawn()
    {
        playerTrans.position = GetRandomSpawnPos();
        ResetPlayerState();
        EnablePlayer();
        GameController.Instance.Respawn();

        onRespawn?.Invoke();
    }

    private Vector3 GetRandomSpawnPos()
    {
        return Random.insideUnitCircle * Random.Range(-100f, 100f);
    }

    #endregion

    #region Player Mechanics
    private void ResetPlayerState()
    {
        CombatManager.Instance.ResetKnockback();
        PlayerMovement.Instance.inLiquo = false;
    }
    public void StopPlayer()
    {
        Debug.Log("StopPlayer()");
        rb.velocity = Vector2.zero;
    }
    internal void TryGroundPlayer()
    {
        if (PlayerJump.Instance.grounded)
            return;

        GroundPlayer();
    }

    private void GroundPlayer()
    {
        Debug.Log("GroundPlayer()");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, raycastDistance, EnvironmentManager.Instance.whatIsGround);
        if (hit.collider == null)
            return;
        playerTrans.position = new Vector2(hit.point.x, hit.point.y + playerHeight / 2);
    }

    public void LockPlayer()
    {
        Debug.Log("LockPlayer()");
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    public void UnlockPlayer()
    {
        Debug.Log("UnlockPlayer()");
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public void DisableAttack()
    {
        PlayerAttack.Instance.isEnabled = false;
    }
    public void EnableAttack()
    {
        PlayerAttack.Instance.isEnabled = true;
    }

    public void DisableJump()
    {
        PlayerJump.Instance.isEnabled = false;
    }
    public void EnableJump()
    {
        PlayerJump.Instance.isEnabled = true;
    }

    public void DisableMovement()
    {
        Debug.Log("DisableMovement()");
        PlayerMovement.Instance.canMove = false;
    }
    public void EnableMovement()
    {
        Debug.Log("EnableMovement()");
        PlayerMovement.Instance.canMove = true;
    }

    // Disable Player mechanics completely
    public void DisablePlayer()
    {
        DisableMovement();
        //DisableAttack();
        //DisableJump();
    }
    public void DisablePlayerWithoutStop()
    {
        DisableMovement();
        //DisableAttack();
        //DisableJump();
    }

    public void EnablePlayer()
    {
        EnableMovement();
        //EnableAttack();
        //EnableJump();
    }

    public void DisablePlayerCollision()
    {
        rb.isKinematic = true;
        collider.enabled = false;
    }
    public void EnablePlayerCollision()
    {
        rb.isKinematic = false;
        collider.enabled = true;
    }

    #endregion

    #region Getters

    public bool GetIsMovingX()
    {
        return isMovingX;
    }
    public bool GetIsMovingY()
    {
        return isMovingY;
    }
    public bool GetIsMovingRight()
    {
        return isMovingRight;
    }
    public bool GetIsMovingLeft()
    {
        return isMovingLeft;
    }
    public bool GetIsMoving()
    {
        return isMoving;
    }
    public bool GetIsLookingRight()
    {
        return isLookingRight;
    }
    public bool GetIsLookingLeft()
    {
        return isLookingLeft;
    }

    public Animator GetPlayerAnim()
    {
        return anim;
    }

    public GameObject GetPlayer()
    {
        return player;
    }

    internal Rigidbody2D GetRigidbody()
    {
        return rb;
    }
    internal SpriteRenderer GetSr()
    {
        return sr;
    }
}
#endregion