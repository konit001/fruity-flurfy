using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;

public class StateGenerate : MonoBehaviour
{
    public static StateGenerate instance;
    [Header("Map Settings")]
    public int minRoom = 5;
    public int maxRoom = 10;

    [Header("3D Room Dimensions (Size in Unity Units)")]
    public float roomWidth = 10f;
    public float roomDepth = 10f;

    [Header("Room Settings")]
    [SerializeField] private List<StateRoom> activeRooms = new List<StateRoom>();
    private Dictionary<Vector2Int, StateRoom> gridMap = new Dictionary<Vector2Int, StateRoom>();

    [Header("Layout Prefabs (3D Models)")]
    public StateRoom[] Rooms;
    public StateRoom startRoom;
    public StateRoom bossRoom;
    public StateRoom treasureRoom;
    public StateRoom shopRoom;
    public StateRoom secretRoom;
    public StateRoom specialRoom;
    public StateRoom restRoom;
    void Awake()
    {
        instance = this;
    }
    private static Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(0, 1),   // North 
        new Vector2Int(1, 0),   // East 
        new Vector2Int(0, -1),  // South 
        new Vector2Int(-1, 0)   // West 
    };

#if UNITY_EDITOR
    [Button("Start Play")]
    public void StartPlay()
    {
        GenerateMap();
        MapUiGenerate.instance.MapGenrerate();
    }
#endif

    public void GenerateMap()
    {
        ClearMap();

        Vector2Int currentGridPos = Vector2Int.zero;

        // 1. วางห้องเริ่มต้น (Start Room)
        PlaceRoomInstance(startRoom, currentGridPos);

        List<Vector2Int> availablePositions = new List<Vector2Int>();
        AddValidNeighbors(currentGridPos, startRoom.gridSize, availablePositions);

        int targetRoomCount = Random.Range(minRoom, maxRoom);

        while (activeRooms.Count < targetRoomCount && availablePositions.Count > 0)
        {
            int randomIndex = Random.Range(0, availablePositions.Count);
            Vector2Int nextPos = availablePositions[randomIndex];
            availablePositions.RemoveAt(randomIndex);

            StateRoom roomPrefabToUse = GetRandomRoomPrefab();

            if (CanPlaceRoom(nextPos, roomPrefabToUse.gridSize))
            {
                StateRoom newRoom = PlaceRoomInstance(roomPrefabToUse, nextPos);
                AddValidNeighbors(nextPos, newRoom.gridSize, availablePositions);
            }
        }

        PlaceBossRoom(availablePositions);
        UpdateAllRoomDoors();
    }

    // ฟังก์ชันสำหรับตรวจสอบว่าพื้นที่ว่างพอสำหรับขนาด gridSize หรือไม่
    private bool CanPlaceRoom(Vector2Int startPos, Vector2Int size)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                if (gridMap.ContainsKey(startPos + new Vector2Int(x, y)))
                    return false;
            }
        }
        return true;
    }

    private StateRoom PlaceRoomInstance(StateRoom prefab, Vector2Int startPos)
    {
        Vector3 worldPos = new Vector3(startPos.x * roomWidth, 0f, startPos.y * roomDepth);
        StateRoom newRoom = Instantiate(prefab, worldPos, Quaternion.identity, transform);
        activeRooms.Add(newRoom);
        MapUiGenerate.instance.MapCountSetup(newRoom);

        for (int x = 0; x < newRoom.gridSize.x; x++)
        {
            for (int y = 0; y < newRoom.gridSize.y; y++)
            {
                gridMap.Add(startPos + new Vector2Int(x, y), newRoom);
            }
        }

        return newRoom;
    }

    private void AddValidNeighbors(Vector2Int pos, Vector2Int size, List<Vector2Int> availableList)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector2Int cellPos = pos + new Vector2Int(x, y);
                foreach (var dir in directions)
                {
                    Vector2Int neighborPos = cellPos + dir;
                    if (!gridMap.ContainsKey(neighborPos) && !availableList.Contains(neighborPos))
                    {
                        availableList.Add(neighborPos);
                    }
                }
            }
        }
    }

    private StateRoom GetRandomRoomPrefab()
    {
        int rand = Random.Range(0, 100);
        if (rand < 15 && shopRoom != null) return shopRoom;
        if (rand < 30 && treasureRoom != null) return treasureRoom;
        if (rand < 45 && restRoom != null) return restRoom;

        if (Rooms != null && Rooms.Length > 0)
        {
            return Rooms[Random.Range(0, Rooms.Length)];
        }
        return startRoom;
    }

    private void PlaceBossRoom(List<Vector2Int> availablePositions)
    {
        if (bossRoom != null && availablePositions.Count > 0)
        {
            for (int i = availablePositions.Count - 1; i >= 0; i--)
            {
                Vector2Int bossPos = availablePositions[i];
                if (CanPlaceRoom(bossPos, bossRoom.gridSize))
                {
                    PlaceRoomInstance(bossRoom, bossPos);
                    break;
                }
            }
        }
    }

    // สั่งให้ทุกห้องอัปเดตการเปิด/ปิดกำแพง
    private void UpdateAllRoomDoors()
    {
        HashSet<StateRoom> processedRooms = new HashSet<StateRoom>();

        foreach (var kvp in gridMap)
        {
            StateRoom room = kvp.Value;
            if (!processedRooms.Contains(room))
            {
                room.UpdateDoors(gridMap, GetRoomStartPos(room) , this);
                processedRooms.Add(room);
            }
        }
    }

    public Vector2Int GetRoomStartPos(StateRoom room)
    {
        int x = Mathf.RoundToInt(room.transform.position.x / roomWidth);
        int y = Mathf.RoundToInt(room.transform.position.z / roomDepth);
        return new Vector2Int(x, y);
    }

    public void ClearMap()
    {
        foreach (var room in activeRooms)
        {
            if (room != null) Destroy(room.gameObject);
        }
        activeRooms.Clear();
        gridMap.Clear();

        if (MapUiGenerate.instance != null)
        {
            MapUiGenerate.instance.ClearMap();
            MapUiGenerate.instance.ClearRoomData();
        }
    }

    //
