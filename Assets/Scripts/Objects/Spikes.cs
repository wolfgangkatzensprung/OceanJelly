using System.Collections;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    public int spikeDamage = 9;
    public float knockbackForce = 123f;
    public float knockupCooldown = 1f;

    bool isSpiking;

    private void OnCollisionStay2D(Collision2D collision)
    {
        HandleSpiking(collision);
    }

    private void HandleSpiking(Collision2D collision)
    {
        if (!isSpiking && collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(SpikePlayer(collision));
        }
        if (collision.gameObject.CompareTag("Entity"))
        {
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (TryGetComponent(out EntityHealth eh))
            {
                eh.ApplyDamage(spikeDamage);
            }
            Vector2 knockbackDir = (Vector2)collision.transform.position - (Vector2)collision.GetContact(0).point;
            rb.AddForce(knockbackDir * knockbackForce);
        }
    }

    IEnumerator SpikePlayer(Collision2D collision)
    {
        Debug.Log("SpikePlayer()");
        isSpiking = true;

        Vector2 knockbackDir = (Vector2)PlayerManager.Instance.playerTrans.position - (Vector2)collision.GetContact(0).point;

        CombatManager.Instance.SetupKnockback(knockbackDir, knockbackForce, knockupCooldown);
        CombatManager.Instance.DealDamageToPlayer(spikeDamage);
        CombatManager.Instance.TryDoKnockback();

        yield return new WaitForEndOfFrame();
        yield return new WaitForFixedUpdate();

        isSpiking = false;
    }
}
