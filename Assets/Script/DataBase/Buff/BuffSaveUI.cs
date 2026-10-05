using System.Linq;
using UnityEngine;

// เซฟ/โหลด buff ที่เลือก — เขียนผ่าน PlayerBuffRepository, ตอนโหลดให้ BuffManager ใส่ stat ซ้ำ
public class BuffSaveUI : MonoBehaviour
{
    [SerializeField] private BuffManager buffManager;

    private PlayerBuffRepository repo;

    void Awake()
    {
        repo = new PlayerBuffRepository(DbProvider.Connection);
        if (buffManager == null) buffManager = FindFirstObjectByType<BuffManager>();
    }

    public void Save()
    {
        if (!GameSession.IsLoggedIn || buffManager == null) return;

        var entries = buffManager.Picked.Select(kv => (kv.Key, kv.Value));
        repo.SaveQuantities(GameSession.PlayerId, entries);
    }

    public void Load()
    {
        if (!GameSession.IsLoggedIn || buffManager == null) return;

        foreach (PlayerBuffRow row in repo.GetActive(GameSession.PlayerId))
            buffManager.Restore(row.BuffId, row.Quantity);
    }

    // เริ่มรอบใหม่: buff ที่ถือเป็น 0 แต่เก็บสถิติตลอดกาลไว้
    public void ResetRun()
    {
        if (GameSession.IsLoggedIn) repo.ResetRun(GameSession.PlayerId);
    }
}
