using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [Header("Data")]
    public List<ShopData> items = new List<ShopData>();
    public int itemCount = 4;   // จำนวนสินค้าที่โชว์ต่อการเข้าร้าน 1 ครั้ง

    [Header("UI")]
    public GameObject shopUi;
    public Transform itemParent;
    public ShopItemPanel itemPanelPrefab;

    [SerializeField] private BuffManager buffManager;

    private readonly List<ShopItemPanel> panels = new List<ShopItemPanel>();
    private readonly HashSet<ShopData> purchased = new HashSet<ShopData>(); // ซื้อแล้วในการเข้าร้านรอบนี้

    private Action onClosed;

    public event Action<ShopData> OnPurchased;

    void Awake()
    {
        if (buffManager == null) buffManager = FindFirstObjectByType<BuffManager>();
        if (shopUi != null) shopUi.SetActive(false);
    }

    // เปิดร้านแล้วหยุดเกม — ปิดร้านเมื่อไหร่เรียก closed
    public void Open(Action closed = null)
    {
        onClosed = closed;
        BuildUi();
        shopUi.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Close()
    {
        shopUi.SetActive(false);
        Time.timeScale = 1f;
        onClosed?.Invoke();
        onClosed = null;
    }

    void BuildUi()
    {
        foreach (Transform child in itemParent)
            Destroy(child.gameObject);
        panels.Clear();
        purchased.Clear();

        foreach (ShopData item in RollItems())
        {
            ShopItemPanel panel = Instantiate(itemPanelPrefab, itemParent);
            panel.Setup(item, Buy);
            panels.Add(panel);
        }
        RefreshPanels();
    }

    // สุ่มไม่ซ้ำกัน จำกัดจำนวนต่อการเข้าร้านแต่ละครั้ง
    List<ShopData> RollItems()
    {
        return items.OrderBy(_ => UnityEngine.Random.value).Take(itemCount).ToList();
    }

    // คืน true เมื่อซื้อสำเร็จ — หักเงินแล้วใช้ modifiers กับ player
    public bool Buy(ShopData item)
    {
        if (item == null || MoneyManager.Instance == null) return false;
        if (purchased.Contains(item)) return false;
        if (!MoneyManager.Instance.TrySpend(item._Price)) return false;

        purchased.Add(item);
        foreach (StatModifier m in item.modifiers)
            buffManager.ApplyStat(m.stat, m.value);

        Debug.Log($"Buy: {item._Name} ({item._Price}g)");
        OnPurchased?.Invoke(item);
        RefreshPanels();
        return true;
    }

    // ปิดปุ่มของที่ซื้อแล้ว หรือเงินไม่พอ
    void RefreshPanels()
    {
        int gold = MoneyManager.Instance != null ? MoneyManager.Instance.GetGold() : 0;
        foreach (ShopItemPanel p in panels)
        {
            bool sold = purchased.Contains(p.Data);
            p.SetSold(sold);
            p.SetAffordable(!sold && gold >= p.Data._Price);
        }
    }
}
