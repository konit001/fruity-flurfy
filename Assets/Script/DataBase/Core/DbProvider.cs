using UnityEngine;
using SQLite;
using System.IO;

// connection เดียวใช้ร่วมกันทั้งเกม แทนที่แต่ละสคริปต์จะเปิด SQLiteConnection ของตัวเอง
// (กัน "database is locked" ที่เกิดจากหลาย connection แย่ง lock กัน)
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
