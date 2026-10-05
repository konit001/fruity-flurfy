using System.Collections.Generic;
using UnityEngine;

// เก็บจำนวนไอเทมที่ซื้อ (นับต่อ ShopData) แล้วแสดงลง slot ใน Shop UI
// สร้างช่องรอไว้ตาม initialSlots ถ้าของเกินจำนวนช่อง จะสร้างช่องเพิ่มให้
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [SerializeField] private ShopManager shopManager;
    [SerializeField] private RectTransform slotParent;   // ที่ที่ slot วางอยู่ (มี Grid Layout Group)
    [SerializeField] private InventorySlotUI slotPrefab; // ไม่ใส่ = สร้างช่องพื้นฐานด้วยโค้ด
    [SerializeField] private int initialSlots = 20;

    private readonly Dictionary<ShopData, int> counts = new Dictionary<ShopData, int>();
    private readonly List<ShopData> order = new List<ShopData>(); // เรียงตามลำดับที่ซื้อครั้งแรก
    private readonly List<InventorySlotUI> slots = new List<InventorySlotUI>();

    void Awake()
    {
        Instance = this;
        if (shopManager == null) shopManager = FindFirstObjectByType<ShopManager>();

        if (slotParent == null)
        {
            Debug.LogError("InventoryManager: Slot Parent is not assigned.", this);
            return;
        }

        while (slots.Count < initialSlots) AddSlot();
        Refresh();
    }

    void OnEnable()
    {
        if (shopManager != null) shopManager.OnPurchased += Add;
    }

    void OnDisable()
    {
        if (shopManager != null) shopManager.OnPurchased -= Add;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    // ของที่ซื้อทั้งหมด (ShopData, จำนวน) — ใช้ตอนเซฟ
    public IEnumerable<KeyValuePair<ShopData, int>> All => counts;

    public int GetCount(ShopData item) => item != null && counts.TryGetValue(item, out int n) ? n : 0;

    public void Add(ShopData item) => Add(item, 1);

    public void Add(ShopData item, int amount)
    {
        if (item == null || amount <= 0) return;

        if (!counts.ContainsKey(item))
        {
            counts[item] = 0;
            order.Add(item);
        }
        counts[item] += amount;
        Refresh();
    }

    public void Clear()
    {
        counts.Clear();
        order.Clear();
        Refresh();
    }

    void Refresh()
    {
        if (slotParent == null) return;

        while (slots.Count < order.Count) AddSlot();   // slotParent ไม่ว่างแล้ว AddSlot เพิ่มได้แน่

        for (int i = 0; i < slots.Count; i++)
        {
            if (i < order.Count) slots[i].Set(order[i], counts[order[i]]);
            else slots[i].SetEmpty();
        }
    }

    void AddSlot()
    {
        if (slotParent == null) return;

        InventorySlotUI slot = slotPrefab != null
            ? Instantiate(slotPrefab, slotParent)
            : InventorySlotUI.CreateDefault(slotParent);
        slots.Add(slot);
    }
}
