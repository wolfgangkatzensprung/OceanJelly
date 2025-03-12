using UnityEngine;

public class BigEnemyAttack : MonoBehaviour
{
    Animator anim;
    Rigidbody2D rb;

    public int damage;

    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CombatManager.Instance.DealDamageToPlayer(damage);
        }
    }
}
