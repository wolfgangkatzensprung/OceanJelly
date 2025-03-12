using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlyphPuzzle : Singleton<GlyphPuzzle>
{
    public IceCave ic;

    [Tooltip("FrostfireBlade Item Prefab")]
    public GameObject frostfireBladePrefab;
    [Tooltip("Circular ForceField Prefab")]
    public GameObject forcefieldPrefab;

    [Tooltip("Blau, Pink, Orange, Gruen")]
    public GameObject[] glyphPrefabs; // Prefabs of the glyphs
    public GameObject glyphParent; // Parent object for the glyphs
    public GameObject[] slotPrefabs; // Prefabs of the slots
    public GameObject slotParent; // Parent object for the slots
    public GlyphSequenceLight[] crystalSequence;    // crystal sequence for correct order

    internal int sequenceIndex = 0;

    internal List<Glyph> collectedGlyphs = new List<Glyph>(); // List to store collected glyphs
    private float minX;
    private float maxX;
    private float minY;
    private float maxY;

    private Transform playerTrans;
    private GameObject forceField;

    public enum GlyphColor { Blau, Pink, Orange, Gruen }

    private void Start()
    {
        playerTrans = PlayerManager.Instance.playerTrans;

        StartCoroutine(StartPuzzle());

        ItemCollector.Instance.onItemCollected += StartBossfight;
    }

    private void StartBossfight(Item.Type itemType)
    {
        if (itemType == Item.Type.Sword)
        {
            if (forceField != null)
                Destroy(forceField);

            ic.StartBossfight();
        }
    }

    IEnumerator StartPuzzle()
    {
        yield return new WaitForEndOfFrame();

        SpawnGlyphsAndSlots();

    }

    internal void AddGlyph(Glyph glyph)
    {
        int currentIndex = sequenceIndex;

        if (!collectedGlyphs.Contains(glyph))
        {
            collectedGlyphs.Add(glyph);
        }
        if (!CheckGlyphMatchesCrystalSequence(glyph))
        {
            ResetGlyphs();
        }
        else
        {
            crystalSequence[currentIndex].Enable();

            if (currentIndex == 3)
            {
                Debug.Log($"All Glyphs placed in correct order");
                FinishPuzzle();
            }
        }
    }

    internal void ResetGlyphs()
    {
        Debug.Log($"Reset Glyphs");
        foreach (Glyph glyph in collectedGlyphs)
        {
            glyph.UnplaceFromSlot();
        }
        collectedGlyphs.Clear();

        ResetSequence();
    }

    // Check if the collected glyphs match the crystal sequence
    private bool CheckGlyphMatchesCrystalSequence(Glyph glyph)
    {
        Debug.Log($"CheckGlyphMatchesCrystalSequence: {glyph.glyphColor} Glyph for SequenceIndex {sequenceIndex} (color {crystalSequence[sequenceIndex].glyphColor})");
        if (glyph.glyphColor == crystalSequence[sequenceIndex].glyphColor)
        {
            sequenceIndex += 1;
            return true;
        }
        else
        {
            sequenceIndex = 0;
            return false;
        }
    }

    // Spawn glyphs and slots randomly across the room
    private void SpawnGlyphsAndSlots()
    {
        Vector2[] points = ic.GetPoints();

        // Shuffle the positions of the crystal sequence points
        ShuffleCrystalSequencePoints();

        // Spawn glyphs randomly
        SpawnObjectsRandomly(glyphPrefabs, points, glyphParent.transform);

        // Spawn slots randomly
        //SpawnObjectsRandomly(slotPrefabs, points, slotParent.transform);
        SpawnSlotPrefabs(slotPrefabs);

        // Get the center of the room
        Vector2 roomCenter = ic.transform.position;
        MoveFurthestToCenter(roomCenter);
        MoveFurthestToCenter(roomCenter);

        // Log the final positions of the glyphs and slots
        foreach (Transform glyphTransform in glyphParent.transform)
        {
            Debug.Log($"Final Glyph Position of {glyphTransform.name}: {glyphTransform.position}");
        }

        foreach (Transform slotTransform in slotParent.transform)
        {
            Debug.Log($"Final GlyphSlot Position of {slotTransform.name}: {slotTransform.position}");
        }
    }

    private void SpawnSlotPrefabs(GameObject[] slotPrefabs)
    {
        int prefabCount = slotPrefabs.Length;
        float angleStep = 180f / (prefabCount + 1);
        float radius = 32f; // Adjust this value to control the radius of the semicircle

        for (int i = 0; i < prefabCount; i++)
        {
            float angle = (i + 1) * angleStep;
            float radian = angle * Mathf.Deg2Rad;

            float xPos = playerTrans.position.x + radius * Mathf.Cos(radian);
            float yPos = playerTrans.position.y + radius * Mathf.Sin(radian);

            Vector2 spawnPosition = new Vector2(xPos, yPos);
            GameObject prefabInstance = Instantiate(slotPrefabs[i], spawnPosition, Quaternion.identity, slotParent.transform);
        }
    }


    private void MoveFurthestToCenter(Vector2 roomCenter)
    {
        // Move the furthest glyph to the center
        GameObject furthestGlyph = FindFurthestObject(glyphParent.transform, roomCenter);
        MoveObjectToCenter(furthestGlyph, roomCenter);

        //// Move the furthest slot to the center
        //GameObject furthestSlot = FindFurthestObject(slotParent.transform, roomCenter);
        //MoveObjectToCenter(furthestSlot, roomCenter);
    }

    // Spawn objects randomly within the specified boundaries
    private void SpawnObjectsRandomly(GameObject[] objectPrefabs, Vector2[] boundaries, Transform parentTransform)
    {
        foreach (GameObject prefab in objectPrefabs)
        {
            Vector2 randomPosition = GetRandomPosition(boundaries);
            Instantiate(prefab, randomPosition, Quaternion.identity, parentTransform);
            if (!IsPointInsideBounds(prefab.transform.position, boundaries))
            {
                MoveObjectToCenter(prefab, ic.transform.position);
            }
        }
    }

    // Find the furthest object from the specified center position
    private GameObject FindFurthestObject(Transform parentTransform, Vector2 center)
    {
        GameObject furthestObject = null;
        float maxDistance = 0f;

        foreach (Transform childTransform in parentTransform)
        {
            GameObject childObject = childTransform.gameObject;
            float distance = Vector2.Distance(childObject.transform.position, center);

            if (distance > maxDistance)
            {
                furthestObject = childObject;
                maxDistance = distance;
            }
        }

        return furthestObject;
    }


    // Move the object to the specified center position
    private void MoveObjectToCenter(GameObject obj, Vector3 center)
    {
        if (obj != null)
        {
            Vector3 randomPointOnSphere = Random.onUnitSphere * 100f;
            Vector3 adjustedPosition = center - randomPointOnSphere;
            Debug.Log($"MoveObjectToCenter() {obj.name} from {obj.transform.position} to {adjustedPosition}");
            obj.transform.position = (Vector2)adjustedPosition;
        }
    }

    private Vector2 GetRandomPosition(Vector2[] points)
    {
        int maxAttempts = 100; // Maximum number of attempts to find a valid position

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector2 randomPosition = GenerateRandomPointInBounds(points);
            Collider2D[] colliders = Physics2D.OverlapCircleAll(randomPosition, 10f); // Adjust the radius based on the size of your objects

            if (colliders.Length == 0)
            {
                Debug.Log("Random position found: " + randomPosition);
                return randomPosition;
            }
        }

        Debug.LogWarning("Failed to find a valid random position after " + maxAttempts + " attempts.");
        return Vector2.zero; // Return a default position if a valid position is not found
    }

    private Vector2 GenerateRandomPointInBounds(Vector2[] points)
    {
        minX = Mathf.Infinity;
        maxX = Mathf.NegativeInfinity;
        minY = Mathf.Infinity;
        maxY = Mathf.NegativeInfinity;
        float offset = 30f; // Adjust the offset as needed

        // Find the minimum and maximum X and Y values of the points
        foreach (Vector2 point in points)
        {
            minX = Mathf.Min(minX, point.x - offset);
            maxX = Mathf.Max(maxX, point.x + offset);
            minY = Mathf.Min(minY, point.y - offset);
            maxY = Mathf.Max(maxY, point.y + offset);
        }


        Debug.Log("minX: " + minX);
        Debug.Log("maxX: " + maxX);
        Debug.Log("minY: " + minY);
        Debug.Log("maxY: " + maxY);


        Vector2 randomPoint;
        do
        {
            randomPoint = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
            Debug.Log($"Try Random Position {randomPoint}");
        } while (!IsPointInsideBounds(randomPoint, points));

        return randomPoint;
    }

    private bool IsPointInsideBounds(Vector2 point, Vector2[] bounds)
    {
        int intersectionCount = 0;
        int vertexCount = bounds.Length;

        for (int i = 0; i < vertexCount; i++)
        {
            Vector2 vertex1 = bounds[i];
            Vector2 vertex2 = bounds[(i + 1) % vertexCount];

            if (((vertex1.y <= point.y && point.y < vertex2.y) || (vertex2.y <= point.y && point.y < vertex1.y)) &&
                (point.x < (vertex2.x - vertex1.x) * (point.y - vertex1.y) / (vertex2.y - vertex1.y) + vertex1.x))
            {
                intersectionCount++;
            }
        }

        return (intersectionCount % 2) == 1;
    }

    // Shuffle the crystal sequence points using Fisher-Yates algorithm
    private void ShuffleCrystalSequencePoints()
    {
        for (int i = crystalSequence.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            // Swap the crystalSequence array elements
            GlyphSequenceLight tempSequence = crystalSequence[i];
            crystalSequence[i] = crystalSequence[randomIndex];
            crystalSequence[randomIndex] = tempSequence;

            // Swap the corresponding transform.position values
            Vector2 tempPosition = crystalSequence[i].transform.position;
            crystalSequence[i].transform.position = crystalSequence[randomIndex].transform.position;
            crystalSequence[randomIndex].transform.position = tempPosition;
        }
        // Print the updated order
        Debug.Log("Updated Crystal Sequence Order:");
        for (int i = 0; i < crystalSequence.Length; i++)
        {
            Debug.Log($"GlyphSequenceLight {i}: {crystalSequence[i].glyphColor}");
        }
    }


    private void FinishPuzzle()
    {
        MusicManager.Instance.PlayMusic("FrostfireBlade");
        TimeManager.Instance.StartSlowMotion();

        Vector2 spawnPos = (Vector2)transform.position - Vector2.up * 40f;

        Instantiate(frostfireBladePrefab, spawnPos, Quaternion.identity);
        forceField = Instantiate(forcefieldPrefab, spawnPos, Quaternion.identity);
        ParticleManager.Instance.SpawnParticles("BubbleBurst", spawnPos, Quaternion.identity);

        DisableGlyphLights();

        slotParent.SetActive(false);
        glyphParent.SetActive(false);
    }

    private void ResetSequence()
    {
        foreach (GlyphSequenceLight gsl in crystalSequence)
        {
            gsl.Disable();
        }
        sequenceIndex = 0;
    }
    private void DisableGlyphLights()
    {
        foreach (GlyphSequenceLight gsl in crystalSequence)
        {
            gsl.gameObject.SetActive(false);
        }
    }
}