#if UNITY_EDITOR
    [Button("Auto Calculate Room Size")]
    public void AutoCalculateRoomSize()
    {
        if (startRoom == null)
        {
            Debug.LogWarning("กรุณาใส่ Start Room Prefab ก่อนครับ!");
            return;
        }

        // พยายามดึง BoxCollider จากห้องเริ่มต้นมาใช้เป็นตัววัดขนาด
        BoxCollider boxCol = startRoom.GetComponent<BoxCollider>();

        if (boxCol != null)
        {
            // ดึงค่ากว้างยาวจาก BoxCollider (คูณด้วย Scale เผื่อโมเดลถูกย่อ/ขยาย)
            roomWidth = boxCol.size.x * startRoom.transform.localScale.x;
            roomDepth = boxCol.size.z * startRoom.transform.localScale.z;

            Debug.Log($"[สำเร็จ] คำนวณขนาด 1 ช่อง Grid ได้: กว้าง {roomWidth}, ลึก {roomDepth}");
        }
        else
        {
            // ถ้าไม่มี BoxCollider ให้พยายามวัดจาก Mesh (โมเดล 3D) ชิ้นแรกที่เจอ
            Renderer rend = startRoom.GetComponentInChildren<Renderer>();
            if (rend != null)
            {
                roomWidth = rend.bounds.size.x;
                roomDepth = rend.bounds.size.z;
                Debug.Log($"[วัดจาก Mesh] คำนวณขนาด 1 ช่อง Grid ได้: กว้าง {roomWidth}, ลึก {roomDepth}");
            }
            else
            {
                Debug.LogError("ไม่พบ BoxCollider หรือ MeshRenderer ใน Start Room เลย ไม่สามารถคำนวณอัตโนมัติได้!");
            }
        }
    }
#endif
}