using UnityEngine;
using UnityEngine.Tilemaps;

public class PerlinNoiseTilemap : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase[] tiles;
    public Vector2Int gridSize;
    public float noiseScale = 0.1f;
    public float threshold = 0.5f;

    void Start()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                float noiseValue = Mathf.PerlinNoise((float)x * noiseScale, (float)y * noiseScale);

                if (noiseValue < threshold)
                {
                    //tilemap.SetTile(new Vector3Int(x, y, 0), tiles[0]);
                }
                else
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), tiles[1]);
                }
            }
        }
    }
}
