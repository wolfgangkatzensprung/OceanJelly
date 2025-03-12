using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Rigidbody2D), typeof(Collider2D))]
public class Mermaid : MonoBehaviour
{
    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator anim;
    Collider2D col;

    [Header("Variations")]
    public MermaidVariation[] variations;
    public enum Variation { red = 0, blue = 1, yellow = 2 }
    internal Variation variation;

    [Header("Settings")]
    public Vector2 directionChangeTimeRange = new Vector2(1f, 5f);
    public float minVelocity = 5f;
    public float maxVelocity = 90f;
    public float smoothTime = 0.1f;

    public float noiseScale = 0.5f;
    public float noiseSpeed = 1f;

    [Tooltip("Mermaid will pause following when distance to player is too low (x), and stop following when distance to player is too high (y)")]
    public Vector2 playerDistanceLimit = new Vector2(7f, 30f);

    public float slowdownDistance = 5f;

    bool following;
    bool startFlipX;    // state of SpriteRenderer Flip variable for x, at Start
    public float wavyMotionSpeed = 3f;
    public float wavyMotionAmplitude = 3f;
    private Vector2 targetPosition;

    [Header("Gizmos for TargetPos")]
    public float gizmosSize = 3f;
    public Color gizmosColor = Color.white;

    private void OnEnable()
    {
        GameController.Instance.onDungeonFinished += FinishEscort;
    }
    private void OnDisable()
    {
        GameController.Instance.onDungeonFinished -= FinishEscort;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();

        startFlipX = sr.flipX;

        SetRandomHaircolor();
    }

    private void SetRandomHaircolor()
    {
        Variation rndVariation = GetRandomVariation();
        Debug.Log($"Random Mermaid Variation: {(int)rndVariation}");
        sr.sprite = variations[(int)rndVariation].mermaidSprite;
        variation = rndVariation;
    }

    private void FixedUpdate()
    {
        if (!following)
            return;

        FollowMovement();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            col.enabled = false;
            TriggerFollow();
        }
    }
    private void FinishEscort()
    {
        if (following && PlayerHealth.Instance.alive)
        {
            GameProgress.Instance.AddArtifact(variation);
        }
    }

    private void FollowMovement()
    {
        Vector2 direction = CombatManager.Instance.GetDirectionToPlayer(transform.position);

        if (direction.magnitude < playerDistanceLimit.x || (direction.magnitude < playerDistanceLimit.x * 2f && Mathf.Abs(CombatManager.Instance.GetPlayerVelocity().magnitude) < 3f))    // wenn player zu nah dran ist oder wenn er recht nah dran aber zu langsam ist
        {
            return;
        }

        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        if (direction.magnitude > playerDistanceLimit.y || screenPosition.x < 0 || screenPosition.x > Screen.width || screenPosition.y < 0 || screenPosition.y > Screen.height) // wenn player zu weit weg oder mermaid out of screen
        {
            StopFollow();
            return;
        }

        direction.Normalize();

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.identity;

        if (direction.x < 0)
        {
            rot = Quaternion.AngleAxis(angle + 180, Vector3.forward);
            if (!sr.flipX)
            {
                sr.flipX = true;
            }
        }
        else if (direction.x > 0)
        {
            rot = Quaternion.AngleAxis(angle, Vector3.forward);
            if (sr.flipX)
            {
                sr.flipX = false;
            }
        }
        rb.SetRotation(rot);

        Vector2 perlinOffset = GetPerlinOffset();


        targetPosition = (Vector2)PlayerManager.Instance.playerPosition + perlinOffset;

        float distanceToTargetPos = Vector2.Distance(rb.position, targetPosition);

        if (distanceToTargetPos <= slowdownDistance)
        {
            float slowdownFactor = distanceToTargetPos / slowdownDistance;
            targetPosition = Vector2.Lerp(rb.position, targetPosition, slowdownFactor);
        }

        float verticalOffset = Mathf.Sin(Time.time * wavyMotionSpeed) * wavyMotionAmplitude;
        Vector2 newPosition = new Vector2(targetPosition.x, targetPosition.y + verticalOffset);

        Vector2 smoothDampVelocity = Vector2.zero;
        newPosition = Vector2.SmoothDamp(rb.position, newPosition, ref smoothDampVelocity, smoothTime, maxVelocity);

        if (smoothDampVelocity.magnitude < minVelocity)
        {
            smoothDampVelocity = smoothDampVelocity.normalized * minVelocity;
        }

        rb.MovePosition(newPosition);
    }

    private Vector2 GetPerlinOffset()
    {
        float time = Time.time * noiseSpeed;
        float perlinOffsetX = Mathf.PerlinNoise(time, 0f) * 2f - 1f; // Adjust the scale to cover the range [-1, 1]
        float perlinOffsetY = Mathf.PerlinNoise(0f, time) * 2f - 1f; // Adjust the scale to cover the range [-1, 1]
        return new Vector2(perlinOffsetX, perlinOffsetY) * noiseScale;
    }

    private void TriggerFollow()
    {
        Debug.Log($"{gameObject} starts following player");
        QuestManager.Instance.MermaidFound();
        following = true;
        ResetSpriteRenderer();
        rb.velocity = Vector2.zero;
    }
    private void StopFollow()
    {
        Debug.Log($"{gameObject} stops following player");
        following = false;
        ResetSpriteRenderer();
        rb.velocity = Vector2.zero;
        col.enabled = true;
    }

    private void ResetSpriteRenderer()
    {
        sr.flipX = startFlipX;
    }

    public Variation GetRandomVariation()
    {
        // Create a list of available variations by removing the excluded variations
        List<Variation> availableVariations = new List<Variation>();
        foreach (Variation variant in Enum.GetValues(typeof(Variation)))
        {
            if (!GameProgress.Instance.artifacts.Contains(variant))
            {
                Debug.Log($"{variant} available for random color");
                availableVariations.Add(variant);
            }
        }

        // Check if there are any available variations left
        if (availableVariations.Count == 0)
        {
            Debug.LogWarning("No available variations to choose from.");
            return Variation.red; // Return a default variation or handle it as per your requirement
        }

        // Shuffle the available variations
        ShuffleList(availableVariations);

        // Get the first variation from the shuffled list
        Variation randomVariation = availableVariations[0];

        return randomVariation;
    }

    private void ShuffleList<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }


    private void OnDrawGizmos()
    {
        if (targetPosition != null)
        {
            Debug.Log("Gizmos");
            Gizmos.color = gizmosColor;
            Gizmos.DrawSphere(targetPosition, gizmosSize);
        }
    }
}

[System.Serializable]
public struct MermaidVariation
{
    public Mermaid.Variation mermaidVariation;
    public Sprite mermaidSprite;
}