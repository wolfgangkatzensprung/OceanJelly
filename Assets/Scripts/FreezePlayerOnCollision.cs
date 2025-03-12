using UnityEngine;

public class FreezePlayerOnCollision : MonoBehaviour
{

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") && !CombatManager.Instance.frozen)
        {
            CombatManager.Instance.SetFrozen();
        }
    }
}
