using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MazeGenerator))]
public class Dungeon : Singleton<Dungeon>
{
    public bool preventEnemySpawn;

    internal Transform playerTrans;
    internal MazeGenerator mg;
    internal GridLayout gridLayout;
    Camera mainCam;

    [Header("Prefabs")]
    public GameObject treasureChest;
    public GameObject mermaid;
    [Tooltip("Minimum distance from player where treasure objects are spawned")]
    public float minDistance = 30f;

    public List<DungeonAsset> loot; // list of prefabs to choose from
    public List<DungeonAsset> enemies; // list of prefabs to choose from
    public List<DungeonAsset> obstacles; // list of prefabs to choose from

    internal List<GameObject> activeEnemies = new List<GameObject>();
    internal List<GameObject> inactiveEnemies = new List<GameObject>();

    [Header("Chunks")]

    public Vector2Int chunkSize; // size of each chunk in tiles
    private Vector2Int currentChunk; // the current chunk the player is in
    private Vector2Int previousChunk; // the previous chunk the player was in

    private List<Vector2Int> loadedChunks;  // chunks where loot has been spawned

    [Tooltip("Tick rate for Chunk Check (Loot Spawn and Allocation)")]
    public float chunkCheckRate = 1f;
    float chunkCheckTimer = 0f;


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

    private Vector2 _exitPoint = Vector2.zero;
    internal Vector2 exitPoint
    {
        get { return _exitPoint; }
        set
        {
            if (value == Vector2.zero)
            {
                _exitPoint = new Vector2(.000001f, .000001f);
            }
            else
            {
                _exitPoint = value;
            }
        }
    }

    public enum DungeonType { Default, IceCave };

    private void Start()
    {
        mg = GetComponent<MazeGenerator>();
        playerTrans = PlayerManager.Instance.playerTrans;
        gridLayout = mg.GetComponent<GridLayout>();
        mainCam = Camera.main;

        MusicManager.Instance.StopMusic();

        GameController.Instance.StartDungeon(this);
        UIManager.Instance.FadeIn();

        Vector3Int gridPosition = new Vector3Int(mg.gridSize.x / 2, mg.gridSize.y / 2, 0);
        Vector2 spawnPos = mg.tilemap.CellToWorld(gridPosition);

        loadedChunks = new List<Vector2Int>();

        playerTrans.position = spawnPos;
        currentChunk = new Vector2Int((int)(playerTrans.position.x / chunkSize.x), (int)(playerTrans.position.y / chunkSize.y));

        SpawnTreasures();
    }

    private void Update()
    {

        if (exitPoint != null && exitPoint != Vector2.zero)
        {
            ExitCheck();
        }

        if (isFinished)
            return;

        chunkCheckTimer += Time.deltaTime;
        if (chunkCheckTimer > chunkCheckRate)
        {
            ChunkCheck();
            chunkCheckTimer = 0f;
        }
        if (preventEnemySpawn)
            return;
        spawnTimer += Time.deltaTime;
        if (spawnTimer > spawnRate)
        {
            SpawnEnemy();
            spawnTimer = 0f;
        }
    }

