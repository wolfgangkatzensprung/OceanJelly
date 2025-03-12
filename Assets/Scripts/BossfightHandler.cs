using System.Collections.Generic;
using UnityEngine;

public class BossfightHandler : MonoBehaviour
{
    private Transform playerTransform; // Reference to the player's transform
    public Transform bgImageTrans;
    public GameObject iceShardPrefab; // Reference to the projectile prefab
    public GameObject sentinelPrefab;

    private List<GameObject> iceShards = new List<GameObject>(); // List of in-game projectile game objects
    private List<GameObject> sentinels = new List<GameObject>(); // List of in-game projectile game objects

    public int iceShardCount = 3;
    public int iceSentinelCount = 3;

    public float circleRadius = 15f; // Radius of the circle formation
    public float circleRadiusOffset = 15f; // Additional Radius for Ice Sentinels

    public float rotationSpeed = 30f; // Rotation speed of the projectiles
    private float startRotationSpeed = 0f;
    private float rotationOffset = 0f; // Offset for angle steps to move along the circle
    public float forceStrength = 1f;

    [Header("Background vibration for Bossfight Start")]

    public float duration = 1f; // Duration of the vibration
    public float intensity = 0.1f; // Intensity of the vibration
    public float speed = 1f; // Speed of the vibration

    private Vector3 bgPos;
    private float timer = 0f;
    private float maxVelocity = 80f;

    int bossfightPhase = 0;

    private void Start()
    {
        playerTransform = PlayerManager.Instance.playerTrans;
        bgPos = bgImageTrans.position;
        startRotationSpeed = rotationSpeed;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            StartPhase1();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            StartPhase2();
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            AddIceShard();
        }


