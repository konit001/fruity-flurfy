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
    }

    // buff ของรอบปัจจุบัน (Quantity > 0)
    public List<PlayerBuffRow> GetActive(int playerId)
    {
        return db.Table<PlayerBuffRow>().Where(r => r.PlayerId == playerId && r.Quantity > 0).ToList();
    }

    // สถิติตลอดกาล (รวมแถวที่ Quantity = 0)
    public List<PlayerBuffRow> GetAll(int playerId)
    {
        return db.Table<PlayerBuffRow>().Where(r => r.PlayerId == playerId).ToList();
    }

    // แถวที่มีอยู่แก้ Quantity และบวก TotalCount ตามที่เลือกเพิ่มจากเซฟครั้งก่อน
    // แถวที่ไม่อยู่ในชุดใหม่ตั้ง Quantity = 0 (ไม่ลบ)
    public void SaveQuantities(int playerId, IEnumerable<(string BuffId, int Quantity)> entries)
    {
        var current = entries.Where(e => e.Quantity > 0).ToDictionary(e => e.BuffId, e => e.Quantity);

        db.RunInTransaction(() =>
        {
            foreach (var row in GetAll(playerId))
            {
                if (current.ContainsKey(row.BuffId)) continue;

                row.Quantity = 0;
                db.Update(row);
            }

            foreach (var pair in current)
            {
                var row = db.Table<PlayerBuffRow>()
                    .Where(r => r.PlayerId == playerId && r.BuffId == pair.Key)
                    .FirstOrDefault();

                if (row == null)
                {
                    db.Insert(new PlayerBuffRow
                    {
                        PlayerId = playerId,
                        BuffId = pair.Key,
                        Quantity = pair.Value,
                        TotalCount = pair.Value
                    });
                    continue;
                }

                int added = pair.Value - row.Quantity; // เลือกเพิ่มตั้งแต่เซฟครั้งก่อน
                if (added > 0) row.TotalCount += added;
                row.Quantity = pair.Value;
                db.Update(row);
            }
        });
    }

    // ตาย: buff ที่ถือเป็น 0 แต่เก็บ TotalCount ไว้
    public void ResetRun(int playerId)
    {
        db.Execute("UPDATE PlayerBuffs SET Quantity = 0 WHERE PlayerId = ?", playerId);
    }
}
