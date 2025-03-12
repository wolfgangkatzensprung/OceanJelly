using System.Collections;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;

    public GameObject player;
    public Rigidbody2D rb;

    public PlayerMovement pm;
    public PlayerHealth ph;

    // Knockback
    bool isKnockback;
    float knockbackStrength = 0f;   // fuer Knockback den Player erfaehrt
    Vector2 knockbackDirection = Vector2.zero;
    float knockCdTimer = 0f;
    float knockCooldown = .33f;
    bool isResetting;   // coroutine um zu verhindern dass 2x im selben frame geknockt wird

    //Attack
    public float playerDamage;
    public float rawDamage;
    public float dmgBonusFactor;
    public float bonusDamage;

    //Shooting Projectiles
    public float projectileDamage;

    // Player erleidet Damage
    bool isDamaged = false; // wird dann true
    float damagedTime = 1f; // wie lange VisualManager effekte aktiv sein sollen
    float damagedTimer = 0f;      // multiplikator fuer rotes post processing vfx
    float dmgTimer = 0f;     // timer fuer erlittene aoe damages
    private float dmgTick = .1f;
    private int highestDamage = 0;

    // Status Effects
    bool isStunned = false;
    float stunCD;

    // OverlapCircle Detection von Bombs etc um Spieler herum

    [Tooltip("Radius around Player to check")]
    public float bombDetectionRadius = 5f;
    float detectionTimer = 0f;
    [Tooltip("Time until next Detection Check")]
    public float detectionDelay = .3f;


    // Freeze

    internal static bool playerInsideIceField;
    private float freezeRate = 1f; // Rate at which frozen bar decreases per second
    private float thawRate = 2f; // Rate at which frozen bar increases per second
    private float frozenBar = 0f; // Current value of frozen bar
    internal bool frozen;
    private float frozenTime = 3f;
    public GameObject eiswuerfel;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        player = PlayerManager.Instance.GetPlayer(); ;
        rb = PlayerManager.Instance.GetRigidbody();
    }

    void Update()
    {
        if (!GameController.Instance.IsGamePaused())
        {
            //if (isDamaged)
            //{
            //    damagedTimer -= Time.deltaTime;
            //    if (damagedTimer > 0)
            //    {
            //        //VisualEffectsManager.Instance.DamageVisuals(damagedTimer);
            //    }
            //    else if (damagedTimer < 0)
            //    {
            //        damagedTimer = 1;
            //        isDamaged = false;
            //    }
            //}

            if (isStunned)
            {
                stunCD -= Time.deltaTime;
                if (stunCD < 0.01f)
                {
                    ResetStun();
                }
            }

            detectionTimer += Time.deltaTime;
            if (detectionTimer > detectionDelay)
            {
                BombDetection();
                detectionTimer = 0f;
            }

            //Debug.Log($"PlayerInsideIceField {playerInsideIceField}");

            if (playerInsideIceField && !frozen)
            {
                FreezePlayer();
            }
            else if (!playerInsideIceField && frozenBar > 0)
            {
                UnfreezePlayer();
            }


            if (dmgTimer > 0)
            {
                if (dmgTimer >= dmgTick)
                {
                    DealDamageToPlayer(highestDamage);
                    ResetHighestDamage();
                }
                dmgTimer -= Time.deltaTime;
            }
        }


    }

    void FixedUpdate()
    {
        if (!GameController.Instance.IsGamePaused())
        {
            if (isKnockback)
            {
                TryDoKnockback();
            }
        }
    }

    private void ResetHighestDamage()
    {
        highestDamage = 0;
    }
    public void DealAoEDamageToPlayer(int dmg)
    {
        Debug.Log($"Deal {dmg} AoE Damage to Player");
        highestDamage = Mathf.Max(dmg, highestDamage);
        if (dmgTimer <= 0f)
        {
            dmgTimer = dmgTick;
        }
    }


    public void UnfreezePlayer()
    {
        frozenBar -= thawRate;
        if (frozenBar < 0f)
        {
            frozenBar = 0f;
        }
        Debug.Log($"UnfreezePlayer. FrozenBar: {frozenBar}");
    }

    private void FreezePlayer()
    {
        if (!frozen && frozenBar >= 1f)
        {
            SetFrozen();
        }
        else
        {
            frozenBar += freezeRate * Time.deltaTime;
        }
        Debug.Log($"FreezePlayer. FrozenBar {frozenBar}");

    }

    IEnumerator Eiswuerfel()
    {
        PlayerManager.Instance.DisableMovement();
        eiswuerfel.SetActive(true);

        yield return new WaitForSeconds(frozenTime);

        ResetFrozen();
    }

    public void SetFrozen()
    {
        frozen = true;
        StartCoroutine(Eiswuerfel());
    }
    public void ResetFrozen()
    {
        PlayerManager.Instance.EnableMovement();
        eiswuerfel.SetActive(false);

        frozenBar = 0f;
        frozen = false;
    }

    internal Vector2 GetDirectionToPlayer(Vector3 enemyPosition)
    {
        Vector2 directionToPlayer = player.transform.position - enemyPosition;
        return directionToPlayer;
    }

    internal Vector2 GetPlayerVelocity()
    {
        return rb.velocity;
    }
    private void BombDetection()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, bombDetectionRadius, Vector2.zero);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform.TryGetComponent(out BombAttack ba))
            {
                Debug.Log($"{ba} found at {ba.transform.position} from PlayerPosition {transform.position}");
                ba.StartExplosion();
                Debug.Log("Bumm");
            }
        }
    }

    private void ResetStun()
    {
        Debug.Log("ResetStun()");
        isStunned = false;
        PlayerManager.Instance.EnablePlayer();
        PlayerManager.Instance.EnablePlayerCollision();
        PlayerManager.Instance.UnlockPlayer();
    }

    public void DealDamageToPlayer(int dmgDealt)
    {
        isDamaged = true;
        damagedTimer = damagedTime;
        ph.ReceiveDmg(dmgDealt);
    }

    #region Knockback
    public void SetupKnockback(float knockStrength, Vector3 dmgPosition, float knockCd)
    {
        float dirX;
        float dirY;
        dirX = player.transform.position.x - dmgPosition.x;
        dirY = player.transform.position.y - dmgPosition.y;

        knockbackDirection = (transform.position - dmgPosition).normalized;
        knockbackStrength = knockStrength;
        knockCooldown = knockCd;
        isKnockback = true;
    }
    public void SetupKnockback(Vector2 knockbackDirection, float knockStrength, float knockTime)
    {
        this.knockbackDirection = knockbackDirection;
        knockbackStrength = knockStrength;
    }
    public void TryDoKnockback()
    {
        knockCdTimer += Time.fixedDeltaTime;
        if (knockCdTimer > knockCooldown)
        {
            ResetKnockback();
            return;
        }

        DoKnockback();
    }

    private void DoKnockback()
    {
        PlayerManager.Instance.DisablePlayerWithoutStop();

        if (knockbackDirection.x > 0)
        {
            knockbackDirection.x = 1f;
        }
        else if (knockbackDirection.x < 0)
        {
            knockbackDirection.x = -1f;
        }
        else if (knockbackDirection.x == 0)
        {
            knockbackDirection.x = 1f;
        }

        Vector2 forceDirection = new Vector2(knockbackDirection.x, 0.1f);
        rb.AddForce(forceDirection * knockbackStrength, ForceMode2D.Impulse);
        Debug.Log("DoKnockback() mit Impulse Force: " + knockbackStrength + " und Direction " + forceDirection);
        ResetKnockback();
    }

    public void ResetKnockback()
    {
        Debug.Log("ResetKnockback()");
        if (!isResetting)
        {
            StartCoroutine(ResetKnock());
        }

    }

    IEnumerator ResetKnock()
    {
        //Debug.Log("ResetKnock Coroutine at " + Time.time);
        isResetting = true;
        yield return new WaitForEndOfFrame();
        isKnockback = false;
        knockCdTimer = 0f;
        PlayerManager.Instance.EnablePlayer();

        isResetting = false;
    }

    #endregion


    public void TryStunPlayer(float stunTime)
    {
        if (!isStunned)
        {
            PlayerManager.Instance.LockPlayer();
            StunPlayer(stunTime);
        }
    }

    private void StunPlayer(float stunTime)
    {
        PlayerJump.Instance.CancelJump();
        PlayerManager.Instance.DisablePlayer();
        PlayerManager.Instance.DisablePlayerCollision();
        stunCD = stunTime;
        isStunned = true;
    }

    public void CalculateDamage()
    {
        playerDamage = rawDamage * dmgBonusFactor + bonusDamage;
        Debug.Log("PlayerDamage: " + playerDamage);
    }

    internal void TryKnockbackEnemy(Transform enemy)
    {
        if (enemy.TryGetComponent(out KnockBack kb))
        {
            kb.Knockback(player.transform.position, playerDamage);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, bombDetectionRadius);
    }
}
