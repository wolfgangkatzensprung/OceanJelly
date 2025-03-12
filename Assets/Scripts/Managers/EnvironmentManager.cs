using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    public static EnvironmentManager _instance;
    public static EnvironmentManager Instance { get { return _instance; } }

    public LayerMask whatIsGround { get; private set; }
    public LayerMask whatIsLiquo { get; private set; }

    Vector2 lastFlockSpawnPosition = Vector2.zero;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }

    public Vector2 GetLastFlockSpawnPosition()
    {
        Debug.Log("LastFlockSpawnPosition: " + lastFlockSpawnPosition);
        return lastFlockSpawnPosition;
    }
    public void UpdateLastFlockSpawnPosition(Vector3 flockSpawnPos)
    {
        Debug.Log("UpdateLastFlockSpawnPosition to " + flockSpawnPos);
        lastFlockSpawnPosition = flockSpawnPos;
    }
}
