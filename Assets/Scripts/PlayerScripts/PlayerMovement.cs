using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance;

    public PlayerDash _dash;

    Rigidbody2D rb;
    Animator anim;

    public float runSpeed = 100f;
    internal float speedBonus = 0f;
    float startSpeed;

    float direction;
    float horizontalMove = 0f;
    public bool canMove = true;             // normales Movement

    public bool _isHooked = false;          // ob Player am Hook hängt
    Vector2 hookedForce;                    // AddForce Movement am Hook
    float hookedJumpForce = 1200f;                  // AddForce Jump am Hook
    internal bool inLiquo;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        AbilityManager.Instance.onAbilityIncrease += SetSpeedBonus;
        PlayerInputManager.Instance.onTap += MovePlayer;
        SwipeHandler.Instance.onSwiped += AttackBySwipe;
        PlayerHealth.Instance.onDeath += ResetSpeed;
    }
    private void OnDisable()
    {
#if UNITY_EDITOR
        return;
#endif
        AbilityManager.Instance.onAbilityIncrease -= SetSpeedBonus;
        PlayerInputManager.Instance.onTap -= MovePlayer;
        SwipeHandler.Instance.onSwiped -= AttackBySwipe;
        PlayerHealth.Instance.onDeath -= ResetSpeed;
    }

    private void Start()
    {
        _dash = GetComponent<PlayerDash>();    // Dash Script von Player
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        startSpeed = runSpeed;
    }

    public void MovePlayer(Vector2 pos)
    {
        if (!canMove)
            return;

        Debug.Log("Player Tap Swipe Move");
        Vector3 position = Camera.main.ScreenToWorldPoint(pos);
        position.z = 0f;
        Vector3 dir = position - PlayerManager.Instance.playerPosition;

        Debug.Log($"MovePlayer in direction: {dir.ToString()}");

        float tolerance = 0.1f;
        if (Mathf.Abs(dir.x) < tolerance)   // rotation bug fix fuer dir = unten
        {
            dir = Vector3.down;
            transform.rotation = Quaternion.Euler(0f, 0f, 180f);
        }
        else
        {
            transform.up = dir.normalized;
        }

        rb.AddForce(dir.normalized * (runSpeed + speedBonus), ForceMode2D.Impulse);

        anim.Play("Swim");
    }

    private void AttackBySwipe(Vector2 dir)
    {
        Debug.Log($"Attack in direction {dir.ToString()}");
    }

    private void SetSpeedBonus(AbilityManager.Abilities ability)
    {
        if (ability.Equals(AbilityManager.Abilities.Speed))
            speedBonus = 10f * AbilityManager.Instance.abilityLevels[ability];
    }

    private void ResetSpeed()
    {
        runSpeed = startSpeed;
        speedBonus = 0f;
    }

    //void Update()
    //{
    //    if (!isEnabled)
    //    {
    //        return;
    //    }
    //if (!PlayerJump.Instance.anim.GetBool("isJumpingWall"))
    //{
    //    if (Input.GetAxisRaw("Horizontal") > .3f)
    //        horizontalMove = Mathf.Ceil(Input.GetAxisRaw("Horizontal")) * runSpeed;
    //    else if (Input.GetAxisRaw("Horizontal") < -.3f)
    //        horizontalMove = Mathf.Floor(Input.GetAxisRaw("Horizontal")) * runSpeed;
    //    else
    //        horizontalMove = 0;
    //}
    //}

    //void FixedUpdate()
    //{
    //    if (!isEnabled)
    //    {
    //        return;
    //    }
    //    if (_isHooked)
    //    {
    //        hookedForce = new Vector2(horizontalMove, 0f);
    //    }

    //    if (canMove)
    //    {
    //        controller.Move(horizontalMove * Time.fixedDeltaTime);    // Player Movement
    //    }

    //    else if (!canMove)               // wenn Player sich nicht normal bewegen kann
    //    {
    //        if (_isHooked)      // wenn Player am Hook hängt
    //        {
    //            rb.AddForce(hookedForce);
    //        }
    //        else if (!_isHooked)
    //        {
    //            canMove = true;
    //            Debug.Log("Can move again");
    //        }
    //    }
    //}

    public void SetSleep()
    {
        canMove = false;
    }
    public void WakeUp()
    {
        canMove = true;
    }
}