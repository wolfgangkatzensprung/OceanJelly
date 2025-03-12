using System.Collections;
using UnityEngine;

public class EnemyDmgTouch : MonoBehaviour
{
    [SerializeField] int damage = 1;
    [SerializeField] bool knockbackEnabled = true;
    public float knockbackStrength = 100f;
    public float knockbackTime = .5f;

    bool damaging;
    public float dmgCooldown = .33f;

    private void OnDisable()
    {
        damaging = false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!damaging && collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(DealDmgOnTouch());
        }
    }

    IEnumerator DealDmgOnTouch()
    {
        Debug.Log($"{gameObject.name} DmgTouch");
        damaging = true;
        CombatManager.Instance.DealDamageToPlayer(damage);
        if (knockbackEnabled)
        {
            CombatManager.Instance.SetupKnockback(knockbackStrength, transform.position, knockbackTime);
        }
        yield return new WaitForSeconds(dmgCooldown);
        damaging = false;
    }
}
