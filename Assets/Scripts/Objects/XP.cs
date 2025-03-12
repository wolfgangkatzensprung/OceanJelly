using UnityEngine;

public class XP : MonoBehaviour
{
    public int xpWorth;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ItemCollector.Instance.CollectXP(xpWorth);
            Destroy(gameObject);
        }
    }
}
