using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

/// <summary>
/// Similar to Dungeon but no MazeGenerator, TileMap, Chunks, ExitPoint, Treasures
/// </summary>
public class IceCave : MonoBehaviour
{
    public BossfightHandler bossfight;
    internal Transform playerTrans;
    Camera mainCam;

    public Transform backgroundImageTrans;
    public SpriteShapeController iceCaveSsc;
    public GlyphPuzzle glyphPuzzle;

    public bool preventEnemySpawn;

    [Header("Prefabs")]
    [Tooltip("Minimum distance from player where treasure objects are spawned")]
    public float minDistance = 30f;

    public List<DungeonAsset> loot; // list of prefabs to choose from
    public List<DungeonAsset> enemies; // list of prefabs to choose from
    public List<DungeonAsset> obstacles; // list of prefabs to choose from

    internal List<GameObject> activeEnemies = new List<GameObject>();
    internal List<GameObject> inactiveEnemies = new List<GameObject>();

    [Header("Challenge Settings")]
    public int maxEnemies = 100;
    [Tooltip("Enemy Spawning Rate in seconds")]
    public float spawnRate = 4.756321f;
    float spawnTimer = 0f;

    // enemy spawning spread
    public float scale = 10.0f;
    public float persistence = 0.5f;
    public int octaves = 6;
    public float threshold = 0.5f;

    public float minSpawnRadius = 30;
    public float maxSpawnRadius = 150;

    private bool isFinished;

    private void Awake()
    {
        UIManager.Instance.FadeIn();
    }

    private void Start()
    {
        playerTrans = PlayerManager.Instance.playerTrans;
        mainCam = Camera.main;

        transform.position = playerTrans.position;

        MusicManager.Instance.PlayMusic("IceCave");
        GameController.Instance.StartIceCave(this);

        for (int i = 0; i < 3; i++)
        {
            SpawnEnemy();
        }
    }

    private void Update()
    {
        if (isFinished)
            return;

        if (preventEnemySpawn)
            return;

        spawnTimer += Time.deltaTime;
        if (spawnTimer > spawnRate)
        {
            SpawnEnemy();
            spawnTimer = 0f;
        }
    }

    private void SpawnEnemy() // spawn in box um player
    {
        if (activeEnemies.Count >= maxEnemies)
            return;

        float randomRadius = Random.Range(minSpawnRadius, maxSpawnRadius); // Random radius between the inner and outer radius
        Vector3 spawnPos = PlayerManager.Instance.GetPlayerPos() + Ocean_AssetSpawner.GetPointOnUnitCircleCircumference() * randomRadius;

        Vector3 screenPos = mainCam.WorldToScreenPoint(spawnPos);
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
                GameObject enemy = Instantiate(enemies[Random.Range(0, enemies.Count)].prefab, spawnPos, Quaternion.identity);
                activeEnemies.Add(enemy);
            }
        }
        else SpawnEnemy();  // retry
    }

    internal Vector2[] GetPoints()
    {
        Spline spline = iceCaveSsc.spline;
        Vector2[] points = new Vector2[spline.GetPointCount()];

        for (int i = 0; i < spline.GetPointCount(); i++)
        {
            points[i] = spline.GetPosition(i) + iceCaveSsc.transform.position;
        }

        return points;
    }

    internal Collider2D GetCollider()
    {
        return iceCaveSsc.GetComponent<EdgeCollider2D>();
    }

    private GameObject GetRandomPrefab(List<DungeonAsset> dungeonAssets)
    {
        float totalWeight = 0.0f;

        // Calculate the total weight of all loot items
        foreach (DungeonAsset item in dungeonAssets)
        {
            totalWeight += item.weight;
        }

        // If there are no loot items or the total weight is 0, return null
        if (loot.Count == 0 || totalWeight == 0.0f)
        {
            return null;
        }

        float randomWeight = Random.Range(0.0f, totalWeight);

        // Select a loot item based on the probability distribution
        foreach (DungeonAsset item in dungeonAssets)
        {
            if (randomWeight < item.weight)
            {
                return item.prefab;
            }

            randomWeight -= item.weight;
        }

        // If we reach this point, return the last loot item
        return dungeonAssets[dungeonAssets.Count - 1].prefab;
    }

    private void OnDisable()
    {
        if (CombatManager.Instance != null)
            CombatManager.Instance.ResetFrozen();
    }

    [System.Serializable]
    public class DungeonAsset
    {
        public GameObject prefab;
        public float weight;
    }

    internal void StartBossfight()
    {
        TimeManager.Instance.EndSlowMotion();
        bossfight.StartPhase1();
    }
}