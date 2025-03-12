using System.Collections;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    public static PlayerJump _instance;
    public static PlayerJump Instance { get { return _instance; } }

    public Rigidbody2D rb;
    public Animator anim;

    internal bool isEnabled = true;

    [Header("References")]
    public Transform topCheck;
    public Transform groundCheck;
    public Transform wallCheck;
    Collider2D wallCollider;

    [Header("Settings")]
    public LayerMask groundLayer;
    public float groundCheckRadius = 2;
    public float stillGroundedTime = .32f;

    internal bool grounded { get; private set; }
    internal bool touchingWall;
    float wasGroundedTimer = 0f;     // wenn man grounded war geht der timer los; solange er nicht stillGroundedTime ueberschreitet kann man noch jumpen

    // Jump
    bool jumping;
    public float jumpSpeed = 50f;
    public float maxFallSpeed = 100f;
    public float fallForce = 100f;
    float jumpSpeedMultiplier = 1f;
    public float jumpTime = 1f;
    float jumpTimer = 0f;   // laeuft hoch solange bool jump aktiv ist
    public float jumpTimeCounterNeg = 0f;  // von jumpTime nach 0
    float pressedJumpTimerNeg = 0f; // laeuft von preJumpTime bis 0 wenn man jump gedrueckt hat. Wenn es 0 unterschreitet und man dann ground beruehrt, jumpt man nicht mehr automatisch
    public float preJumpTime = .24f;
    bool jumpCd;    // bool das true ist solange Player losjumpt. Dient dazu um zu vermeiden, dass Player mit 2x schnell klicken mittels wasGroundedTimer-AfterJump doppelt jumpen kann

    // Extra Jumps
    public int extraJumpAmount = 0;
    int extraJumps;

    // Liquo Jump
    public float liquoJumpForce = 1000f;

    // Wall Jump
    [SerializeField] int wallHopForce = 10; // Impulse Force
    private Vector2 wallHopDirection;
    Coroutine walljumpRoutine;

    // Hook Jump
    private bool jumpingHook;
    public float hookJumpForce = 1000f;

    // Gravity and Falling
    float startGravity;
    bool falling;   // wird durch FallDown() true gesetzt um die fallmechanik loszutreten
    Coroutine fallRoutine;
    float velocityY = 0;
    float fallingMultiplicator = 1f; // wird multipliziert mit falling force
    float wallSlideDrag = 33f;
    bool pauseFalling;  // solange es true ist, wird keine FallingForce ausgeuebt

    // Liquo Gravity
    public float liquoTime = 1f; // die Zeit die die Liquo Gravity benoetigt um 0 zu werden
    float liquoTimer;       // Timer laeuft von liquoTime gegen 0 ; default ist von -1 nach 0
    bool isPlatsching = false;  // wenn Player ins Wasser eintritt, wird er einen kurzen Moment einplatschen. In diesem kurzen Moment wird Gravity mehrmals mit 0.8 oder so multipliziert
    float _liquoFactor;  // gravity factor fuer liquo

    private void Awake()
    {
        if (_instance == null) _instance = this;
    }

    private void Start()
    {
        wallCollider = wallCheck.GetComponent<Collider2D>();
        extraJumpAmount = PlayerPrefs.GetInt("ExtraJumps");
        extraJumps = extraJumpAmount;
        startGravity = rb.gravityScale;
        liquoTimer = liquoTime;
        pressedJumpTimerNeg = preJumpTime;

        FallDown();
    }

    private void Update()
    {
        if (!isEnabled)
        {
            return;
        }

        GroundCheck();
        WallCheck();
        LiquoCheck();
        FallingCheck();

        ManageInputs();
        SetJumpTimer();
    }

    private void FixedUpdate()
    {
        if (!isEnabled)
        {
            return;
        }

        HandleJumping();

        if (falling)
        {
            if (rb.velocity.y > -maxFallSpeed)
            {
                ApplyFallingForce();
            }
        }
    }

    private void HandleJumping()
    {
        if (anim.GetBool("isJumping") && !anim.GetBool("isJumpingWall"))
        {
            DoJump();
        }
        else if (anim.GetBool("isJumpingLiquo"))
        {
            LiquoJump();
        }
        else if (anim.GetBool("isJumpingExtra"))
        {
            ExtraJump();
        }
        else if (anim.GetBool("isJumpingWall"))
        {
            walljumpRoutine = StartCoroutine(WallJump());
        }
        else if (anim.GetBool("isJumpingHook") && !jumpingHook)
        {
            StartCoroutine(HookJump());
        }
    }

    private void ManageInputs()
    {
        // Press Jump
        //if (Input.GetButtonDown("Jump"))
        //{
        //    SetupJump();
        //}

        //// Hold Jump
        //if (Input.GetButton("Jump"))
        //{
        //    pressedJumpTimerNeg -= Time.deltaTime;

        //    // Wenn man jump drueckt kurz bevor man grounded ist
        //    if (grounded && pressedJumpTimerNeg > 0)
        //    {
        //        Debug.Log("PreTouchJump");
        //        if (!anim.GetBool("isJumping"))
        //        {
        //            anim.SetBool("isJumping", true);
        //            anim.Play("Jumping");
        //        }
        //        pressedJumpTimerNeg = 0f;
        //    }
        //}

        // Release Jump
        //else if (Input.GetButtonUp("Jump"))
        //{
        //    if (anim.GetBool("isJumping"))
        //    {
        //        Debug.Log("Release Jump");
        //        anim.SetBool("isJumping", false);
        //        jumpTimeCounterNeg = 0;

        //    }
        //    pressedJumpTimerNeg = preJumpTime;
        //    if (!falling)
        //    {
        //        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * .5f);
        //        FallDown();
        //    }
        //    jumping = false;
        //}
    }
    internal void CancelJump()
    {
        anim.SetBool("isJumping", false);
    }

    // Timer
    void SetJumpTimer()
    {
        if (jumping)
        {
            jumpTimer += Time.deltaTime;
            //Debug.Log("Set JumpTimer to " + jumpTimer);
        }
        else if (jumpTimer != 0)
        {
            //Debug.Log("Not jumping anymore. JumpTimer set to 0");
            jumpTimer = 0;
        }
    }


    #region Checks
    private void FallingCheck()
    {
        if (jumping && !falling && !grounded)
        {
            if (pressedJumpTimerNeg < (0 - jumpTime))
            {
                //Debug.Log("Laenger Jump gedrueckt als jumpTime");
                FallDown();
            }
            if (jumpTimer > jumpTime)
            {
                //Debug.Log("Laenger im Jump als jumpTime");
                FallDown();
            }
        }
        else if (!falling && !grounded)
        {
            //Debug.Log("Nicht Falling aber auch nicht Grounded");
            FallDown();
        }
    }

    private void LiquoCheck()
    {
        if (!PlayerMovement.Instance._isHooked && !anim.GetBool("isWallSliding"))
        {
            if (PlayerMovement.Instance.inLiquo)
            {
                //Debug.Log("Applying Liquo Gravity");
                if (liquoTime < 0)
                {
                    liquoTimer += Time.deltaTime;
                }
                SetLiquoScale(_liquoFactor * liquoTimer * liquoTimer);
            }
        }
    }

    private void WallCheck()
    {
        if (wallCollider.IsTouchingLayers(groundLayer))
        {
            if (!grounded && PlayerManager.Instance.GetIsMovingX() && !PlayerMovement.Instance.inLiquo && rb.velocity.y < 0)
            {
                {
                    Debug.Log("isWallSliding");
                    anim.SetBool("isWallSliding", true);
                    SetGravityScale(.5f);
                    rb.drag = wallSlideDrag;
                }
            }
            else if (anim.GetBool("isWallSliding")) // reset
            {
                anim.SetBool("isWallSliding", false);
                SetGravityScale(startGravity);
                rb.drag = 0;
            }
        }
        else if (anim.GetBool("isWallSliding")) //reset
        {
            anim.SetBool("isWallSliding", false);
            SetGravityScale(startGravity);
            rb.drag = 0;
        }
    }

    private void GroundCheck()
    {
        grounded = false;
        Collider2D[] collidersGround = Physics2D.OverlapCapsuleAll(groundCheck.position, new Vector2(groundCheckRadius, 1), CapsuleDirection2D.Horizontal, 0, groundLayer);
        for (int i = 0; i < collidersGround.Length; i++)
        {
            if (collidersGround[i].gameObject != gameObject)
            {
                SetGrounded();
            }
        }

        TopGroundCheck();

        if (!grounded)
        {
            wasGroundedTimer -= Time.deltaTime;
        }
    }

    private void TopGroundCheck()
    {
        Collider2D[] collidersTop = Physics2D.OverlapCapsuleAll(topCheck.position, new Vector2(groundCheckRadius, 1), CapsuleDirection2D.Horizontal, 0, groundLayer);
        for (int i = 0; i < collidersTop.Length; i++)
        {
            if (collidersTop[i].gameObject != gameObject)
            {
                FallDown();
            }
        }
    }

    private void SetGrounded()
    {
        grounded = true;
        falling = false;
        extraJumps = extraJumpAmount;
        wasGroundedTimer = stillGroundedTime;
        jumpTimeCounterNeg = 0f;

        /*
        if (!anim.GetBool("isJumping"))
        {
            Debug.Log("PlayerAnimator is not set to isJumping, so SetGrounded sets bool jumping to false");
            jumping = false;
        }
        */
        if (!anim.GetBool("isGrounded"))
        {
            anim.SetBool("isGrounded", true);
        }
        if (anim.GetBool("isFalling"))
        {
            anim.SetBool("isFalling", false);
        }
    }

    #endregion

    // Setting up the jump type and animator
    void SetupJump()
    {
        Debug.Log("SetupJump");
        if (PlayerMovement.Instance.inLiquo)
        {
            Debug.Log("Set isJumpingLiquo true");
            anim.SetBool("isJumpingLiquo", true);
        }
        else
        {
            if (PlayerMovement.Instance._isHooked)
            {
                anim.SetBool("isJumpingHook", true);
            }
            else
            {
                if (anim.GetBool("isWallSliding"))
                {
                    Debug.Log("isWallSliding animator bool true");
                    if (PlayerPrefs.GetInt("WallJump") == 1)
                    {
                        anim.SetBool("isJumpingWall", true);
                    }
                }
                else if (grounded || wasGroundedTimer > 0 && !jumpCd)
                {
                    Debug.Log("Jump");
                    anim.SetBool("isJumping", true);
                    anim.Play("Jumping");
                    jumpTimeCounterNeg = jumpTime;
                }
                else if (extraJumps > 0)
                {
                    if (!anim.GetBool("isWallSliding"))
                    {
                        anim.SetBool("isJumpingExtra", true);
                        extraJumps -= 1;
                    }
                }
            }
        }
    }

    #region Jumps

    void DoJump()          // normaler Jump
    {
        //Debug.Log("DoJump() at " + Time.time);
        jumpCd = true;
        anim.SetBool("isGrounded", false);
        jumping = true;
        jumpTimeCounterNeg -= Time.fixedDeltaTime;
        if (jumpTimeCounterNeg <= 0)
        {
            jumpTimeCounterNeg = 0;
            anim.SetBool("isJumping", false);
        }
        Vector2 velocityVector = new Vector2(rb.velocity.x, jumpSpeed * jumpSpeedMultiplier);
        if (!Mathf.Round(rb.velocity.y).Equals(Mathf.Round(velocityVector.y)))  // nur velocity aendern wenn es was bringt
            rb.velocity = velocityVector;
        StartCoroutine(JumpCooldown());
    }
    IEnumerator JumpCooldown()
    {
        yield return new WaitForSeconds(stillGroundedTime); // so lange warten bis Player nicht mehr stillGrounded abusen kann
        //yield return new WaitForFixedUpdate();
        jumpCd = false;
    }


    public void HitJump()   // Spike Jump etc
    {
        Debug.Log("HitJump() at " + Time.time);
        anim.SetBool("isGrounded", false);
        jumping = true;
        jumpTimeCounterNeg -= Time.fixedDeltaTime;
        if (jumpTimeCounterNeg <= 0)
        {
            jumpTimeCounterNeg = 0;
            anim.SetBool("isJumping", false);
        }
        Vector2 velocityVector = new Vector2(rb.velocity.x, jumpSpeed * jumpSpeedMultiplier);
        ParticleManager.Instance.SpawnParticles("HitJump", transform.position, Quaternion.identity);
        if (!Mathf.Round(rb.velocity.y).Equals(Mathf.Round(velocityVector.y)))  // nur velocity aendern wenn es was bringt
            rb.velocity = velocityVector;

        jumping = false;
    }

    void ExtraJump()       // Double / Triple / Quadruple Jump etc
    {
        Debug.Log("ExtraJump");
        //jumpTimeCounterNeg -= Time.fixedDeltaTime;
        //rb.velocity = new Vector2(rb.velocity.x, jumpSpeed * jumpSpeedMultiplier);
        //ParticleManager.Instance.SpawnParticles("ExtraJump", transform.position, Quaternion.identity);
        //if (Input.GetButtonUp("Jump") || jumpTimeCounterNeg <= 0)
        //{
        //    anim.SetBool("isJumpingExtra", false);
        //}
    }

    IEnumerator WallJump()        // Wall Hop
    {
        Debug.Log("WallJump");
        if (PlayerManager.Instance.GetIsLookingLeft())
        {
            wallHopDirection = new Vector2(1, 1);
        }
        else if (PlayerManager.Instance.GetIsLookingRight())
        {
            wallHopDirection = new Vector2(-1, 1);
        }
        pauseFalling = true;
        PlayerManager.Instance.DisableMovement();
        Vector2 forceToAdd = new Vector2(wallHopDirection.x, wallHopDirection.y) * wallHopForce;
        rb.AddForce(forceToAdd);
        yield return new WaitForFixedUpdate();
        rb.AddForce(forceToAdd * 1.1f);
        yield return new WaitForFixedUpdate();
        rb.AddForce(forceToAdd * 1.3f);
        yield return new WaitForFixedUpdate();
        rb.AddForce(forceToAdd * 1.6f);
        yield return new WaitForFixedUpdate();
        rb.AddForce(forceToAdd * 2f);
        anim.SetBool("isJumpingWall", false);
        PlayerManager.Instance.EnableMovement();
        pauseFalling = false;
    }

    IEnumerator HookJump()
    {
        jumpingHook = true;
        Debug.Log("HookJump");
        jumpTimeCounterNeg = jumpTime;
        float hookForceX = 0f;
        if (PlayerManager.Instance.GetIsLookingLeft())
        {
            hookForceX = -140f;
        }
        if (PlayerManager.Instance.GetIsLookingRight())
        {
            hookForceX = 140f;
        }
        pauseFalling = true;
        Vector2 addF = new Vector2(hookForceX, hookJumpForce);
        yield return new WaitForFixedUpdate();
        rb.AddForce(addF);
        yield return new WaitForFixedUpdate();
        rb.AddForce(addF);
        yield return new WaitForFixedUpdate();
        rb.AddForce(addF);
        yield return new WaitForFixedUpdate();
        rb.AddForce(addF);
        yield return new WaitForFixedUpdate();
        rb.AddForce(addF);
        anim.SetBool("isJumpingHook", false);
        pauseFalling = false;
        jumpingHook = false;
    }

    #endregion

    #region Falling

    void FallDown()
    {
        //Debug.Log("FallDown");
        if (!anim.GetBool("isFalling"))
            anim.SetBool(("isFalling"), true);
        falling = true;
        jumping = false;
    }

    void ApplyFallingForce()
    {
        if (pauseFalling)
        {
            return;
        }
        fallingMultiplicator += (2 - fallingMultiplicator) + .3f;
        rb.AddForce(Vector2.down * fallForce * fallingMultiplicator);
    }

    public void SetGravityScale(float theGravityScale)
    {
        //Debug.Log("Setting GravityScale to " + theGravityScale);
        rb.gravityScale = theGravityScale;
    }
    #endregion

    #region Liquo
    public void SetLiquoScale(float liquoScale)
    {
        if (liquoScale != rb.gravityScale)
        {
            Debug.Log("Setting LiquoScale to " + startGravity * liquoScale);
            rb.gravityScale = startGravity * liquoScale;
        }
    }

    public void SetLiquoFactor(float liquoFactor)
    {
        Debug.Log("_LiquoFactor set to: " + liquoFactor);
        this._liquoFactor = liquoFactor;
    }

    void LiquoJump()       // additiver bzw kumulativer Wasser Jump
    {
        rb.AddForce(Vector2.up * liquoJumpForce);
        anim.SetBool("isJumpingLiquo", false);
    }

    public void Platsch(float plFactor, float minPlVel)
    {
        StartCoroutine(Platscher(plFactor, minPlVel));
    }
    public IEnumerator Platscher(float platschFactor, float minPlatschVel)
    {
        Debug.Log("Platsch");
        rb.velocity *= platschFactor;
        yield return new WaitForSeconds(.1f);
        yield return new WaitForFixedUpdate();
        {
            if (PlayerJump.Instance.rb.velocity.y < minPlatschVel)
            {
                Platsch(platschFactor, minPlatschVel);
            }
        }
    }

    #endregion
}
