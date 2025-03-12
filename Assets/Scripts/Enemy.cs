using UnityEngine;

public class Enemy : MonoBehaviour
{
    private void OnDisable()
    {
#if UNITY_EDITOR
        return;
#endif
        if (Ocean_AssetSpawner.Instance != null)
        {
            Ocean_AssetSpawner.Instance.activeEnemies.Remove(gameObject);
        }
    }
}
