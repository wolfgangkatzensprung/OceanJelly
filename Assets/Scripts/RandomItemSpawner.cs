using System.Collections;
using UnityEngine;

public class RandomItemSpawner : MonoBehaviour
{
    public GameObject[] itemPrefabs;

    [Tooltip("If spawn should be delayed")]
    public bool delayed = true;
    [Tooltip("Delay after which the spawn happens")]
    public float spawnDelay = .3f;

    void Start()
    {
        if (delayed)
        {
            StartCoroutine(DelayedSpawnRoutine());
        }
        else
        {
            Instantiate(itemPrefabs[Random.Range(0, itemPrefabs.Length)], transform.position, Quaternion.identity);
            gameObject.SetActive(false);
        }
    }

    IEnumerator DelayedSpawnRoutine()
    {
        yield return new WaitForSeconds(spawnDelay);
        Instantiate(itemPrefabs[Random.Range(0, itemPrefabs.Length)], transform.position, Quaternion.identity);
        gameObject.SetActive(false);
    }
}
