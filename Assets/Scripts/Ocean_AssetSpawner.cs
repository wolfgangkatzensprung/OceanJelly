using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Spawns Hope, Loot and Enemies around Player, using pooling.
/// </summary>
public class Ocean_AssetSpawner : Singleton<Ocean_AssetSpawner>
{
    [Header("Prefabs")]
    [Tooltip("Prefab des Hoffnungsfunken")]
    public GameObject hopePrefab;
    internal List<GameObject> activeHope = new List<GameObject>();
    internal List<GameObject> inactiveHope = new List<GameObject>();
    public int maxHope = 1000;

    [Tooltip("Prefabs fuer Random Loot")]
    public GameObject[] randomLootPrefabs;
    internal List<GameObject> activeLoot = new List<GameObject>();
    internal List<GameObject> inactiveLoot = new List<GameObject>();
    public int maxLoot = 500;

    [Tooltip("Prefabs fuer Gegner")]
    public GameObject[] enemiesPrefabs;
    internal List<GameObject> activeEnemies = new List<GameObject>();
    internal List<GameObject> inactiveEnemies = new List<GameObject>();
    public int maxEnemies = 100;

    public GameObject[] obstaclesPrefabs;
    internal List<GameObject> activeObstacles = new List<GameObject>();
    internal List<GameObject> inactiveObastacles = new List<GameObject>();
    public int maxObstacles = 100;

    [Header("Spawn Settings")]
    float spawnerTimer = 0f;
    [Tooltip("Time between spawn calls")]
    public float spawnerThreshold = 1f;

    [Tooltip("Minimum Radius around Player where Loot spawns (inside circle)")]
    public float minLootSpawnRadius = 50f;
    [Tooltip("Maximum Radius around Player where Loot spawns (outside circle)")]
    public float maxLootSpawnRadius = 100f;

    public float minDistanceToPlayer = 30f; // fuer HopeAlongWay() line

    public float scale = 10.0f;
    public float persistence = 0.5f;
    public int octaves = 6;
    public float threshold = 0.5f;

