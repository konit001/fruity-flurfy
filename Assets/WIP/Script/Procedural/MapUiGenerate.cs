using System.Collections.Generic;
using UnityEngine;

public class MapUiGenerate : MonoBehaviour
{
    public static MapUiGenerate instance;
    [Header("UI Settings")]
    public RectTransform MapLayout;
    public MapUiSlot mapSlotPrefab;

    [Header("Map Scale Settings")]
    public float uiSpacing = 50f; // Prefab (..X..*..Y..)
    public float roomWorldSize = 15f; // Same at roomWidth and roomDepth In StateGenerate

    [SerializeField] private List<StateRoom> activeRooms = new List<StateRoom>();
    [SerializeField] private List<MapUiSlot> spawnedSlots = new List<MapUiSlot>();

    private void Awake()
    {
        instance = this;
    }
    public void MapCountSetup(StateRoom room)
    {
        activeRooms.Add(room);
    }
    public void MapGenrerate()
    {
        ClearMap();

        foreach (StateRoom room in activeRooms)
        {
            MapUiSlot newSlot = Instantiate(mapSlotPrefab, MapLayout);

            float X = (room.transform.position.x / roomWorldSize) * uiSpacing;
            float Y = (room.transform.position.z / roomWorldSize) * uiSpacing;

            newSlot.GetComponent<RectTransform>().anchoredPosition = new Vector2(X, Y);

            if (room._roomData.IconRoomSprite != null)
            {
                newSlot.Setup(room._roomData.IconRoomSprite);
            }
            else
            {
                newSlot.Setup(null);
            }

            spawnedSlots.Add(newSlot);
        }
    }

    public void ClearMap()
    {
        foreach (var slot in spawnedSlots)
        {
            if (slot != null) Destroy(slot.gameObject);
        }
        spawnedSlots.Clear();
    }
    public void ClearRoomData()
    {
        activeRooms.Clear();
    }
}
