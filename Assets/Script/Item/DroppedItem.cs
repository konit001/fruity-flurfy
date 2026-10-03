using System.Collections.Generic;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    public static readonly List<DroppedItem> All = new List<DroppedItem>();

    public MaterialData data;
    [HideInInspector]public int amount = 1;

    [Header("Bob")]
    [SerializeField] private float bobHeight = 0.15f;
    [SerializeField] private float bobSpeed = 2f;

    Vector3 basePos;
    float bobOffset;
    Transform target;
    float moveSpeed;
    float pickupDistance;

    void OnEnable() => All.Add(this);
    void OnDisable() => All.Remove(this);

    // ล้างของที่ตกอยู่บนพื้นทั้งหมด — เรียกตอนขึ้นเวฟใหม่
    public static void ClearAll()
    {
        for (int i = All.Count - 1; i >= 0; i--)
            if (All[i] != null) Destroy(All[i].gameObject);
    }

    void Start()
    {
        basePos = transform.position;
        bobOffset = Random.value * Mathf.PI * 2f; // ไม่ให้ทุกชิ้นลอยพร้อมกัน
    }

    // เรียกจาก PlayerMagnet
    public void Attract(Transform to, float speed, float pickupDist)
    {
        target = to;
        moveSpeed = speed;
        pickupDistance = pickupDist;
    }

    void Update()
    {
        if (target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
            basePos = transform.position;

            if (Vector3.Distance(transform.position, target.position) <= pickupDistance)
                TryPickup();
            return;
        }

        Vector3 pos = basePos;
        pos.y += Mathf.Sin(Time.time * bobSpeed + bobOffset) * bobHeight;
        transform.position = pos;
    }

    void TryPickup()
    {
        // ของที่ดรอปคือเหรียญทอง: data.price = ทองต่อชิ้น
        int gold = data != null ? data.price * amount : 0;
        if (MoneyManager.Instance != null) MoneyManager.Instance.AddGold(gold);
        Destroy(gameObject);
    }
}
