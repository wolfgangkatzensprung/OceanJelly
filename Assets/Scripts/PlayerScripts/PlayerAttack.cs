using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public static PlayerAttack _instance;
    public static PlayerAttack Instance { get { return _instance; } }

    public Animator attackAnim;
    public Animator playerAnim;

    Rigidbody2D rb;
    public PlayerAura playerAura;

    [Header("Attack Positions")]
    public Transform attackPositionLR;
    public Transform attackPositionOben;
    public Transform attackPositionUnten;

    [Header("Attack Sprite Renderer")]
    public SpriteRenderer attackSr;

    [Header("Layers")]
    public LayerMask whatIsEnemies;
    public LayerMask whatIsDestructible;
    public LayerMask whatIsSpikes;
    public LayerMask whatIsGround;

    [Header("Settings")]
    public bool isEnabled = true;
    bool jumpHit = false;

    [SerializeField] float attackTimer;
    [SerializeField] float attackDuration;
    [SerializeField] float attackDelay;
    public float attackRange;

    public float heavyAttackFactor = 1.5f;

    public enum AttackType
    {
        AttackOben, AttackFrontal, AttackUnten
    }

    private void Awake()
    {
        if (_instance == null) _instance = this;
    }
    void Start()
    {
        rb = PlayerManager.Instance.GetRigidbody();
    }

    public void TryDoAttack(AttackType atkType)
    {
        Debug.Log("TryDoAttack(" + atkType.ToString() + ")");
        if (!isEnabled)
        {
            Debug.Log("Attack not enabled. Returning.");
            return;
        }
        if (attackTimer <= 0)   // nur angreifen wenn cooldown up ist
        {
            DoAttack(atkType);
        }
    }

    private void Update()
    {
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (jumpHit)
        {
            rb.velocity = Vector2.zero;
            PlayerJump.Instance.HitJump();
            jumpHit = false;
        }
    }

    void DoAttack(AttackType attackTyp)
    {
        Debug.Log("DoAttack(" + attackTyp.ToString() + ")");
        SoundManager.Instance.PlaySound("PlayerAttack");
        StartAttackCooldownTimer();

        if (playerAnim != null)
        {
            switch (attackTyp)
            {
                case AttackType.AttackFrontal:
                    //Debug.Log("AttackFrontal");
                    playerAnim.Play("AttackFrontal");
                    attackAnim.Play("AttackFrontal");
                    break;
                case AttackType.AttackOben:
                    //Debug.Log("AttackOben");
                    playerAnim.Play("AttackOben");
                    attackAnim.Play("AttackOben");
                    break;
                case AttackType.AttackUnten:
                    //Debug.Log("AttackUnten");
                    playerAnim.Play("AttackUnten");
                    attackAnim.Play("AttackUnten");
                    break;
            }
        }

        HitEnemies(attackTyp);
        HitDestructibles(attackTyp);
        HitSpikes(attackTyp);
        HitGround(attackTyp);
    }

    private void HitGround(AttackType attackTyp)
    {
        Vector3 attackPosition = GetAttackTransform(attackTyp).position;
        Collider2D[] targetableGround = Physics2D.OverlapCircleAll(attackPosition, attackRange, whatIsGround);
        for (int i = 0; i < targetableGround.Length; i++)
        {
            Debug.Log("Ground Hit");
            if (targetableGround[i].CompareTag("Destructible"))
            {
                DirectorScript.Instance.Screenshake();
                TimeManager.Instance.HitFreeze(0.04f);
            }
        }
    }

    private void HitSpikes(AttackType attackTyp)
    {
        if (attackTyp == AttackType.AttackUnten)
        {
            Collider2D[] spikesToJumpFrom = Physics2D.OverlapCircleAll(attackPositionUnten.position, attackRange, whatIsSpikes);
            for (int i = 0; i < spikesToJumpFrom.Length; i++)
            {
                Debug.Log("Jumping from Spikes");
                DirectorScript.Instance.Screenshake();
                TimeManager.Instance.HitFreeze(0.04f);
                jumpHit = true;
            }
        }
    }

    private void HitDestructibles(AttackType attackTyp)
    {
        Transform attackPosition = GetAttackTransform(attackTyp);
        Collider2D[] destructiblesToDestroy = Physics2D.OverlapCircleAll(attackPosition.position, attackRange, whatIsDestructible);
        for (int i = 0; i < destructiblesToDestroy.Length; i++)
        {
            Debug.Log("Destructible Object hit");
            DirectorScript.Instance.Screenshake();
            TimeManager.Instance.HitFreeze(0.04f);
            ParticleManager.Instance.SpawnParticles("BubbleBurst", destructiblesToDestroy[i].transform.position, Quaternion.identity);
            if (Input.GetAxisRaw("Vertical") < -.6f)
                jumpHit = true;
            Destroy(destructiblesToDestroy[i].gameObject);
        }
    }

    void CancelAttack()
    {
        attackSr.sprite = null;
        attackAnim.Play("Idle");
    }

    private Transform GetAttackTransform(AttackType attackTyp)
    {
        if (attackTyp == AttackType.AttackFrontal)
        {
            return attackPositionLR;
        }
        else if (attackTyp == AttackType.AttackOben)
        {
            return attackPositionOben;
        }
        else if (attackTyp == AttackType.AttackUnten)
        {
            return attackPositionUnten;
        }
        return null;
    }

    private void HitEnemies(AttackType attackTyp)
    {
        Transform attackPosition = GetAttackTransform(attackTyp);
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPosition.position, attackRange, whatIsEnemies);
        for (int i = 0; i < enemiesToDamage.Length; i++)
        {
            if (!enemiesToDamage[i].isTrigger)
            {
                //Debug.Log("Entity hit");

                if (PlayerChanneling.Instance.GetChargeRdy())
                {
                    DoHeavyAttack(enemiesToDamage, i);
                }
                else if (!PlayerChanneling.Instance.GetChargeRdy())
                {
                    DoNormalAttack(enemiesToDamage, i);
                }
                if (attackTyp == AttackType.AttackUnten)
                {
                    jumpHit = true;
                }
            }
        }
    }

    private void DoNormalAttack(Collider2D[] enemiesToDamage, int i)
    {
        Debug.Log("Normal Attack");
        CombatManager.Instance.TryKnockbackEnemy(enemiesToDamage[i].transform);
        enemiesToDamage[i].GetComponent<EntityHealth>().ApplyDamage(CombatManager.Instance.playerDamage);    // dmg
        //VFXManager.Instance.SpawnRandomAttackSlash(enemiesToDamage[i].ClosestPoint(PlayerManager.Instance.playerPosition));
        ParticleManager.Instance.SpawnParticles("BubbleBurst", enemiesToDamage[i].transform.position, Quaternion.identity);
        ParticleManager.Instance.SpawnParticles("HitEffect", enemiesToDamage[i].transform.position, Quaternion.identity);
        DirectorScript.Instance.Screenshake();
        TimeManager.Instance.HitFreeze(0.05f);
    }

    private void DoHeavyAttack(Collider2D[] enemiesToDamage, int i)
    {
        Debug.Log("Heavy Attack");
        enemiesToDamage[i].GetComponent<EntityHealth>().ApplyDamage(CombatManager.Instance.playerDamage * heavyAttackFactor);    // dmg
        DirectorScript.Instance.Screenshake();
        TimeManager.Instance.HitFreeze(0.1f);
        // PlayerChanneling.Instance.ResetCharge();
    }

    private void StartAttackCooldownTimer()
    {
        attackTimer = attackDelay;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (Input.GetButton("Attack"))
        {
            Gizmos.DrawWireSphere(attackPositionLR.position, attackRange);
            Gizmos.DrawWireSphere(attackPositionOben.position, attackRange);
            Gizmos.DrawWireSphere(attackPositionUnten.position, attackRange);
        }
    }

    public void InitializeAttack()
    {
        attackSr.sprite = null;
    }
}
