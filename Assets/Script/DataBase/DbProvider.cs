using System.IO;
using SQLite;
using UnityEngine;

// connection เดียวใช้ร่วมกันทั้งเกม กัน "database is locked" จากหลาย connection แย่ง lock กัน
public static class DbProvider
{
    private static SQLiteConnection db;

    public static SQLiteConnection Connection
    {
        get
        {
            if (db == null)
            {
                var path = Path.Combine(Application.persistentDataPath, "game.db");
                db = new SQLiteConnection(path);
            }
            return db;
        }
    }
}
