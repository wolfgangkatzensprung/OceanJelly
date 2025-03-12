using System.Collections;
using UnityEngine;

public class EntityHealth : MonoBehaviour
{
    EnemyPatrolMovement em;
    Animator anim;

    [Header("Settings")]
    public float maxHealth = 5;
    float currentHealth;
    [SerializeField] bool turnWhenAttacked = false;
    [SerializeField] bool reflectProjectiles = false;
    [SerializeField] bool isInvincible = false;

    Color startColor;
    float damagedTime = .1f;

    [Header("Loot")]
    [Tooltip("What loot to drop")]
    public GameObject lootDropPrefab;
    [Tooltip("How many copies of loot drop")]
    public float lootDropAmount;
    public GameObject xpDropPrefab;
    public int xpDropAmount = 3;

    // Cooldown
    bool rdyForDmg = true;

    public delegate void DamagedDelegate();
    public DamagedDelegate onDamaged;
    public delegate void DeathDelegate();
    public DeathDelegate onDeath;

    private void OnEnable()
    {
        currentHealth = maxHealth;
        rdyForDmg = true;
    }

    private void Start()
    {
        if (turnWhenAttacked)
        {
            em = GetComponent<EnemyPatrolMovement>();
        }
    }

    public void ApplyDamage(float damage)
    {
        if (rdyForDmg)
        {
            rdyForDmg = false;
            onDamaged?.Invoke();

            if (TryGetComponent(out Animator anim))
            {
                anim.SetTrigger("Damaged");
            }

            StartCoroutine(DmgTakenTimer());
            //if (turnWhenAttacked)
            //{
            //    em.TurnOnAttacked();
            //}
            currentHealth -= damage;
            CheckHealth();
        }
    }

    private IEnumerator DmgTakenTimer()
    {
        yield return new WaitForSeconds(damagedTime);
        rdyForDmg = true;
    }

    public void TakeProjectileDamage(float damage, Transform projectileTransform, float speed)
    {
        if (reflectProjectiles)
        {
            Debug.Log("Reflecting Projectile");
            Vector2 dir = projectileTransform.position - transform.position;
            Rigidbody2D rb = projectileTransform.GetComponent<Rigidbody2D>();
            rb.velocity = new Vector2(dir.x, dir.y).normalized * speed;
        }
        else
        {
            Debug.Log("Not Reflecting Projectile");
            ApplyDamage(damage);
            Destroy(projectileTransform.gameObject);
            CheckHealth();
        }
    }

    private void CheckHealth()
    {
        if (!isInvincible)
        {
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                Death();
            }
        }
    }

    public void Death()
    {
        onDeath?.Invoke();

        //Death Animation
        ParticleManager.Instance.SpawnParticles("Death", transform.position, Quaternion.identity);
        ParticleManager.Instance.SpawnParticles("DeathFlecken", transform.position, Quaternion.identity);
        DropLoot();
        DropXP();
        gameObject.SetActive(false);
    }

    private void DropXP()
    {
        for (int i = 0; i < xpDropAmount; i++)
        {
            Instantiate(xpDropPrefab, transform.position + (Vector3)PerlinHelper.GetPerlin2(transform.position + new Vector3(i, i, 0f)), Quaternion.identity);
        }
    }

    public void DropLoot()
    {
        for (int i = 0; i < lootDropAmount; i++)
        {
            Instantiate(lootDropPrefab, transform.position + (Vector3)PerlinHelper.GetPerlin2(transform.position), Quaternion.identity);
        }
    }

    internal float GetHealth()
    {
        return currentHealth;
    }
}
