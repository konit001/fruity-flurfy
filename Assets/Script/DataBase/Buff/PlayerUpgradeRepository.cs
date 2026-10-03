using System.Collections.Generic;
using SQLite;

public class PlayerUpgradeRepository
{
    private SQLiteConnection db;

    public PlayerUpgradeRepository(SQLiteConnection connection)
    {
        db = connection;
        db.CreateTable<PlayerUpgrade>();
    }

    // เขียนทับอัพเกรดทั้งหมดของ player ด้วยชุดใหม่ ในทรานแซกชันเดียว
    public void ReplaceAll(int playerId, IEnumerable<(string Stat, int Quantity)> entries)
    {
        db.RunInTransaction(() =>
        {
            db.Execute("DELETE FROM PlayerUpgrades WHERE PlayerId = ?", playerId);
            foreach (var (stat, quantity) in entries)
            {
                if (quantity <= 0) continue;
                db.Insert(new PlayerUpgrade { PlayerId = playerId, Stat = stat, Quantity = quantity });
            }
        });
    }

    public void Clear(int playerId)
    {
        db.Execute("DELETE FROM PlayerUpgrades WHERE PlayerId = ?", playerId);
    }
}
