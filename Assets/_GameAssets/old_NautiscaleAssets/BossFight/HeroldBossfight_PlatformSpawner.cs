using System.Collections;
using UnityEngine;

public class HeroldBossfight_PlatformSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject[] prefabs;
    public Transform[] spawnPoints;

    [Header("Settings")]
    int spawnPointIndex = 0;
    [Tooltip("Maximum distance for random position")]
    public Vector2 rndXAddMax;
    Vector2 randomXAdd;
    Vector2 spawnPosition;
    bool isSpawning;

    [Tooltip("Seconds until next spawn")]
    public float spawnRate = 1f;

    public void SpawnObjects()
    {
        isSpawning = true;
        StartCoroutine(SpawnPlatforms());
    }

    IEnumerator SpawnPlatforms()
    {
        while (isSpawning)
        {

            int randomIndex = Random.Range(0, prefabs.Length);
            randomXAdd.x = Random.Range(rndXAddMax.x, rndXAddMax.y);

            yield return new WaitForSeconds(spawnRate);

            Instantiate(prefabs[randomIndex], spawnPoints[spawnPointIndex].position + (Vector3)randomXAdd, Quaternion.identity);
        }
    }
}
