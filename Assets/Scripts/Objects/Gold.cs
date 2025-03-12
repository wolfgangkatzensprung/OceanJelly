using UnityEngine;

public class Gold : MonoBehaviour
{
    public int goldWorth;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ItemCollector.Instance.CollectMoney(goldWorth);
        if (collision.collider.CompareTag("Player"))
            Destroy(gameObject);
    }
}