    // Hope Along Way
    [Header("Hope Along Way")]
    public float spawnInterval = 0.5f;
    public float minHopePathDistance = 50f; // minimum distance from player to HopeAlongWay spawn
    public float spawnNoiseScale = 0.1f;
    private float timeSinceLastSpawn = 0f;
    private Transform player;

    Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        if (!GameController.Instance.inDungeon || !GameController.Instance.inShipwreck)
        {
            if (!((PlayerManager.Instance.rb.velocity != Vector2.zero) && Time.time > 5f) || !PlayerHealth.Instance.alive)
                return;

            spawnerTimer += Time.deltaTime;

            if (spawnerTimer > spawnerThreshold)
            {
                SpawnRandom();
                spawnerTimer = 0f;
            }

            SpawnHopeAlongWay();
        }
    }

    private void InitialSpawn()
    {
        for (int i = 0; i < 5; i++)
        {
            SpawnHope();
            SpawnRandom();
        }
    }

    private void SpawnRandom()
    {
        int rnd = Random.Range(0, 4);
        if (rnd == 0)
            SpawnHope();
        else if (rnd == 1)
            SpawnRandomLoot();
        else if (rnd == 2)
            SpawnObstacle();
        else if (rnd == 3)
            SpawnEnemy();
    }

    private void SpawnHopeAlongWay()    // spawn in lines from player to levelsets
    {
        timeSinceLastSpawn += Time.deltaTime;

        if (timeSinceLastSpawn >= spawnInterval)
        {
            timeSinceLastSpawn = 0f;

            List<Vector2> entrances = new List<Vector2>();
            for (int i = 0; i < Ocean_MapHandler.Instance.activeLevelSets.Count; i++)
            {
                entrances.Add(Ocean_MapHandler.Instance.activeLevelSets[i].eingang.transform.position);
            }

            if (entrances.Count > 0)
            {
                int rndIndex = Random.Range(0, entrances.Count);
                //Vector3 direction = entrances[rndIndex] - (Vector2)player.position;
                //Vector3 spawnPosition = player.position + direction.normalized * Random.Range(minHopePathDistance, direction.magnitude);
                Vector3 spawnPosition = GetRandomPointOnLine(player.position, entrances[rndIndex]);
                GameObject o = Instantiate(hopePrefab, spawnPosition, Quaternion.identity);
                activeHope.Add(o);
            }
        }
    }

    private void SpawnHope()    // spawn im kreis um player
    {
        if (activeHope.Count >= maxHope)
            return;

        float randomRadius = Random.Range(minLootSpawnRadius, maxLootSpawnRadius); // Random radius between the inner and outer radius
        Vector3 lootSpawnPos = PlayerManager.Instance.GetPlayerPos() + GetPointOnUnitCircleCircumference() * randomRadius;

        GameObject hope = Instantiate(hopePrefab, lootSpawnPos, Quaternion.identity);
        hope.transform.parent = transform;
        activeHope.Add(hope);
    }

    private void SpawnRandomLoot()  // spawn im kreis um player
    {
        if (activeLoot.Count >= maxLoot)
            return;

        float randomRadius = Random.Range(minLootSpawnRadius, maxLootSpawnRadius); // Random radius between the inner and outer radius
        Vector3 lootSpawnPos = PlayerManager.Instance.GetPlayerPos() + GetPointOnUnitCircleCircumference() * randomRadius;

        GameObject loot = Instantiate(randomLootPrefabs[Random.Range(0, randomLootPrefabs.Length)], lootSpawnPos, Quaternion.identity);
        loot.transform.parent = transform;
        activeLoot.Add(loot);
    }

    private void SpawnObstacle()   // spawn in box um player
    {
        if (activeObstacles.Count >= maxObstacles)
            return;
        float randomRadius = Random.Range(minLootSpawnRadius, maxLootSpawnRadius); // Random radius between the inner and outer radius
        Vector3 randomPos = PlayerManager.Instance.GetPlayerPos() + GetPointOnUnitCircleCircumference() * randomRadius;

        float noise = Mathf.PerlinNoise(randomPos.x / scale, randomPos.y / scale) * octaves;
        noise = Mathf.Pow(noise, persistence);
        Vector2 spawnPos = new Vector2(noise * randomPos.x, noise * randomPos.y);

        Vector3 screenPos = mainCamera.WorldToScreenPoint(spawnPos);

        //Check if the position is on screen
        if (screenPos.x < -threshold * Screen.width || screenPos.x > (1 + threshold) * Screen.width ||
           screenPos.y < -threshold * Screen.height || screenPos.y > (1 + threshold) * Screen.height)
        {
            if (inactiveObastacles.Count != 0)
            {
                int randomIndex = Random.Range(0, inactiveObastacles.Count - 1);
                GameObject reusableObstacle = inactiveObastacles[randomIndex];
                inactiveEnemies.Remove(reusableObstacle);
                activeEnemies.Add(reusableObstacle);
                reusableObstacle.transform.position = spawnPos;
                reusableObstacle.SetActive(true);
            }
            else
            {
                GameObject obstacle = Instantiate(obstaclesPrefabs[Random.Range(0, obstaclesPrefabs.Length)], spawnPos, Quaternion.identity);
                obstacle.transform.parent = transform;
                activeObstacles.Add(obstacle);
            }
        }
        else
        {
            SpawnObstacle();
        }
    }
    private void SpawnEnemy() // spawn in box um player
    {
        if (activeEnemies.Count >= maxEnemies)
            return;

        float randomRadius = Random.Range(minLootSpawnRadius, maxLootSpawnRadius); // Random radius between the inner and outer radius
        Vector3 randomPos = PlayerManager.Instance.GetPlayerPos() + GetPointOnUnitCircleCircumference() * randomRadius;

        float noise = Mathf.PerlinNoise(randomPos.x / scale, randomPos.y / scale) * octaves;
        noise = Mathf.Pow(noise, persistence);
        Vector2 spawnPos = new Vector2(noise * randomPos.x, noise * randomPos.y);

        Vector3 screenPos = mainCamera.WorldToScreenPoint(spawnPos);

        //Check if the position is on screen
        if (screenPos.x < -threshold * Screen.width || screenPos.x > (1 + threshold) * Screen.width ||
           screenPos.y < -threshold * Screen.height || screenPos.y > (1 + threshold) * Screen.height)
        {
            if (inactiveEnemies.Count != 0)
            {
                int randomIndex = Random.Range(0, inactiveEnemies.Count - 1);
                GameObject reusableEnemy = inactiveEnemies[randomIndex];
                inactiveEnemies.Remove(reusableEnemy);
                activeEnemies.Add(reusableEnemy);
                reusableEnemy.transform.position = spawnPos;
                reusableEnemy.SetActive(true);
            }
            else
            {
                GameObject enemy = Instantiate(enemiesPrefabs[Random.Range(0, enemiesPrefabs.Length)], spawnPos, Quaternion.identity);
                activeEnemies.Add(enemy);
            }
        }
        else
        {
            //Try to spawn again
            SpawnEnemy();
        }
    }

    public static Vector2 GetPointOnUnitCircleCircumference()
    {
        float randomAngle = Random.Range(0f, Mathf.PI * 2f);
        return new Vector2(Mathf.Sin(randomAngle), Mathf.Cos(randomAngle)).normalized;
    }

    public Vector2 GetRandomPointOnLine(Vector2 start, Vector2 end)
    {
        // Generate a random value between 0 and 1
        float t = Random.Range(0f, 1f);

        // Use the random value to find a point on the line
        Vector2 pointOnLine = (1 - t) * start + t * end;

        // Check the distance between point and player
        float distance = Vector2.Distance(pointOnLine, player.position);

        // If the point is too close to the player
        if (distance < minDistanceToPlayer)
        {
            // Generate a new random point within a circle with a radius of minDistanceToPlayer
            Vector2 randomPoint = Random.insideUnitCircle * minDistanceToPlayer;

            // Add the random point to the player position
            pointOnLine = (Vector2)player.position + randomPoint;
        }

        return pointOnLine;
    }

    internal void ResetAssets()
    {
        DisableHopeAll();
        DisableLootAll();
        DisableEnemiesAll();
    }
    internal void ClearListsAll()
    {
        activeHope.Clear();
        inactiveHope.Clear();
        activeLoot.Clear();
        inactiveLoot.Clear();
        activeEnemies.Clear();
        inactiveEnemies.Clear();
    }

    private void DisableEnemiesAll()
    {
        GameObject[] enemies = activeEnemies.ToArray();
        for (int i = enemies.Length - 1; i >= 0; i--)
        {
            Debug.Log($"Disabling Enemy {i}");
            if (enemies[i] != null)
            {

                enemies[i].SetActive(false);
                activeEnemies.Remove(enemies[i]);
                inactiveEnemies.Add(enemies[i]);
            }
            else
            {
                inactiveEnemies.Clear();
                activeEnemies.Clear();
            }
        }
    }

    private void DisableLootAll()
    {
        GameObject[] loot = activeLoot.ToArray();
        for (int i = loot.Length - 1; i >= 0; i--)
        {
            Debug.Log($"Disabling Loot {i}");
            loot[i].SetActive(false);
            activeLoot.Remove(loot[i]);
            inactiveLoot.Add(loot[i]);
        }
    }

    private void DisableHopeAll()
    {
        GameObject[] hope = activeHope.ToArray();
        for (int i = hope.Length - 1; i >= 0; i--)
        {
            Debug.Log($"Disabling Hope {i}");

            hope[i].SetActive(false);
            activeHope.Remove(hope[i]);
            inactiveHope.Add(hope[i]);
        }
    }
}
