using System.Collections;
using UnityEngine;

public class Schatztruhe : MonoBehaviour
{
    [Tooltip("Hope Prefab")]
    public GameObject hopePrefab;
    [Tooltip("Muschelschild")]
    public GameObject specialLootPrefab;
    public Animator anim;

    public int lootDropAmount = 15;
    public Transform lootSpawnPos;

    internal bool isLooted = false;

    public void OpenTreasureChest()
    {
        StartCoroutine(SpawnLootRoutine());
        isLooted = true;
    }

    private IEnumerator SpawnLootRoutine()
    {
        int bonusLoot = 0;
        if (AbilityManager.Instance.abilityLevels.TryGetValue(AbilityManager.Abilities.TreasureHunter, out bonusLoot))
        {
            Debug.Log("BonusLoot: " + AbilityManager.Instance.abilityLevels[AbilityManager.Abilities.TreasureHunter]);
        }
        for (int i = 0; i < lootDropAmount + bonusLoot; i++)
        {
            GameObject o = Instantiate(hopePrefab, lootSpawnPos.position + new Vector3(Mathf.Sin(Time.time) * 3f, Mathf.Sin(Time.time) * 3f, 0), Quaternion.identity);
            o.transform.parent = transform.parent;  // Hope wird child vom LevelSet
            yield return new WaitForSeconds(.1f);
        }

        Instantiate(specialLootPrefab, lootSpawnPos.position, Quaternion.identity);

        gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
            anim.Play("Open");
    }
}
