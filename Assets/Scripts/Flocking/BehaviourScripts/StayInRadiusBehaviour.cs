using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/Stay In Radius")]
public class StayInRadiusBehaviour : FlockBehaviour
{
    public float radius = 15f;
    Vector2 flockSpawnPosition;

    public override Vector2 CalculateMove(FlockAgent agent, List<Transform> context, Flocking flock)
    {
        if (flockSpawnPosition == null)
            flockSpawnPosition = EnvironmentManager.Instance.GetLastFlockSpawnPosition();
        Vector2 centerOffset = flockSpawnPosition - (Vector2)agent.transform.position;
        float t = centerOffset.magnitude / radius;
        if (t < 0.9f)
        {
            return Vector2.zero;
        }

        return centerOffset * t * t;
    }
}
