using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class GridMaterialSpawn : MonoBehaviour
{
    [System.Serializable]
    public class SpawnEntry
    {
        public GameObject prefab;
        public MaterialData material;
        public float weight = 1f;
    }

    [Header("Mask")]
    [SerializeField] private Tilemap maskTilemap;

    [Header("Spawn Settings")]
    [SerializeField] private Transform spawnParent;
    [SerializeField, Range(0f, 1f)] private float spawnChance = 0.3f;
    [SerializeField] private SpawnEntry[] spawnEntries;

    [Header("Placement Jitter")]
    [SerializeField] private Vector2 cellOffsetJitter = Vector2.zero;
    [SerializeField] private bool randomYRotation = true;

    private readonly List<GameObject> spawned = new List<GameObject>();

    void Start()
    {
        Spawn();
    }

    // เรียกตอนขึ้นเวฟใหม่ — ลบ node เดิมแล้วสุ่มตำแหน่งใหม่ทั้งหมด
    public void Respawn()
    {
        foreach (var obj in spawned)
            if (obj != null) Destroy(obj);
        spawned.Clear();

        Spawn();
    }

    public void Spawn()
    {
        if (maskTilemap == null)
        {
            Debug.LogWarning($"{name}: maskTilemap ยังไม่ได้กำหนด");
            return;
        }
        if (spawnEntries == null || spawnEntries.Length == 0)
        {
            Debug.LogWarning($"{name}: spawnEntries ว่างเปล่า");
            return;
        }
        Transform parent = spawnParent != null ? spawnParent : transform;
        BoundsInt bounds = maskTilemap.cellBounds;

        // สุ่มตำแหน่งใหม่ทุกครั้ง (ทุกเวฟ) แล้ว spawn เลย
        foreach (Vector3Int cellPos in bounds.allPositionsWithin)
        {
            if (!maskTilemap.HasTile(cellPos)) continue;
            if (Random.value > spawnChance) continue;

            var entry = GetWeightedRandomEntry();
            if (entry == null || entry.prefab == null) continue;

            Vector3 worldPos = maskTilemap.GetCellCenterWorld(cellPos);
            worldPos.x += Random.Range(-cellOffsetJitter.x, cellOffsetJitter.x);
            worldPos.z += Random.Range(-cellOffsetJitter.y, cellOffsetJitter.y);

            Quaternion rot = randomYRotation
                ? Quaternion.Euler(0f, Random.Range(0f, 360f), 0f)
                : Quaternion.identity;

            GameObject obj = Instantiate(entry.prefab, worldPos, rot, parent);
            spawned.Add(obj);

            var materialItem = obj.GetComponent<MaterialItem>();
            // if (materialItem != null)
            //     materialItem.DropAmount = materialItem.RollDropAmount();
        }
    }

    private SpawnEntry GetWeightedRandomEntry()
    {
        float totalWeight = 0f;
        foreach (var entry in spawnEntries)
            totalWeight += entry.weight;

        if (totalWeight <= 0f) return null;

        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;
        foreach (var entry in spawnEntries)
        {
            cumulative += entry.weight;
            if (roll <= cumulative)
                return entry;
        }
        return spawnEntries[spawnEntries.Length - 1];
    }
}
