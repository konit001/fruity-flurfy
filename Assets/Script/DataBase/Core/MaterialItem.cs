using System.Collections.Generic;
using UnityEngine;

// ย้าย Min / Max มาไว้ที่ไอเทมแต่ละชนิดแทน
[System.Serializable]
public class ItemDrop
{
    public GameObject itemDrop;
    public MaterialData _data;
    
    [Header("Drop Amount")]
    public int minAmount = 1;
    public int maxAmount = 1;

    public static void SpawnAll(List<ItemDrop> items, Vector3 origin, float radiusMin, float radiusMax)
    {
        if (items == null) return;
        foreach (var dropItem in items)
        {
            if (dropItem.itemDrop == null) continue;
            int amountToDrop = Random.Range(dropItem.minAmount, dropItem.maxAmount + 1);

            for (int i = 0; i < amountToDrop; i++)
            {
                Vector2 dir = Random.insideUnitCircle.normalized * Random.Range(radiusMin, radiusMax);
                Vector3 offset = new Vector3(dir.x, 0f, dir.y);
                GameObject drop = Object.Instantiate(dropItem.itemDrop, origin + offset, Quaternion.identity);
                if (drop.TryGetComponent(out DroppedItem dropped))
                {
                    dropped.data = dropItem._data;
                    dropped.amount = 1;
                }
            }
        }
    }
}

public class MaterialItem : MonoBehaviour
{
    public static readonly List<MaterialItem> All = new List<MaterialItem>();

    public List<ItemDrop> items; 
    
    [SerializeField] private int breakPoint = 3;
    
    [Header("Drop Scatter")]
    [SerializeField] private float dropRadiusMin = 0.5f;
    [SerializeField] private float dropRadiusMax = 1.5f;

    void OnEnable() => All.Add(this);
    void OnDisable() => All.Remove(this);

    public void TakeDamage(int damage)
    {
        if (breakPoint <= 0) return;

        breakPoint -= damage;
        if (breakPoint <= 0)
        {
            Drop();
            Destroy(gameObject);
        }
    }

    public void Drop()
    {
        ItemDrop.SpawnAll(items, transform.position, dropRadiusMin, dropRadiusMax);
    }
}
