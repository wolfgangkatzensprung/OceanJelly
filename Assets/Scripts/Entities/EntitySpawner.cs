using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject[] entityPrefabs;
    public float[] entityAmount;

    [Header("Settings")]

    [Tooltip("If true, entities will spawn at this transform.position")]
    public bool spawnHere = true;

    [Tooltip("Spawner will self destruct after spawning this amount of objects")]
    public float maxSpawnAmount = 1f;

    [Tooltip("Objects will be spawned at these positions unless spawnHere is set to true")]
    public Transform[] spawnPoints;

    [Tooltip("Object will be offset by a random amount")]
    public Vector2 rndAdderRadius;

    [Tooltip("If true, entities will spawn as long as player is in trigger zone")]
    public bool continuousSpawning = false;

    [Tooltip("Seconds until next spawn")]
    public float spawnRate = 1f;

    [Tooltip("Delay until first spawn (doesnt work for continuous spawning)")]
    public float spawnDelay = 0f;

    int spawnPointIndex = 0;
    Vector2 randomAdder;
    Vector2 spawnPosition;
    bool isSpawning;
    float spawnTimer = 0f;
    List<GameObject> spawnedObjects = new List<GameObject>();

    private void Start()
    {
        if (TryGetComponent<SpriteRenderer>(out SpriteRenderer sr))
            sr.sprite = null;
    }

    public void SpawnEntities()
    {
        for (int i = 0; i < entityPrefabs.Length; i++)
        {
            for (int j = 0; j < (int)entityAmount[i]; j++)
            {
                randomAdder.x = Random.Range(-rndAdderRadius.x, rndAdderRadius.x);
                randomAdder.y = Random.Range(-rndAdderRadius.y, rndAdderRadius.y);

                spawnPointIndex = j % spawnPoints.Length;

                if (spawnHere)
                {
                    spawnPosition = transform.position;
                }
                else if (spawnPoints != null)
                {
                    spawnPosition = spawnPoints[spawnPointIndex].position + (Vector3)randomAdder;
                }

                spawnedObjects.Add(Instantiate(entityPrefabs[i], spawnPosition + randomAdder, Quaternion.identity));

                if (spawnedObjects.Capacity >= maxSpawnAmount)
                    Destroy(gameObject);
            }
        }
        spawnTimer = 0f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!isSpawning && spawnDelay > 0)
            {
                StartCoroutine(SpawnEntitiesAfterDelay());
                return;
            }
            SpawnEntities();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (continuousSpawning)
        {
            if (collision.CompareTag("Player"))
            {
                spawnTimer += Time.deltaTime;
                if (spawnTimer > spawnRate)
                {
                    SpawnEntities();
                }
            }
        }
    }

    IEnumerator SpawnEntitiesAfterDelay()
    {
        isSpawning = true;
        yield return new WaitForSeconds(spawnDelay);
        SpawnEntities();
        isSpawning = false;
    }

    private void OnDrawGizmos()
    {
        if (spawnPoints != null)
        {
            foreach (Transform spawnPoint in spawnPoints)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(spawnPoint.position, 7f);
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(spawnPoint.position, 3f);
            }
        }
    }
}
