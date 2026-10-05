using System.Collections.Generic;
using System.Linq;
using SQLite;

public class InventoryRepository
{
    private SQLiteConnection db;

    public InventoryRepository(SQLiteConnection connection)
    {
        db = connection;
        db.CreateTable<InventoryRow>();
    }

    // ของที่ถืออยู่ในรอบปัจจุบัน (Quantity > 0)
    public List<InventoryRow> GetActive(int playerId)
    {
        return db.Table<InventoryRow>().Where(r => r.PlayerId == playerId && r.Quantity > 0).ToList();
    }

    // สถิติตลอดกาล (รวมแถวที่ Quantity = 0)
    public List<InventoryRow> GetAll(int playerId)
    {
        return db.Table<InventoryRow>().Where(r => r.PlayerId == playerId).ToList();
    }

    // แถวที่มีอยู่แก้ Quantity และบวก TotalCount ตามที่ซื้อเพิ่มจากเซฟครั้งก่อน
    // แถวที่ไม่อยู่ในชุดใหม่ตั้ง Quantity = 0 (ไม่ลบ)
    public void SaveQuantities(int playerId, IEnumerable<(string ItemId, int Quantity)> entries)
    {
        var current = entries.Where(e => e.Quantity > 0).ToDictionary(e => e.ItemId, e => e.Quantity);

        db.RunInTransaction(() =>
        {
            foreach (var row in GetAll(playerId))
            {
                if (current.ContainsKey(row.ShopItemId)) continue;

                row.Quantity = 0;
                db.Update(row);
            }

            foreach (var pair in current)
            {
                var row = db.Table<InventoryRow>()
                    .Where(r => r.PlayerId == playerId && r.ShopItemId == pair.Key)
                    .FirstOrDefault();

                if (row == null)
                {
                    db.Insert(new InventoryRow
                    {
                        PlayerId = playerId,
                        ShopItemId = pair.Key,
                        Quantity = pair.Value,
                        TotalCount = pair.Value
                    });
                    continue;
                }

                int added = pair.Value - row.Quantity; // ซื้อเพิ่มตั้งแต่เซฟครั้งก่อน
                if (added > 0) row.TotalCount += added;
                row.Quantity = pair.Value;
                db.Update(row);
            }
        });
    }

    // ตาย: ของที่ถือเป็น 0 แต่เก็บ TotalCount ไว้
    public void ResetRun(int playerId)
    {
        db.Execute("UPDATE Inventory SET Quantity = 0 WHERE PlayerId = ?", playerId);
    }
}
