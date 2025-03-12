using System.Collections;
using UnityEngine;

public class SpawnNpc : MonoBehaviour
{
    public GameObject npcPrefab;
    public Transform spawnPosition;
    public float delay;
    [Tooltip("Enable NPC instead of Instantiating")]
    public bool enablerMode;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        StartCoroutine(spawnDelay());
    }

    IEnumerator spawnDelay()
    {
        yield return new WaitForSeconds(delay);
        if (enablerMode)
        {
            npcPrefab.SetActive(true);
        }
        else if (spawnPosition != null)
        {
            Instantiate(npcPrefab, spawnPosition);
        }
    }
}