        if (timer < duration)
        {
            timer += Time.deltaTime;

            float x = bgPos.x + Mathf.PerlinNoise(Time.time * speed, 0f) * intensity;
            float y = bgPos.y + Mathf.PerlinNoise(0f, Time.time * speed) * intensity;
            float z = bgPos.z;

            transform.position = new Vector3(x, y, z);
        }
        else
        {
            transform.position = bgPos;
        }
    }

    private void FixedUpdate()
    {
        MoveIceShards();
        if (bossfightPhase > 0)
        {
            MoveSentinels();
        }
    }

    public void InitializeIceShards()
    {
        float angleStep = 360f / iceShardCount;

        for (int i = 0; i < iceShardCount; i++)
        {
            float angle = i * angleStep;
            Vector2 offset = Quaternion.Euler(0f, 0f, angle) * Vector2.up * circleRadius;
            GameObject iceShard = Instantiate(iceShardPrefab, playerTransform.position + (Vector3)offset * 5f, Quaternion.identity);
            iceShards.Add(iceShard);
        }
    }
    public void InitializeIceSentinels()
    {
        float angleStep = 360f / iceSentinelCount;

        for (int i = 0; i < iceSentinelCount; i++)
        {
            float angle = i * angleStep;
            Vector2 offset = Quaternion.Euler(0f, 0f, angle) * Vector2.up * (circleRadius + circleRadiusOffset);
            GameObject sentinel = Instantiate(sentinelPrefab, playerTransform.position + (Vector3)offset * 5f, Quaternion.identity);
            sentinels.Add(sentinel);
        }
    }

    public void AddIceShard()
    {
        float angleStep = 360f / iceShardCount;

        float angle = iceShards.Count * angleStep;
        Vector2 offset = Quaternion.Euler(0f, 0f, angle) * Vector2.up * circleRadius;
        GameObject iceShard = Instantiate(iceShardPrefab, playerTransform.position + (Vector3)offset * 5f, Quaternion.identity);
        //GameObject iceShard = Instantiate(iceShardPrefab, playerTransform.position + (Vector3)offset, Quaternion.identity);
        iceShards.Add(iceShard);
    }


    //private void MoveIceShards()
    //{
    //    float rotationStep = rotationSpeed * Time.fixedDeltaTime;
    //    float angleStep = 360f / iceShards.Count;

    //    rotationOffset += rotationStep; // Update the rotation offset

    //    float minRadius = 15f;
    //    float radius = minRadius + Mathf.PingPong(Time.time, circleRadius);

    //    GameObject[] iceShardsArray = iceShards.ToArray();
    //    for (int i = 0; i < iceShardsArray.Length; i++)
    //    {
    //        GameObject iceShard = iceShardsArray[i];
    //        float angle = i * angleStep + rotationOffset; // Add the rotation offset to the angle
    //        Vector2 offset = Quaternion.Euler(0f, 0f, angle) * (Vector2.up * radius);
    //        Vector2 targetPosition = (Vector2)playerTransform.position + offset;
    //        iceShard.transform.position = targetPosition;
    //    }
    //}

    private Vector2 smoothedPlayerPosition; // Smoothed player position
    private float smoothingFactor = 0.1f; // Smoothing factor (adjust as desired)
    private float velocityThreshold = 0.1f; // Velocity threshold for applying breaking force

    private void MoveIceShards()
    {
        float rotationStep = rotationSpeed * Time.fixedDeltaTime;
        float angleStep = 360f / iceShards.Count;

        rotationOffset += rotationStep; // Update the rotation offset

        float minRadius = 15f;
        float radius = minRadius + Mathf.PingPong(Time.time, circleRadius);

        // Smooth the player position using a low-pass filter
        Vector2 targetPlayerPosition = playerTransform.position;
        smoothedPlayerPosition = Vector2.Lerp(smoothedPlayerPosition, targetPlayerPosition, smoothingFactor);

        for (int i = 0; i < iceShards.Count; i++)
        {
            GameObject iceShard = iceShards[i];
            float angle = i * angleStep + rotationOffset; // Add the rotation offset to the angle
            Vector2 offset = Quaternion.Euler(0f, 0f, angle) * (Vector2.up * circleRadius);
            Vector2 targetPosition = smoothedPlayerPosition + offset;

            Rigidbody2D iceShardRb = iceShard.GetComponent<Rigidbody2D>();

            Vector2 direction = (targetPosition - (Vector2)iceShard.transform.position).normalized;
            Vector2 desiredVelocity = direction * maxVelocity;

            Vector2 velocityDifference = desiredVelocity - iceShardRb.velocity;

            if (velocityDifference.magnitude > velocityThreshold)
            {
                Vector2 breakingForce = velocityDifference / Time.fixedDeltaTime;
                iceShardRb.AddForce(breakingForce);
            }
        }
    }

    private void MoveSentinels()
    {
        float rotationStep = rotationSpeed * Time.fixedDeltaTime;
        float angleStep = 360f / sentinels.Count;

        rotationOffset += rotationStep; // Update the rotation offset

        float minRadius = 15f;
        float radius = minRadius + Mathf.PingPong(Time.time, circleRadius) + circleRadiusOffset;

        // Smooth the player position using a low-pass filter
        Vector2 targetPlayerPosition = playerTransform.position;
        smoothedPlayerPosition = Vector2.Lerp(smoothedPlayerPosition, targetPlayerPosition, smoothingFactor);

        for (int i = 0; i < sentinels.Count; i++)
        {
            GameObject sentinel = sentinels[i];
            float angle = i * angleStep + rotationOffset; // Add the rotation offset to the angle
            Vector2 offset = Quaternion.Euler(0f, 0f, angle) * (Vector2.up * circleRadius * 2f);
            Vector2 targetPosition = smoothedPlayerPosition + offset;

            Rigidbody2D sentinelRb = sentinel.GetComponent<Rigidbody2D>();

            Vector2 direction = (targetPosition - (Vector2)sentinel.transform.position).normalized;
            Vector2 desiredVelocity = direction * maxVelocity;

            Vector2 velocityDifference = desiredVelocity - sentinelRb.velocity;

            if (velocityDifference.magnitude > velocityThreshold)
            {
                Vector2 breakingForce = velocityDifference / Time.fixedDeltaTime;
                sentinelRb.AddForce(breakingForce);
            }
        }
    }

    internal void StartPhase1()
    {
        MusicManager.Instance.PlayMusic("IceCaveBossfight");
        timer = 0f;
        bossfightPhase = 1;
        InitializeIceShards();

    }

    internal void StartPhase2()
    {
        timer = 0f;
        bossfightPhase = 2;
        InitializeIceSentinels();
    }

}
