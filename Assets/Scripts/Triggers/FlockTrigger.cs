using UnityEngine;

public class FlockTrigger : MonoBehaviour
{
    public Flocking flockSpawner;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!flockSpawner.instaActivate)
                flockSpawner.SpawnFlock();
            Destroy(gameObject);
        }
    }
}
