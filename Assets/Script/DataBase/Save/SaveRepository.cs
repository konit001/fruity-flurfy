using System.Linq;
using SQLite;

public class SaveRepository
{
    private SQLiteConnection db;

    public SaveRepository(SQLiteConnection connection)
    {
        db = connection;
        db.CreateTable<SaveData>();
    }

    public void Save(int playerId, string sceneName, int wave, int gold)
    {
        db.RunInTransaction(() =>
        {
            db.Execute("DELETE FROM SaveData WHERE PlayerId = ?", playerId);

            var data = new SaveData
            {
                PlayerId = playerId,
                SceneName = sceneName,
                Wave = wave,
                Gold = gold
            };
            db.Insert(data);
        });
    }

    public SaveData Load(int playerId)
    {
        return db.Table<SaveData>().Where(s => s.PlayerId == playerId).FirstOrDefault();
    }

    public void Clear(int playerId)
    {
        db.Execute("DELETE FROM SaveData WHERE PlayerId = ?", playerId);
    }
}
