using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    private Transform playerTransform; // Reference to the player's transform
    public float offsetAngle;

    private void Start()
    {
        playerTransform = PlayerManager.Instance.playerTrans;
    }

    private void Update()
    {
        if (playerTransform != null)
        {
            Vector2 directionToPlayer = playerTransform.position - transform.position;
            float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle + offsetAngle);
        }
    }
}
