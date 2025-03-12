using UnityEngine;

public class TriggerEnemyMovement : MonoBehaviour
{
    public GameObject enemyToTrigger;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (enemyToTrigger != null)
            {
                Debug.Log("Triggered by Trigger Zone");
                enemyToTrigger.GetComponent<Animator>().SetBool("isAttacking", true);
            }
        }
    }
}
