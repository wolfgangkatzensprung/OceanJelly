using System.Collections;
using UnityEngine;

public class DamageOnTrigger : MonoBehaviour
{
    public int dmgPerTick = 1;
    [Tooltip("Delay between ticks in seconds")]
    public float delay = 1f;

    bool damaging;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!damaging)
            StartCoroutine(DealDamage(dmgPerTick, delay));
    }
    IEnumerator DealDamage(int dmg, float delay)
    {
        damaging = true;
        CombatManager.Instance.DealDamageToPlayer(dmg);
        yield return new WaitForSeconds(delay);
        damaging = false;
    }
}
