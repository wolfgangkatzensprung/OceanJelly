using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    internal bool isChasing;
    public float shootingRadius = 10f;
    public float shootingInterval = 2f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;

    private Transform playerTransform;
    private bool isShooting = false;
    private float shootingTimer = 0f;

    private void Start()
    {
        playerTransform = PlayerManager.Instance.playerTrans;
    }

    private void Update()
    {
        if (isChasing && Vector3.Distance(transform.position, playerTransform.position) < shootingRadius)
        {
            if (!isShooting)
            {
                StartShooting();
            }
        }
        else
        {
            StopShooting();
        }

        if (isShooting)
        {
            shootingTimer += Time.deltaTime;
            if (shootingTimer >= shootingInterval)
            {
                ShootProjectile();
                shootingTimer = 0f;
            }
        }
    }

    private void StartShooting()
    {
        isShooting = true;
        shootingTimer = shootingInterval;
    }

    private void StopShooting()
    {
        isShooting = false;
        shootingTimer = 0f;
    }

    private void ShootProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
    }
}
