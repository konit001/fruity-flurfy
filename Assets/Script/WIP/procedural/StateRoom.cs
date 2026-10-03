using UnityEngine;
using System.Collections.Generic;

public enum RoomType { None, Normal, Boss, Treasure, Shop, Secret, Special, Rest }

[System.Serializable]
public class RoomData
{
    public RoomType roomType;
    public Sprite roomSprite;
    public Sprite IconRoomSprite;
}

public class StateRoom : MonoBehaviour
{
    [Header("Grid Size")]
    public Vector2Int gridSize = new Vector2Int(1, 1);

    [Header("Room Settings")]
    public RoomData _roomData;

    [Header("Transition Waypoints")]
    public Transform[] northWaypoint;
    public Transform[] eastWaypoint;
    public Transform[] southWaypoint;
    public Transform[] westWaypoint;

    [Header("Door Sockets / Walls")]
    public GameObject[] northWalls;
    public GameObject[] southWalls;
    public GameObject[] eastWalls;
    public GameObject[] westWalls;

    public void UpdateDoors(Dictionary<Vector2Int, StateRoom> gridMap, Vector2Int startPos, StateGenerate generator)
    {
        // North
        for (int x = 0; x < gridSize.x; x++)
        {
            Vector2Int checkPos = startPos + new Vector2Int(x, gridSize.y);
            if (gridMap.TryGetValue(checkPos, out StateRoom neighbor) && neighbor != this)
            {
                if (x < northWalls.Length && northWalls[x] != null) northWalls[x].SetActive(false);

                int neighborX = checkPos.x - generator.GetRoomStartPos(neighbor).x;
                if (x < northWaypoint.Length && northWaypoint[x] != null &&
                    neighborX < neighbor.southWaypoint.Length && neighbor.southWaypoint[neighborX] != null)
                {
                    // RoomWarp myWarp = northWaypoint[x].GetComponent<RoomWarp>();
                    // if (myWarp != null)
                    // {
                    //     myWarp.destination = neighbor.southWaypoint[neighborX];
                    //     myWarp.gameObject.SetActive(true);
                    // }
                }
            }
        }

        // South
        for (int x = 0; x < gridSize.x; x++)
        {
            Vector2Int checkPos = startPos + new Vector2Int(x, -1);
            if (gridMap.TryGetValue(checkPos, out StateRoom neighbor) && neighbor != this)
            {
                if (x < southWalls.Length && southWalls[x] != null) southWalls[x].SetActive(false);

                int neighborX = checkPos.x - generator.GetRoomStartPos(neighbor).x;
                if (x < southWaypoint.Length && southWaypoint[x] != null &&
                    neighborX < neighbor.northWaypoint.Length && neighbor.northWaypoint[neighborX] != null)
                {
                    // RoomWarp myWarp = southWaypoint[x].GetComponent<RoomWarp>();
                    // if (myWarp != null)
                    // {
                    //     myWarp.destination = neighbor.northWaypoint[neighborX];
                    //     myWarp.gameObject.SetActive(true);
                    // }
                }
            }
        }

        // East
        for (int y = 0; y < gridSize.y; y++)
        {
            Vector2Int checkPos = startPos + new Vector2Int(gridSize.x, y);
            if (gridMap.TryGetValue(checkPos, out StateRoom neighbor) && neighbor != this)
            {
                if (y < eastWalls.Length && eastWalls[y] != null) eastWalls[y].SetActive(false);

                int neighborY = checkPos.y - generator.GetRoomStartPos(neighbor).y;
                if (y < eastWaypoint.Length && eastWaypoint[y] != null &&
                    neighborY < neighbor.westWaypoint.Length && neighbor.westWaypoint[neighborY] != null)
                {
                    // RoomWarp myWarp = eastWaypoint[y].GetComponent<RoomWarp>();
                    // if (myWarp != null)
                    // {
                    //     myWarp.destination = neighbor.westWaypoint[neighborY];
                    //     myWarp.gameObject.SetActive(true);
                    // }
                }
            }
        }

        // West
        for (int y = 0; y < gridSize.y; y++)
        {
            Vector2Int checkPos = startPos + new Vector2Int(-1, y);
            if (gridMap.TryGetValue(checkPos, out StateRoom neighbor) && neighbor != this)
            {
                if (y < westWalls.Length && westWalls[y] != null) westWalls[y].SetActive(false);

                int neighborY = checkPos.y - generator.GetRoomStartPos(neighbor).y;
                if (y < westWaypoint.Length && westWaypoint[y] != null &&
                    neighborY < neighbor.eastWaypoint.Length && neighbor.eastWaypoint[neighborY] != null)
                {
                    // RoomWarp myWarp = westWaypoint[y].GetComponent<RoomWarp>();
                    // if (myWarp != null)
                    // {
                    //     myWarp.destination = neighbor.eastWaypoint[neighborY];
                    //     myWarp.gameObject.SetActive(true);
                    // }
                }
            }
        }
    }
}