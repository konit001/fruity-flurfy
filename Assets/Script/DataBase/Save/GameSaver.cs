using UnityEngine;
using UnityEngine.SceneManagement;

// เซฟสถานะเกมลง DB ตอนเปลี่ยนเวฟ / ตอนตาย
// (inventory กับ gold เขียนลง DB ทุกครั้งที่เปลี่ยนอยู่แล้ว จึงเซฟเพิ่มแค่เวฟ + ฉาก + ตำแหน่ง)
public static class GameSaver
{
    // เริ่มเวฟใหม่: เซฟเวฟ + ฉาก + ตำแหน่งปัจจุบัน + gold
    public static void SaveAtWave(int wave)
    {
        if (!GameSession.IsLoggedIn) return;

        var db = DbProvider.Connection;
        var playerRepo = new PlayerRepository(db);
        playerRepo.UpdateWave(GameSession.PlayerId, wave);

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null) return;

        var pos = playerObj.transform.position;
        var player = playerRepo.GetById(GameSession.PlayerId);
        int gold = player != null ? player.Gold : 0;

        new SaveRepository(db).Save(GameSession.PlayerId, SceneManager.GetActiveScene().name, 1, gold, pos.x, pos.y, pos.z);
    }

    // ตาย: เซฟเวฟที่ตาย + gold แต่ไม่เซฟตำแหน่งที่ตาย (ใช้ตำแหน่งตอนเริ่มเวฟเดิม)
    public static void SaveOnDeath(int wave)
    {
        if (!GameSession.IsLoggedIn) return;

        var db = DbProvider.Connection;
        var playerRepo = new PlayerRepository(db);
        playerRepo.UpdateWave(GameSession.PlayerId, wave);

        var saveRepo = new SaveRepository(db);
        var last = saveRepo.Load(GameSession.PlayerId);
        if (last == null) return;

        var player = playerRepo.GetById(GameSession.PlayerId);
        int gold = player != null ? player.Gold : last.Gold;

        saveRepo.Save(GameSession.PlayerId, last.SceneName, last.Day, gold, last.PosX, last.PosY, last.PosZ);
    }
}
