using SQLite;
using System.Linq;

public class SaveRepository
{
    private SQLiteConnection db;

    public SaveRepository(SQLiteConnection connection)
    {
        db = connection;
        db.CreateTable<SaveData>();
    }

    // ลบแถวเก่าของ player คนนี้แล้ว insert แถวใหม่ (เก็บแค่เซฟล่าสุด ตารางไม่โตตามจำนวนเวฟ)
    public void Save(int playerId, string sceneName, int day, int gold, float x, float y, float z)
    {
        db.RunInTransaction(() =>
        {
            db.Execute("DELETE FROM SaveData WHERE PlayerId = ?", playerId);
            db.Insert(new SaveData
            {
                PlayerId = playerId,
                SceneName = sceneName,
                Day = day,
                Gold = gold,
                PosX = x,
                PosY = y,
                PosZ = z
            });
        });
    }

    // เอาแถวล่าสุดของ player คนนี้
    public SaveData Load(int playerId)
    {
        return db.Query<SaveData>(
            "SELECT * FROM SaveData WHERE PlayerId = ? ORDER BY Id DESC LIMIT 1", playerId).FirstOrDefault();
    }
}
