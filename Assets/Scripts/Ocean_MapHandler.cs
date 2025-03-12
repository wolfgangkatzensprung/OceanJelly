using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Handles the overall Ocean Map, including LevelSet pooling
/// </summary>
public class Ocean_MapHandler : Singleton<Ocean_MapHandler>
{
    public Ocean_AssetSpawner assetSpawnerScript;

    [Header("Seed")]
    public int seed = 1;

    [Header("Prefabs")]
    [Tooltip("Level Sets Prefabs")]
    public List<GameObject> levelSetPrefabs = new List<GameObject>();
    private List<GameObject> addedLevelSetPrefabs = new List<GameObject>(); // LevelSets that have been added through game progress

    [Header("LevelSets Pooling")]
    [Tooltip("Currently Loaded LevelSets")]
    public List<LevelSet> activeLevelSets = new List<LevelSet>();
    [Tooltip("Inactive LevelSets")]
    public List<LevelSet> inactiveLevelSets = new List<LevelSet>();

    [Header("LevelSets Settings")]
    [Tooltip("Radius around Player to spawn LevelSets")]
    public float spawnDistance = 250f;
    [Tooltip("When a LevelSet distance to player is higher than this value, it will be replaced")]
    public float maxDistance = 1000f;
    [Tooltip("Minimum distance between LevelSet positions")]
    public float minDistance = 240f;
    [Tooltip("Minimum distance between LevelSet position and player to be despawned after being looted")]
    public float minDistanceForDespawn = 300f;

    float updateTimer = 0f;
    public float updateTime = 12f;

    [Tooltip("Maximum amount of LevelSets gleichzeitig active on the map")]
    public int maxLevelSetCount = 3;

    public delegate void LevelSetUpdateDelegate(Vector2 pos);
    private LevelSetUpdateDelegate onLevelSetUpdate;


    private void Start()
    {
        Random.InitState(seed);
        assetSpawnerScript = GetComponent<Ocean_AssetSpawner>();
    }

    private void Update()
    {
        if (GameController.Instance.inDungeon || GameController.Instance.inShipwreck)
            return;

        updateTimer += Time.deltaTime;

        if (updateTimer > updateTime)
        {
            UpdateLevelSets();
            updateTimer = 0f;
        }
    }

    private void UpdateLevelSets()
    {
        GameObject[] levelSetPrefabsArray = levelSetPrefabs.ToArray();

        LevelSet[] activeLevelSetsArray = activeLevelSets.ToArray();
        //Debug.Log("LevelSetsArray.Length: " + activeLevelSetsArray.Length);
        for (int i = activeLevelSetsArray.Length; i > 0; i--)
        {
            if ((PlayerManager.Instance.GetDistanceToPlayer(activeLevelSetsArray[i - 1].transform.position) > maxDistance) ||
                (PlayerManager.Instance.GetDistanceToPlayer(activeLevelSetsArray[i - 1].schatz.transform.position) > minDistanceForDespawn && activeLevelSetsArray[i - 1].isFinished &&
                PlayerManager.Instance.GetDistanceToPlayer(activeLevelSetsArray[i - 1].eingang.transform.position) > minDistanceForDespawn))
            {
                DisableLevelSet(activeLevelSetsArray[i - 1]);
            }
        }
        if (activeLevelSets.Count < maxLevelSetCount)
        {
            int i = Random.Range(0, levelSetPrefabsArray.Length);
            Debug.Log("Array Length: " + levelSetPrefabsArray.Length);
            Debug.Log("Generated Index: " + i);

            if (i >= 0 && i < levelSetPrefabsArray.Length)
            {
                if (inactiveLevelSets.Contains(levelSetPrefabsArray[i].GetComponent<LevelSet>()))
                {
                    ReenableExistingLevelSet(i);
                }
                else if (!activeLevelSets.Contains(levelSetPrefabsArray[i].GetComponent<LevelSet>()))
                {

                    SpawnNewLevelSet(i);
                }
            }
        }
    }

