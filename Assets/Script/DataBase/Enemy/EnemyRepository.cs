using SQLite;
using System.Linq;

public class EnemyRepository
{
    private SQLiteConnection db;

    public EnemyRepository(SQLiteConnection connection)
    {
        db = connection;
        db.CreateTable<EnemyStats>();
        SeedIfEmpty();
    }

    // ใส่ศัตรูชุดเริ่มต้นครั้งแรกที่ตารางยังว่าง
    private void SeedIfEmpty()
    {
        if (db.Table<EnemyStats>().Count() > 0) return;

        db.InsertAll(new[]
        {
            new EnemyStats { Name = "bat",   MaxHP = 5, ATK = 1, GoldDrop = 10 },
            new EnemyStats { Name = "Slime", MaxHP = 10, ATK = 2, GoldDrop = 20 },
        });
    }

    public EnemyStats GetByName(string name)
    {
        return db.Query<EnemyStats>(
            "SELECT * FROM Enemys WHERE Name = ? COLLATE NOCASE LIMIT 1", name).FirstOrDefault();
    }
}
