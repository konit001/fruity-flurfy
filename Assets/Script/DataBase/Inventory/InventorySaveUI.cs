using System.Linq;
using UnityEngine;

// เซฟ/โหลดของที่ซื้อ — เขียนผ่าน InventoryRepository, ตอนโหลดใส่เข้า InventoryManager แล้วใช้ modifiers ซ้ำ
public class InventorySaveUI : MonoBehaviour
{
    [SerializeField] private ShopManager shopManager;
    [SerializeField] private BuffManager buffManager;
    private InventoryRepository repo;

    void Awake()
    {
        repo = new InventoryRepository(DbProvider.Connection);
        if (shopManager == null) shopManager = FindFirstObjectByType<ShopManager>();
        if (buffManager == null) buffManager = FindFirstObjectByType<BuffManager>();

        if (GameSession.IsLoggedIn && shopManager != null)
        {
            repo.SetupRow(GameSession.PlayerId, shopManager.items.Where(i => i != null).Select(i => i.id));
        }
    }

    public void Save()
    {
        if (!GameSession.IsLoggedIn || InventoryManager.Instance == null) return;

        var entries = InventoryManager.Instance.All.Select(kv => (kv.Key.id, kv.Value));
        repo.SaveQuantities(GameSession.PlayerId, entries);
    }

    public void Load()
    {
        if (!GameSession.IsLoggedIn || InventoryManager.Instance == null || shopManager == null) return;

        foreach (InventoryRow row in repo.GetActive(GameSession.PlayerId))
        {
            ShopData item = shopManager.items.FirstOrDefault(i => i.id == row.ShopItemId);
            if (item == null) continue;

            InventoryManager.Instance.Add(item, row.CurrentRun);
            ApplyModifiers(item, row.CurrentRun);
        }
    }

    private void ApplyModifiers(ShopData item, int quantity)
    {
        if (buffManager == null) return;

        foreach (StatModifier m in item.modifiers)
        {
            if (m.stat == BuffStat.Heal) continue;

            for (int i = 0; i < quantity; i++)
                buffManager.ApplyStat(m.stat, m.value);
        }
    }

    public void ResetRun()
    {
        if (GameSession.IsLoggedIn) repo.ResetRun(GameSession.PlayerId);
    }
}