    private void SpawnNewLevelSet(int i)
    {
        Debug.Log("Spawn LevelSet " + i);
        Vector3 pos = GetRandomPosition();

        GameObject[] levelSetPrefabsArray = levelSetPrefabs.ToArray();
        GameObject levelSetObject = Instantiate(levelSetPrefabsArray[i], pos, Quaternion.identity);
        levelSetObject.transform.parent = transform;

        activeLevelSets.Add(levelSetObject.GetComponent<LevelSet>());

        onLevelSetUpdate?.Invoke(levelSetObject.transform.position);
    }



    private void ReenableExistingLevelSet(int i)
    {
        Debug.Log("Reenable LevelSet " + i);
        Vector3 pos = GetRandomPosition();

        GameObject[] levelSetPrefabsArray = levelSetPrefabs.ToArray();
        LevelSet levelSet = levelSetPrefabsArray[i].GetComponent<LevelSet>();
        levelSet.transform.position = pos;
        levelSet.gameObject.SetActive(true);
        inactiveLevelSets.Remove(levelSet);
        activeLevelSets.Add(levelSet);

        onLevelSetUpdate?.Invoke(levelSet.transform.position);
    }

    private Vector3 GetRandomPosition()
    {
        if (activeLevelSets.Count == 0)
        {
            return PlayerManager.Instance.GetPlayerPos() + GetPointOnUnitCircleCircumference() * spawnDistance;
        }
        else
        {
            while (true)
            {
                Vector2 pos = PlayerManager.Instance.GetPlayerPos() + GetPointOnUnitCircleCircumference() * spawnDistance;
                Debug.Log($"Trying to find unoccupied spot for LevelSet spawn: {pos} ?");
                LevelSet[] activeLevelSetsArray = activeLevelSets.ToArray();
                if (activeLevelSetsArray.Length > 1)
                {
                    if (Vector2.Distance(pos, activeLevelSetsArray[0].transform.position) > minDistance && Vector2.Distance(pos, activeLevelSetsArray[1].transform.position) > minDistance) // ist weit genug weg von beiden anderen LevelSets
                    {
                        return pos;
                    }
                }
                else if (Vector2.Distance(pos, activeLevelSetsArray[0].transform.position) > minDistance && activeLevelSetsArray.Length == 1)   // ist weit genug weg von dem anderen LevelSet
                {
                    return pos;
                }
            }
        }
    }

    private void DisableLevelSet(LevelSet levelSet)
    {
        levelSet.gameObject.SetActive(false);

        activeLevelSets.Remove(levelSet);
        inactiveLevelSets.Add(levelSet);
    }

    public void TryAddLevelSet(GameObject levelSetPrefab)
    {
        if (levelSetPrefabs.Contains(levelSetPrefab))
            return;

        levelSetPrefabs.Add(levelSetPrefab);
        addedLevelSetPrefabs.Add(levelSetPrefab);

        Debug.Log("Ice Cave LevelSet successfully Added");
    }



    public Vector2 GetPointOnUnitCircleCircumference()
    {
        float randomAngle = Random.Range(0f, Mathf.PI * 2f);
        return new Vector2(Mathf.Sin(randomAngle), Mathf.Cos(randomAngle)).normalized;
    }



    internal void ResetMap()
    {
        ResetLevelSets();
        assetSpawnerScript.ResetAssets();
    }

    private void ResetLevelSets()
    {
        LevelSet[] levelSets = activeLevelSets.ToArray();
        for (int i = levelSets.Length - 1; i >= 0; i--)
        {
            DisableLevelSet(levelSets[i]);
        }

        foreach (GameObject addedLevelSetPrefab in addedLevelSetPrefabs)
        {
            levelSetPrefabs.Remove(addedLevelSetPrefab);
        }

        addedLevelSetPrefabs.Clear();
    }
}
