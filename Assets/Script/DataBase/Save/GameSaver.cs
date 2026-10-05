using UnityEngine;
using UnityEngine.SceneManagement;

// จุดเรียกเซฟ/โหลดของเกม — ประสานงาน Repo + InventorySaveUI + BuffSaveUI (ไม่ login = ข้ามทั้งหมด)
public static class GameSaver
{
    // เวฟที่จะเริ่มตอนเข้าซีน (ไม่มีเซฟ = 1)
    public static int GetSavedWave()
    {
        if (!GameSession.IsLoggedIn) return 1;

        SaveData save = new SaveRepository(DbProvider.Connection).Load(GameSession.PlayerId);
        return save != null ? Mathf.Max(save.Wave, 1) : 1;
    }

    // เริ่มเวฟใหม่ (หลังปิดร้าน): เซฟเวฟ + ทอง + ของที่ซื้อ + buff
    public static void SaveAtWave(int wave)
    {
        if (!GameSession.IsLoggedIn) return;

        var db = DbProvider.Connection;
        int gold = MoneyManager.Instance != null ? MoneyManager.Instance.GetGold() : 0;

        new PlayerRepository(db).UpdateProgress(GameSession.PlayerId, wave, gold);
        new SaveRepository(db).Save(GameSession.PlayerId, SceneManager.GetActiveScene().name, wave, gold);

        GetOrCreate<InventorySaveUI>().Save();
        GetOrCreate<BuffSaveUI>().Save();
        AchievementTracker.Evaluate(wave);
    }

    // เข้าซีนเกม: โหลดทอง + ของที่ซื้อ + buff จากเซฟ (เวฟเริ่มจาก GetSavedWave ใน WaveManager)
    public static void LoadRun()
    {
        if (!GameSession.IsLoggedIn) return;

        SaveData save = new SaveRepository(DbProvider.Connection).Load(GameSession.PlayerId);
        if (save == null) return;

        if (MoneyManager.Instance != null) MoneyManager.Instance.SetGold(save.Gold);

        GetOrCreate<InventorySaveUI>().Load();
        GetOrCreate<BuffSaveUI>().Load();
    }

    // ตาย: บันทึกสถิติรอบนี้ ล้างเซฟของรอบ และตั้งของ/buff ที่ถือเป็น 0 (เก็บ TotalCount ตลอดกาลไว้ ไม่ลบแถว)
    public static void SaveOnDeath(int wave)
    {
        if (!GameSession.IsLoggedIn)
        {
            RunStats.Reset();
            return;
        }

        var db = DbProvider.Connection;
        var playerRepo = new PlayerRepository(db);

        new RunHistoryRepository(db).Add(GameSession.PlayerId, wave, RunStats.Kills, RunStats.GoldEarned);
        playerRepo.AddKills(GameSession.PlayerId, RunStats.Kills);
        playerRepo.UpdateProgress(GameSession.PlayerId, 1, 0);

        // ไม่ลบแถวเซฟ — รีเซ็ตให้เริ่มรอบใหม่ที่เวฟ 1 ทอง 0
        new SaveRepository(db).Save(GameSession.PlayerId, GameSession.GameScene, 1, 0);
        GetOrCreate<InventorySaveUI>().ResetRun();
        GetOrCreate<BuffSaveUI>().ResetRun();
        AchievementTracker.Evaluate(wave);

        RunStats.Reset();
    }

    // หา component ในซีน ถ้าไม่มีสร้าง GameObject ใหม่ให้ (ไม่ต้องตั้งค่าในซีนเอง)
    private static T GetOrCreate<T>() where T : MonoBehaviour
    {
        T found = Object.FindFirstObjectByType<T>();
        if (found != null) return found;

        return new GameObject(typeof(T).Name).AddComponent<T>();
    }
}
