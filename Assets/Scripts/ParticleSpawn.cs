using UnityEngine;

public class ParticleSpawn : MonoBehaviour
{
    public GameObject particlePrefab;

    [Header("Settings")]
    [Tooltip("Amount of particles to spawn")]
    public int spawnAmount = 1;
    [Tooltip("Random radius added to position")]
    public float randomRadius = 1f;

    public void SpawnParticles()
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            float rndX = Random.Range(-randomRadius, randomRadius);
            float rndY = Random.Range(-randomRadius, randomRadius);

            Debug.Log($"RandomX = {rndX} und RandomY = {rndY}.");

            Vector2 spawnPos = (Vector2)transform.position + new Vector2(rndX, rndY);
            ParticleManager.Instance.SpawnCustomParticles(particlePrefab, transform.position, Quaternion.identity);
        }
    }

    public void SpawnParticlesAsChild()
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            float rndX = Random.Range(-randomRadius, randomRadius);
            float rndY = Random.Range(-randomRadius, randomRadius);

            Debug.Log($"RandomX = {rndX} und RandomY = {rndY}.");

            Vector2 spawnPos = (Vector2)transform.position + new Vector2(rndX, rndY);
            GameObject childParticles = ParticleManager.Instance.SpawnCustomParticlesGet(particlePrefab, transform.position, Quaternion.identity);
            childParticles.transform.SetParent(transform);
        }
    }
}
