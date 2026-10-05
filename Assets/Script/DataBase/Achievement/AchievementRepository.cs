using System;
using System.Collections.Generic;
using System.Linq;
using SQLite;

public class AchievementRepository
{
    private SQLiteConnection db;

    public AchievementRepository(SQLiteConnection connection)
    {
        db = connection;
        db.CreateTable<AchievementRow>();
    }

    public List<AchievementRow> GetAll(int playerId)
    {
        return db.Table<AchievementRow>().Where(r => r.PlayerId == playerId).ToList();
    }

    // เซฟความคืบหน้า (ไม่ลดลง) — คืน true เมื่อเพิ่งปลดล็อกครั้งนี้
    public bool SaveProgress(int playerId, string achievementId, int progress, int target)
    {
        var row = db.Table<AchievementRow>()
            .Where(r => r.PlayerId == playerId && r.AchievementId == achievementId)
            .FirstOrDefault();

        if (row == null)
        {
            row = new AchievementRow
            {
                PlayerId = playerId,
                AchievementId = achievementId,
                Progress = progress
            };
            bool unlockedNew = TryUnlock(row, target);
            db.Insert(row);
            return unlockedNew;
        }

        if (progress > row.Progress) row.Progress = progress;

        bool unlocked = TryUnlock(row, target);
        db.Update(row);
        return unlocked;
    }

    private bool TryUnlock(AchievementRow row, int target)
    {
        if (!string.IsNullOrEmpty(row.UnlockedAt)) return false;
        if (row.Progress < target) return false;

        row.UnlockedAt = DateTime.UtcNow.ToString("o");
        return true;
    }
}
