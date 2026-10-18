using System.Collections.Generic;
using System.Linq;
using SQLite;

public class PlayerBuffRepository
{
    private SQLiteConnection db;

    public PlayerBuffRepository(SQLiteConnection connection)
    {
        db = connection;
        // สร้างเพื่อให้ได้ composite PK ก่อน CreateTable
        db.Execute(@"CREATE TABLE IF NOT EXISTS PlayerBuffs (
            PlayerId   INTEGER NOT NULL,
            BuffId     INTEGER NOT NULL,
            PlayerName VARCHAR(50),
            BuffName   VARCHAR(50),
            CurrentRun INTEGER NOT NULL DEFAULT 0,
            AllRun     INTEGER NOT NULL DEFAULT 0,
            PRIMARY KEY (PlayerId, BuffId))");
        db.CreateTable<PlayerBuffRow>();
    }

    private string GetPlayerName(int playerId)
    {
        return db.Find<Players>(playerId)?.Name;
    }

    //  PlayerBuffs table
    public void SetupRow(int playerId, IEnumerable<BuffData> buffs)
    {
        db.RunInTransaction(() =>
        {
            var existing = db.Table<PlayerBuffRow>().Where(r => r.PlayerId == playerId).ToList()
                .Select(r => r.BuffId).ToHashSet();
            var playerName = GetPlayerName(playerId);

            db.Execute("UPDATE PlayerBuffs SET PlayerName = ? WHERE PlayerId = ?", playerName, playerId);

            db.InsertAll(buffs.Where(b => b != null && !existing.Contains(b.id)).Select(b => new PlayerBuffRow
            {
                PlayerId = playerId,
                BuffId = b.id, // ดึง id จาก BuffData มาเป็น key ใน DB
                PlayerName = playerName,
                BuffName = b._Name,
                CurrentRun = 0,
                AllRun = 0
            }));
        });
    }

    public void AddPick(int playerId, BuffData buff)
    {
        var playerName = GetPlayerName(playerId);

        int updated = db.Execute(
            "UPDATE PlayerBuffs SET CurrentRun = CurrentRun + 1, AllRun = AllRun + 1 WHERE PlayerId = ? AND BuffId = ?",
            playerId, buff.id); // หาแถวด้วย id ของ BuffData

        if (updated == 0)
            db.Insert(new PlayerBuffRow { PlayerId = playerId, BuffId = buff.id, PlayerName = playerName, BuffName = buff._Name, CurrentRun = 1, AllRun = 1 });
    }

    // buff ของรอบปัจจุบัน
    public List<PlayerBuffRow> GetActive(int playerId)
    {
        return db.Table<PlayerBuffRow>().Where(row => row.PlayerId == playerId && row.CurrentRun > 0).ToList();
    }

    public int GetAllCount(int playerId)
    {
        return db.Table<PlayerBuffRow>().Where(row => row.PlayerId == playerId).ToList().Sum(row => row.AllRun);
    }

    public void ResetRun(int playerId)
    {
        db.Execute("UPDATE PlayerBuffs SET CurrentRun = 0 WHERE PlayerId = ?", playerId);
    }
}
