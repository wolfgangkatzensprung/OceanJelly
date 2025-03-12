using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MazeGenerator : MonoBehaviour
{
    [Header("Maze Settings")]
    public Tilemap tilemap;
    public TileBase[] mazeTiles;
    public Vector2Int gridSize;
    internal int[,] grid;
    private Vector2Int currentCell;

    [Header("Perlin Noise")]
    public float noiseScale = 0.3f;
    public float noiseThreshold = 0.5f;

    [Header("Smoothing")]
    [Tooltip("Tiles with this many surrounding tiles will be replaced with 0")]
    public int maxSurroundingTiles = 4;

    [Header("Rooms")]
    public int minRoomSize = 3;
    public int maxRoomSize = 12;
    public int numRooms = 7;
    internal List<Vector2Int> roomCenters;

    [Header("Pathfinding")]
    [Tooltip("So viele freie Felder vom Spawn aus soll Player in eine Richtung gehen können")]
    public int distance = 23;
    private bool[,] visited;
    public int maxIterations = 5;
    int pathfindingIterations = 0;  // how many times has been checked for a free path 

    [Header("Exit")]
    [Tooltip("Exit Radius in Tiles")]
    public int exitRadius = 5;

    void Start()
    {
        roomCenters = new List<Vector2Int>();

        CreateMaze();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            CreateMaze();
        }
    }

    private void CreateMaze()
    {
        tilemap.ClearAllTiles();
        pathfindingIterations = 0;

        // Initialize the grid
        grid = new int[gridSize.x, gridSize.y];
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                grid[x, y] = 1;
            }
        }

        // Starting point
        currentCell = new Vector2Int(gridSize.x / 2, gridSize.y / 2);   // startet in der Mitte

        CreateRooms();

        Perlin();
        SmoothGrid();

        CreateStartArea();
        FreePath(currentCell);

        CreateBorder();
        GenerateExit();

        CheckCompletionOfTilemap();

        DrawTilemap();
    }

    void CheckCompletionOfTilemap()
    {
        //Astar.?
    }

    /// <summary>
    /// When there is no free path, a new path will be created. Then check again if path is free, etc. Gets bigger every time.
    /// </summary>
    /// <param name="startPos"></param>
    private void CreateEntrancePath(Vector2Int startPos)   // Creates a Path from StartArea to a random end point
    {
        if (pathfindingIterations > maxIterations)
            return;

        Vector2Int start = startPos + new Vector2Int(-3 - pathfindingIterations, -3 - pathfindingIterations);
        Vector2Int end = startPos + new Vector2Int(2 + pathfindingIterations, 2 + pathfindingIterations);
        for (int x = start.x; x <= end.x; x++)
        {
            for (int y = start.y; y <= end.y; y++)
            {
                grid[x, y] = 0;
            }
        }
        pathfindingIterations += 1;
        Debug.Log($"Tried to create a free path {pathfindingIterations} times");
        FreePath(startPos);
    }

    /// <summary>
    /// Checks if player has a free path. If not, call CreateEntrancePath().
    /// </summary>
    /// <param name="startPos"></param>
    public void FreePath(Vector2Int startPos)
    {
        // Create a queue to store the cells to be visited
        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        // Create a 2D array to store the distance from the starting cell for each cell in the grid
        int[,] distances = new int[grid.GetLength(0), grid.GetLength(1)];

        // Enqueue the starting cell and set its distance to 0
        queue.Enqueue(new Vector2Int(startPos.x, startPos.y));
        distances[startPos.x, startPos.y] = 0;


        Vector2Int cell = new Vector2Int();

        // While the queue is not empty
        while (queue.Count > 0)
        {
            // Dequeue the next cell
            cell = queue.Dequeue();

            // Check the distance of the current cell
            if (distances[cell.x, cell.y] > distance)
            {
                continue;
            }

            int[] dx = { -1, 1, 0, 0 };
            int[] dy = { 0, 0, -1, 1 };
            for (int i = 0; i < 4; i++)
            {
                // Calculate the coordinates of the neighboring cell
                int neighborX = cell.x + dx[i];
                int neighborY = cell.y + dy[i];

                // Check if the neighboring cell is within the grid boundaries
                if (neighborX >= 0 && neighborX < grid.GetLength(0) && neighborY >= 0 && neighborY < grid.GetLength(1))
                {
                    // Check if the neighboring cell is a free cell and if it has not been visited yet
                    if (grid[neighborX, neighborY] == 0 && distances[neighborX, neighborY] == 0)
                    {
                        // Enqueue the neighboring cell and update its distance
                        queue.Enqueue(new Vector2Int(neighborX, neighborY));
                        distances[neighborX, neighborY] = distances[cell.x, cell.y] + 1;
                    }
                }
            }
        }


        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                if (distances[i, j] >= distance)
                {
                    Debug.Log("Path is free");
                    return;
                }
            }
        }

        Debug.Log($"Path is blocked.");
        CreateEntrancePath(cell);   // mach Weg frei von letzter position
    }

    private void CreateStartArea()
    {
        // rectangle filled with 0
        Vector2Int start = currentCell + new Vector2Int(-3, -3);
        Vector2Int end = currentCell + new Vector2Int(2, 2);
        for (int x = start.x; x <= end.x; x++)
        {
            for (int y = start.y; y <= end.y; y++)
            {
                grid[x, y] = 0;
            }
        }
    }


    void SmoothGrid()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                int surroundingTiles = GetSurroundingTilesCount(x, y);

                if (surroundingTiles < maxSurroundingTiles)
                {
                    grid[x, y] = 0;
                }
            }
        }
    }
    private void DrawTilemap()
    {
        // Draw the grid on the tilemap
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                if (grid[x, y] == 1)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), mazeTiles[0]);
                }
                else if (grid[x, y] == 2)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), mazeTiles[1]);
                }
                else if (grid[x, y] == 3)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), mazeTiles[2]);
                }
            }
        }
    }

    List<Vector2Int> CreateRooms()
    {
        // Create a list to store the positions of all rooms
        List<Vector2Int> roomPositions = new List<Vector2Int>();

        // Generate the first room
        GenerateRoom(roomPositions);

        return roomPositions;
    }

    void GenerateRoom(List<Vector2Int> roomPositions)
    {
        // Generate a random room size
        int roomWidth = Random.Range(minRoomSize, maxRoomSize + 1);
        int roomHeight = Random.Range(minRoomSize, maxRoomSize + 1);

        // Generate a random room position that does not overlap with any existing rooms
        Vector2Int roomPos = new Vector2Int(Random.Range(1, gridSize.x - roomWidth), Random.Range(1, gridSize.y - roomHeight));

        bool validPos = true;

        foreach (Vector2Int pos in roomPositions)
        {
            if (Mathf.Abs(pos.x - roomPos.x) < roomWidth && Mathf.Abs(pos.y - roomPos.y) < roomHeight)
            {
                validPos = false;
                break;
            }
        }

        if (!validPos)
        {
            // If the position is not valid, try again
            GenerateRoom(roomPositions);
        }
        else
        {
            // Carve out the room by clearing the tiles within the room boundaries
            for (int x = roomPos.x; x < roomPos.x + roomWidth; x++)
            {
                for (int y = roomPos.y; y < roomPos.y + roomHeight; y++)
                {
                    grid[x, y] = 0;
                }
            }

            roomPositions.Add(roomPos);
            Vector2Int roomCenter = new Vector2Int(roomPos.x + roomWidth / 2, roomPos.y + roomHeight / 2);
            roomCenters.Add(roomCenter);

            // If there are still rooms to generate, call GenerateRoom again
            if (roomPositions.Count < numRooms)
            {
                GenerateRoom(roomPositions);
            }
        }
    }

    private void GenerateExit()
    {
        int exitSide = Random.Range(0, 4); // 0: left, 1: top, 2: right, 3: bottom

        Vector2Int exitPoint = Vector2Int.zero;
        switch (exitSide)
        {
            case 0: // Left
                exitPoint = new Vector2Int(0, Random.Range(0, gridSize.y));
                break;
            case 1: // Top
                exitPoint = new Vector2Int(Random.Range(0, gridSize.x), gridSize.y);
                break;
            case 2: // Right
                exitPoint = new Vector2Int(gridSize.x, Random.Range(0, gridSize.y));
                break;
            case 3: // Bottom
                exitPoint = new Vector2Int(Random.Range(0, gridSize.x), 0);
                break;
        }

        Dungeon.Instance.exitPoint = tilemap.CellToWorld((Vector3Int)exitPoint);

        // Draw the exit circle
        for (int x = exitPoint.x - exitRadius; x <= exitPoint.x + exitRadius; x++)
        {
            for (int y = exitPoint.y - exitRadius; y <= exitPoint.y + exitRadius; y++)
            {
                if (x >= gridSize.x || y >= gridSize.y || x < 0 || y < 0)
                {
                    continue;
                }
                int distance = Mathf.RoundToInt(Vector2Int.Distance(new Vector2Int(x, y), exitPoint));
                if (distance <= exitRadius && distance >= exitRadius - 1)
                {
                    Debug.Log($"Place Exit Tile at {x},{y}");
                    grid[x, y] = 3;
                }
                else if (distance < exitRadius - 1)
                {
                    grid[x, y] = 0;
                }
            }
        }

    }

    void Perlin()
    {
        float rndOffset = Random.Range(-1f, 1f);

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                float noiseValue = Mathf.PerlinNoise((float)x * noiseScale + rndOffset, (float)y * noiseScale + rndOffset);

                if (!(noiseValue < noiseThreshold))
                {
                    grid[x, y] = 0;
                }
            }
        }
    }

    int GetSurroundingTilesCount(int x, int y)
    {
        int tileCount = 0;
        for (int neighbourX = x - 1; neighbourX <= x + 1; neighbourX++)
        {
            for (int neighbourY = y - 1; neighbourY <= y + 1; neighbourY++)
            {
                if (neighbourX >= 0 && neighbourX < gridSize.x && neighbourY >= 0 && neighbourY < gridSize.y)
                {
                    if (neighbourX != x || neighbourY != y)
                    {
                        if (grid[neighbourX, neighbourY] != 0)
                        {
                            tileCount++;
                        }
                    }
                }
                else
                {
                    tileCount++;
                }
            }
        }
        //Debug.Log($"TileCount {x},{y}: {tileCount}");
        return tileCount;
    }

    void CreateBorder()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                if (x == 0 || x == gridSize.x - 1 || y == 0 || y == gridSize.y - 1)
                {
                    grid[x, y] = 2;
                }
            }
        }
    }
}
