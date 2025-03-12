using UnityEngine;

public class DecoyPlatform : MonoBehaviour
{
    public GameObject particlePrefab;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ParticleManager.Instance.SpawnCustomParticles(particlePrefab, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
    }
}
