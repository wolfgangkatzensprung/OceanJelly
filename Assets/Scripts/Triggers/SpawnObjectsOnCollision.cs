using UnityEngine;

public class SpawnObjectsOnCollision : MonoBehaviour
{
    public Transform spawnPoint;
    public GameObject[] objectsToSpawn;
    public bool enablerMode;
    bool sleep;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!sleep)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                for (int i = 0; i < objectsToSpawn.Length; i++)
                {
                    if (enablerMode)
                    {
                        objectsToSpawn[i].SetActive(true);
                    }
                    else
                    {
                        Instantiate(objectsToSpawn[i], spawnPoint);
                    }
                }
            }
        }
    }
}
