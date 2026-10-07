using System.Collections.Generic;
using System.Linq;
using SQLite;

public class PlayerBuffRepository
{
    private SQLiteConnection db;

    public PlayerBuffRepository(SQLiteConnection connection)
    {
        db = connection;
        db.CreateTable<PlayerBuffRow>();
        db.CreateTable<BuffRow>();
    }

    //  buff table

    // เพิ่ม buff ใหม่ และอัปเดตค่าของ buff เดิมให้ตรงกับ BuffData
    public void SeedDefinitions(IEnumerable<BuffData> buffs)
    {
        Dictionary<string, BuffRow> existing = GetDefinitions();

        foreach (BuffData buff in buffs)
        {
            if (existing.TryGetValue(buff._Name, out BuffRow row))
            {
                row.BuffName = buff._Name;
                row.Stat = buff.stat;
                row.Value = buff.value;
                db.Update(row);
            }
            else
            {
                db.Insert(new BuffRow { BuffName = buff._Name, Stat = buff.stat, Value = buff.value });
            }
        }
    }

    // key = BuffName (ตรงกับ BuffData._Name)
    public Dictionary<string, BuffRow> GetDefinitions()
    {
        return db.Table<BuffRow>().ToDictionary(r => r.BuffName);
    }

    //  PlayerBuffs table
    public void SetupRow(int playerId, IEnumerable<BuffRow> buffs)
    {
        db.RunInTransaction(() =>
        {
            var existing = db.Table<PlayerBuffRow>().Where(r => r.PlayerId == playerId).ToList()
                .Select(r => r.BuffId).ToHashSet();

            db.InsertAll(buffs.Where(b => !existing.Contains(b.BuffId)).Select(b => new PlayerBuffRow
            {
                PlayerId = playerId,
                BuffId = b.BuffId,
                BuffName = b.BuffName,
                CurrentRun = 0,
                AllRun = 0
            }));
        });
    }

    public void AddPick(int playerId, BuffRow buff)
    {
        PlayerBuffRow row = db.Table<PlayerBuffRow>()
            .FirstOrDefault(row => row.PlayerId == playerId && row.BuffId == buff.BuffId);

        if (row == null)
        {
            db.Insert(new PlayerBuffRow { PlayerId = playerId, BuffId = buff.BuffId, BuffName = buff.BuffName, CurrentRun = 1, AllRun = 1 });
            return;
        }

        row.CurrentRun++;
        row.AllRun++;
        db.Update(row);
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
