using UnityEngine;

public class DebugPrefabs : MonoBehaviour
{
    public PrefabAsset[] prefabs;

    public PrefabAsset[] GetPrefabs()
    {
        return prefabs;
    }

    [System.Serializable]
    public struct PrefabAsset
    {
        public string prefabName;
        public GameObject prefab;

        public GameObject GetPrefab()
        {
            return prefab;
        }
        public string GetName()
        {
            return prefabName;
        }
    }
    public string[] GetPrefabNames()
    {
        string[] prefabNames = new string[prefabs.Length];
        for (int i = 0; i < prefabs.Length; i++)
        {
            prefabNames[i] = prefabs[i].GetName();
        }
        return prefabNames;
    }
}
