using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

/// <summary>
/// Spawns Objects inside the LevelSet
/// </summary>
public class LevelSet : MonoBehaviour
{
    [Tooltip("SpriteShapeControllers in this LevelSet")]
    public SpriteShapeController[] ssc;
    public SpriteShapeRenderer[] ssr;

    public Schatztruhe schatz;
    public Entrance eingang;

    [Tooltip("Prefabs to randomly chose from and spawn")]
    public GameObject[] prefabsToSpawn;
    public int numberOfObjectsToSpawn;
    private Vector2[] polygonPoints;
    private Vector2[] randomPointsInsidePolygon;
    private HashSet<Vector2> occupiedPositions = new HashSet<Vector2>();
    [Tooltip("Distance for RandomWalk; minimum distance between objects")]
    public float walkDistance = 15f;

    internal bool isFinished { get { return schatz.isLooted; } }

    private void Awake()
    {
        ssr = GetComponentsInChildren<SpriteShapeRenderer>();

        SetSpritesEnabled(false);
    }

    void Start()
    {
        ssc = GetComponentsInChildren<SpriteShapeController>();

        if (ssc.Length > 1)
        {
            polygonPoints = GetMultiBounds();
        }
        else
        {
            polygonPoints = GetSplineBounds();
        }
        randomPointsInsidePolygon = new Vector2[numberOfObjectsToSpawn];
        //polygonArea = CalculateArea(polygonPoints);
        //polygonCenter = CalculateCentroid(polygonPoints);

        SpawnInsideItems();
        StartCoroutine(RotateLevelSetNextFrame());
    }
    private void SetSpritesEnabled(bool b)
    {
        foreach (SpriteShapeRenderer s in ssr)
        {
            s.enabled = b;
        }
    }

    private void SpawnInsideItems()
    {
        for (int i = 0; i < numberOfObjectsToSpawn; i++)
        {
            Vector3 randomPosition = GetRandomPointInsidePolygon(polygonPoints);
            randomPointsInsidePolygon[i] = randomPosition;
            occupiedPositions.Add(randomPosition);
        }

        for (int i = 0; i < numberOfObjectsToSpawn; i++)
        {
            Vector3 randomPosition = randomPointsInsidePolygon[i];
            bool overlapping = true;
            int attempts = 0;
            while (overlapping)
            {
                Vector3 newPosition = RandomWalk(randomPosition);
                if (!occupiedPositions.Contains(newPosition))
                {
                    randomPosition = newPosition;
                    occupiedPositions.Add(newPosition);
                    overlapping = false;
                }

                attempts += 1;
                if (attempts > 30) continue;
            }
            GameObject o = Instantiate(prefabsToSpawn[Random.Range(0, prefabsToSpawn.Length)], randomPosition + transform.position, Quaternion.identity);
            o.transform.parent = transform;
        }
    }

    public Vector2[] GetMultiBounds()
    {
        List<Vector2> polygon = new List<Vector2>();

        for (int i = 0; i < ssc.Length; i++)
        {
            Vector2[] points = new Vector2[ssc[i].spline.GetPointCount()];
            for (int j = 0; j < points.Length; j++)
            {
                points[j] = ssc[i].spline.GetPosition(j) + ssc[i].transform.localPosition;
            }

            for (int k = points.Length - 1; k >= 0; k--)
            {
                polygon.Add(points[k]);
            }
        }

        //this will close the polygon by adding the first point of the first spline
        polygon.Add(ssc[0].spline.GetPosition(0));
        //returning the polygon as an array
        return polygon.ToArray();
    }

    public Vector2[] GetSplineBounds()
    {
        Vector2[] points = new Vector2[ssc[0].spline.GetPointCount()];
        for (int i = 0; i < ssc[0].spline.GetPointCount(); i++)
        {
            points[i] = ssc[0].spline.GetPosition(i);
        }
        return points;
    }

    IEnumerator RotateLevelSetNextFrame()   // rotates the level set after waiting for next frame (damit erst items gespawnt werden und erst dann rotiert wird)
    {
        yield return new WaitForEndOfFrame();
        transform.rotation = Quaternion.FromToRotation(-Vector2.up, PlayerManager.Instance.GetDirectionToPlayer(transform.position));
        SetSpritesEnabled(true);
    }

    public Vector3 GetRandomPointInsidePolygon(Vector2[] polygonPoints)
    {
        Vector3 randomPoint = new Vector2();
        float minX = polygonPoints[0].x;
        float maxX = polygonPoints[0].x;
        float minY = polygonPoints[0].y;
        float maxY = polygonPoints[0].y;

        for (int i = 0; i < polygonPoints.Length; i++)
        {
            if (polygonPoints[i].x < minX)
                minX = polygonPoints[i].x;
            if (polygonPoints[i].x > maxX)
                maxX = polygonPoints[i].x;
            if (polygonPoints[i].y < minY)
                minY = polygonPoints[i].y;
            if (polygonPoints[i].y > maxY)
                maxY = polygonPoints[i].y;
        }

        do
        {
            float randomX = Random.Range(minX, maxX);
            float randomY = Random.Range(minY, maxY);
            randomPoint = new Vector3(randomX, randomY, 0);
        } while (!IsPointInsidePolygon(randomPoint, polygonPoints));

        return randomPoint;
    }

    private bool IsPointInsidePolygon(Vector2 point, Vector2[] polygonPoints)
    {
        int i, j;
        bool c = false;
        int nvert = polygonPoints.Length;
        for (i = 0, j = nvert - 1; i < nvert; j = i++)
        {
            if (((polygonPoints[i].y > point.y) != (polygonPoints[j].y > point.y)) &&
            (point.x < (polygonPoints[j].x - polygonPoints[i].x) * (point.y - polygonPoints[i].y) / (polygonPoints[j].y - polygonPoints[i].y) + polygonPoints[i].x))
                c = !c;
        }
        return c;
    }

    private Vector3 RandomWalk(Vector3 startingPosition)
    {
        //Debug.Log("RandomWalk from " + startingPosition);

        Vector3 newPosition = startingPosition;
        float walkAngle = Random.Range(0, 360);
        newPosition.x += walkDistance * Mathf.Cos(walkAngle);
        newPosition.y += walkDistance * Mathf.Sin(walkAngle);
        if (!IsPointInsidePolygon(newPosition, polygonPoints))
        {
            newPosition = startingPosition;
        }
        return newPosition;
    }

    //private void OnDrawGizmos()
    //{
    //    DrawPolygon(polygonPoints);
    //    Gizmos.DrawWireCube(CalculateCentroid(polygonPoints), Vector2.one * 5f);

    //    Gizmos.DrawCube((Vector2)ssc[0].spline.GetPosition(0) + (Vector2)transform.position, Vector2.one * 5f);

    //}

    //private void DrawPolygon(Vector2[] v)
    //{
    //    Vector2 previousPos = transform.position;
    //    for (int i = 0; i < v.Length; i++)
    //    {
    //        Debug.Log($"Draw Gizmos for {v[i]} in {gameObject.name}");
    //        Gizmos.DrawLine(v[i] + (Vector2)transform.position, previousPos);
    //        previousPos = v[i] + (Vector2)transform.position;
    //    }
    //}
}