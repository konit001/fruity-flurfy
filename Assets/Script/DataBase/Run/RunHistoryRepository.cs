using System;
using System.Collections.Generic;
using SQLite;

public class RunHistoryRepository
{
    private SQLiteConnection db;

    public RunHistoryRepository(SQLiteConnection connection)
    {
        db = connection;
        db.CreateTable<RunRecord>();
    }

    public void Add(int playerId, int wave, int kills, int goldEarned)
    {
        var record = new RunRecord
        {
            PlayerId = playerId,
            WaveReached = wave,
            Kills = kills,
            GoldEarned = goldEarned,
            EndedAt = DateTime.UtcNow.ToString("o")
        };
        db.Insert(record);
    }

    // รอบล่าสุดก่อน
    public List<RunRecord> GetRecent(int playerId, int count)
    {
        return db.Query<RunRecord>(
            "SELECT * FROM RunHistory WHERE PlayerId = ? ORDER BY Id DESC LIMIT ?", playerId, count);
    }
}
