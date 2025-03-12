using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonGenerator : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase dungeonTile;
    public Vector2Int gridSize;
    private int[,] grid;
    private Vector2Int currentCell;
    private List<Vector2Int> visitedCells;
    private Vector2Int startPoint;
    private Vector2Int endPoint;
    private int numberOfRooms;
    private List<Rect> rooms;
    public int minRoomSize = 4;
    public int maxRoomSize = 10;

    void Start()
    {
        // Initialize the grid
        grid = new int[gridSize.x, gridSize.y];

        // Initialize the list of visited cells
        visitedCells = new List<Vector2Int>();

        // Initialize the list of rooms
        rooms = new List<Rect>();

        // Create the start point
        startPoint = new Vector2Int(Random.Range(1, gridSize.x - 1), Random.Range(1, gridSize.y - 1));
        grid[startPoint.x, startPoint.y] = 0;
        visitedCells.Add(startPoint);

        // Create the end point
        endPoint = new Vector2Int(Random.Range(1, gridSize.x - 1), Random.Range(1, gridSize.y - 1));
        grid[endPoint.x, endPoint.y] = 0;
        visitedCells.Add(endPoint);

        // Create the rooms
        numberOfRooms = Random.Range(3, 7);
        CreateRooms();

        // Connect the rooms
        ConnectRooms();

        // Draw the grid on the tilemap
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                if (grid[x, y] == 0)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), dungeonTile);
                }
            }
        }
    }
    void CreateRooms()
    {
        for (int i = 0; i < numberOfRooms; i++)
        {
            // Generate random room size
            int roomWidth = Random.Range(minRoomSize, maxRoomSize);
            int roomHeight = Random.Range(minRoomSize, maxRoomSize);

            // Generate random room position
            int roomX = Random.Range(1, gridSize.x - roomWidth - 1);
            int roomY = Random.Range(1, gridSize.y - roomHeight - 1);

            // Create the room
            Rect room = new Rect(roomX, roomY, roomWidth, roomHeight);
            rooms.Add(room);

            // Clear the room on the grid
            for (int x = (int)room.xMin; x < (int)room.xMax; x++)
            {
                for (int y = (int)room.yMin; y < (int)room.yMax; y++)
                {
                    grid[x, y] = 0;
                    visitedCells.Add(new Vector2Int(x, y));
                }
            }
        }
    }

    void ConnectRooms()
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            // Find the closest room to connect to
            int closestRoom = -1;
            float closestDistance = float.MaxValue;
            for (int j = 0; j < rooms.Count; j++)
            {
                if (i == j)
                {
                    continue;
                }

                float distance = Vector2.Distance(rooms[i].center, rooms[j].center);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestRoom = j;
                }
            }

            // Connect the room to the closest room
            if (closestRoom != -1)
            {
                Vector2Int start = new Vector2Int((int)rooms[i].center.x, (int)rooms[i].center.y);
                Vector2Int end = new Vector2Int((int)rooms[closestRoom].center.x, (int)rooms[closestRoom].center.y);
                ConnectRooms(start, end);
            }
        }
    }

    void ConnectRooms(Vector2Int start, Vector2Int end)
    {
        while (start.x != end.x || start.y != end.y)
        {
            grid[start.x, start.y] = 0;
            visitedCells.Add(start);

            if (start.x < end.x)
            {
                start.x++;
            }
            else if (start.x > end.x)
            {
                start.x--;
            }
            else if (start.y < end.y)
            {
                start.y++;
            }
            else if (start.y > end.y)
            {
                start.y--;
            }
        }
    }
}