using System.Linq;
using SQLite;

public class PlayerRepository
{
    private SQLiteConnection db;

    public PlayerRepository(SQLiteConnection connection)
    {
        db = connection;
        db.CreateTable<Players>();
    }

    public Players AddPlayer(string name)
    {
        var player = new Players { Name = name };
        db.Insert(player);
        return player;
    }

    public Players GetByName(string name)
    {
        return db.Table<Players>().Where(p => p.Name == name).FirstOrDefault();
    }

    public Players GetById(int id)
    {
        return db.Find<Players>(id);
    }

    // อัปเดตเวฟกับทองตอนเซฟ
    public void UpdateProgress(int id, int wave, int gold)
    {
        db.Execute("UPDATE Players SET Wave = ?, Gold = ? WHERE Id = ?", wave, gold, id);
    }

    public void AddKills(int id, int kills)
    {
        db.Execute("UPDATE Players SET Kill = Kill + ? WHERE Id = ?", kills, id);
    }
}
