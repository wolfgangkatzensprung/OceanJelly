using System;
using System.Collections.Generic;
using UnityEngine;

public class Flocking : MonoBehaviour
{
    public FlockAgent agentPrefab;
    List<FlockAgent> agents = new List<FlockAgent>();
    public FlockBehaviour behaviour;

    [Range(10, 500)] public int startingCount = 150;
    const float AgentDensity = .5f;

    [Range(1f, 30f)] public float driveFactor = 10f;
    [Range(1f, 100f)] public float maxSpeed = 10f;
    [Range(1f, 50f)] public float neighborRadius = 1.5f;
    [Range(0f, 1f)] public float avoidanceRadiusMultiplier = 0.5f;

    [Tooltip("Instantly spawns Flock without waiting for trigger")] public bool instaActivate;

    float squareMaxSpeed;
    float squareNeighborRadius;
    float squareAvoidanceRadius;

    public Color[] customColors = new Color[3];

    public float SquareAvoidanceRadius { get { return squareAvoidanceRadius; } }

    void Start()
    {
        squareMaxSpeed = maxSpeed * maxSpeed;
        squareNeighborRadius = neighborRadius * neighborRadius;
        squareAvoidanceRadius = squareNeighborRadius * avoidanceRadiusMultiplier * avoidanceRadiusMultiplier;

        if (instaActivate)
        {
            SpawnFlock();
        }

        SetColors();
    }

    void Update()
    {
        foreach (FlockAgent agent in agents)
        {
            if (agent != null)
            {
                List<Transform> context = GetNearbyObjects(agent);

                Vector2 move = behaviour.CalculateMove(agent, context, this);
                move *= driveFactor;
                if (move.sqrMagnitude > squareMaxSpeed)
                {
                    move = move.normalized * maxSpeed;
                    agent.Move(move);
                }
            }
        }
    }

    List<Transform> GetNearbyObjects(FlockAgent agent)
    {
        List<Transform> context = new List<Transform>();
        Collider2D[] contextColliders = Physics2D.OverlapCircleAll(agent.transform.position, neighborRadius);
        foreach (Collider2D c in contextColliders)
        {
            if (c != agent.AgentCollider)
            {
                context.Add(c.transform);
            }
        }
        return context;
    }

    private void SetColors()
    {
        foreach (FlockAgent agent in agents)
        {
            if (agent != null)
            {
                if (customColors != null)
                {
                    List<Transform> context = GetNearbyObjects(agent);

                    Color randomColor = customColors[UnityEngine.Random.Range(0, Mathf.Max(customColors.Length - 1, 0))];
                    agent.GetComponentInChildren<SpriteRenderer>().color = Color.Lerp(Color.white, randomColor, context.Count / 6f);
                }
            }
        }
    }

    public void SpawnFlock()
    {
        for (int i = 0; i < startingCount; i++)
        {
            FlockAgent newAgent = Instantiate
            (
                agentPrefab,
                transform.position + (Vector3)(UnityEngine.Random.insideUnitCircle * startingCount * AgentDensity),
                Quaternion.Euler(Vector3.forward * UnityEngine.Random.Range(0f, 360f)),
                transform
            );
            newAgent.name = "Agent " + i;
            agents.Add(newAgent);
        }

        EnvironmentManager.Instance.UpdateLastFlockSpawnPosition(transform.position);
    }
}
