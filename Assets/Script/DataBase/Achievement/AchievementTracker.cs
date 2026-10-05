using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// ตรวจ/ปลดล็อก achievement จากสถิติใน DB — ไม่เกี่ยวกับ UI (UI ฟัง OnUnlocked เอา)
// นิยาม achievement (AchievementData ใน Assets/Data/Achievement/) รับมาจาก AchievementUI.achievements ผ่าน SetCatalog
// ซีนเกมต้องมี AchievementUI ที่ใส่รายการแล้ว ไม่งั้นไม่มี achievement ให้ตรวจ
public static class AchievementTracker
{
    // ปลดล็อกใหม่ — AchievementUI ฟังเพื่อเด้งป๊อปอัพ
    public static event Action<AchievementData> OnUnlocked;

    // ปลดล็อกใหม่ตั้งแต่ครั้งล่าสุดที่โชว์รายการ — ใช้ติดป้าย NEW (static อยู่ข้ามซีน)
    public static readonly List<AchievementData> JustUnlocked = new List<AchievementData>();

    private static List<AchievementData> catalog = new List<AchievementData>();

    public static IReadOnlyList<AchievementData> Catalog => catalog;

    // AchievementUI ส่งรายการที่ลากไว้ใน Inspector มาให้ตอน Awake (จำไว้ข้ามซีน)
    public static void SetCatalog(IEnumerable<AchievementData> list)
    {
        catalog = list.Where(a => a != null).ToList();
    }

    // อ่านสถิติตลอดกาลจากตารางอื่น แล้วอัปเดตความคืบหน้าทุก achievement
    public static void Evaluate(int waveReached)
    {
        if (!GameSession.IsLoggedIn) return;

        var db = DbProvider.Connection;
        int playerId = GameSession.PlayerId;
        var repo = new AchievementRepository(db);

        Players player = new PlayerRepository(db).GetById(playerId);
        int kills = player != null ? player.Kill : 0;
        int items = new InventoryRepository(db).GetAll(playerId).Sum(r => r.TotalCount);
        int buffs = new PlayerBuffRepository(db).GetAll(playerId).Sum(r => r.TotalCount);

        foreach (AchievementData a in Catalog)
        {
            int value = 0;
            switch (a.metric)
            {
                case AchievementMetric.Kills:       value = kills; break;
                case AchievementMetric.BestWave:    value = waveReached; break;
                case AchievementMetric.ItemsBought: value = items; break;
                case AchievementMetric.BuffsPicked: value = buffs; break;
            }

            if (repo.SaveProgress(playerId, a.id, value, a.target))
            {
                JustUnlocked.Add(a);
                OnUnlocked?.Invoke(a);
            }
        }
    }
}