    private void ChunkCheck()
    {
        // check if the player has moved to a new chunk
        previousChunk = currentChunk;
        currentChunk = (Vector2Int)(gridLayout.WorldToCell(playerTrans.position) / (int)chunkSize.x);
        if (currentChunk != previousChunk)
        {
            Debug.Log($"Chunk Change Check Positive. Previous Chunk: {previousChunk}, Current Chunk: {currentChunk}");

            // load items in the new chunk and the neighbour chunks
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Vector2Int neighbourChunk = new Vector2Int(currentChunk.x + x, currentChunk.y + y);
                    if (!loadedChunks.Contains(neighbourChunk))
                    {
                        SpawnDungeonLoot(neighbourChunk);
                        loadedChunks.Add(neighbourChunk);
                    }
                }
            }
        }
    }

    private void SpawnEnemy() // spawn in box um player
    {
        if (activeEnemies.Count >= maxEnemies)
            return;

        float randomRadius = Random.Range(minSpawnRadius, maxSpawnRadius);  // Random radius between the inner and outer radius
        Vector3 spawnPos = PlayerManager.Instance.GetPlayerPos() + Ocean_AssetSpawner.GetPointOnUnitCircleCircumference() * randomRadius;

        Vector3 screenPos = mainCam.WorldToScreenPoint(spawnPos);
        if (screenPos.x < -threshold * Screen.width || screenPos.x > (1 + threshold) * Screen.width ||      // if inside screen
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


    private void SpawnTreasures()       // Room-based Treasures
    {
        int chestsSkipped = 0;
        int lastSkippedChestIndex = -1;

        for (int i = 0; i < mg.roomCenters.Count; i++)
        {
            Vector2Int room = mg.roomCenters[i];
            Vector3 roomCenterWorldPos = mg.tilemap.CellToWorld((Vector3Int)room);
            if (Vector3.Distance(playerTrans.position, roomCenterWorldPos) > minDistance)
            {
                if (i < mg.roomCenters.Count - 1)
                {
                    Debug.Log($"Spawn Dungeon Treasure {i} : Chest");
                    Instantiate(treasureChest, roomCenterWorldPos, Quaternion.identity);
                }
                else
                {
                    Debug.Log($"Spawn Dungeon Treasure {i} : Mermaid");
                    Instantiate(mermaid, roomCenterWorldPos, Quaternion.identity);
                }
            }
            else
            {
                chestsSkipped += 1;
                lastSkippedChestIndex = i;
                Debug.Log($"Treasure Chest {i} skipped. Chests skipped in total: {chestsSkipped}");
            }
        }
        if (lastSkippedChestIndex > -1)
        {
            Vector2 spawnPos = mg.tilemap.CellToWorld((Vector3Int)mg.roomCenters[lastSkippedChestIndex]);
            Debug.Log($"Spawn Mermaid at Room {lastSkippedChestIndex} center");
            Instantiate(mermaid, spawnPos, Quaternion.identity);
        }
    }

    private void SpawnDungeonLoot(Vector2Int chunk)     // Grid-based Dungeon Loot
    {
        Vector2Int startPos = new Vector2Int(chunk.x * chunkSize.x, chunk.y * chunkSize.y);
        Debug.Log($"StartPos is {startPos} at Chunk {chunk}");
        Vector2Int endPos = new Vector2Int(startPos.x + chunkSize.x, startPos.y + chunkSize.y);
        Debug.Log($"EndPos is {endPos} at Chunk {chunk}");

        for (int x = startPos.x; x < endPos.x; x++)
        {
            for (int y = startPos.y; y < endPos.y; y++)
            {
                Vector3Int cellPos = new Vector3Int(x, y, 0);
                if (mg.tilemap.HasTile(cellPos))
                {
                    Debug.Log($"Tile found at {x}, {y}");
                    continue;
                }
                else
                {
                    Vector3 worldPos = mg.tilemap.CellToWorld(cellPos) + gridLayout.cellSize * .5f;
                    Debug.Log($"Free Cell found at {x}, {y}. Spawning Dungeon Loot on Tile World Position {worldPos}");
                    Instantiate(GetRandomPrefab(loot), worldPos, Quaternion.identity);
                }
            }
        }
    }

    private GameObject GetRandomPrefab(List<DungeonAsset> dungeonAssets)
    {
        float totalWeight = 0.0f;

        foreach (DungeonAsset item in dungeonAssets)
        {
            totalWeight += item.weight;
        }

        if (loot.Count == 0 || totalWeight == 0.0f)
        {
            return null;
        }

        float randomWeight = Random.Range(0.0f, totalWeight);

        // Select a dungeon asset based on its weight
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

    private void ExitCheck()
    {
        float exitDistance = Vector2.Distance(playerTrans.position, exitPoint);
        Debug.Log($"Player ist {exitDistance} vom ExitPoint {exitPoint} entfernt. Radius ist {mg.exitRadius} Tiles, also {mg.exitRadius * mg.tilemap.cellSize.x}.");

        if (exitDistance < mg.exitRadius * mg.tilemap.cellSize.x)
        {
            isFinished = true;
            GameProgress.Instance.dungeonsFinished += 1;
            GameProgress.Instance.AddIceCaveLevelSet();     // Ice Cave Entrance unlocked in Ocean
            GameController.Instance.FinishDungeon();
        }
    }

    private void OnDrawGizmos()
    {
        if (exitPoint != null && exitPoint != Vector2.zero)
        {
            Gizmos.color = new Color(1f, 0f, 1f, 1f);
            Gizmos.DrawSphere(exitPoint, mg.exitRadius * mg.tilemap.cellSize.x);
        }
    }

    [System.Serializable]
    public class DungeonAsset
    {
        public GameObject prefab;
        public float weight;
    }
}