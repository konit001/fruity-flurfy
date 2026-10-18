using System.IO;
using SQLite;
using UnityEngine;

public static class DbProvider
{
    private static SQLiteConnection db;

    public static SQLiteConnection Connection
    {
        get
        {
            var path = Path.Combine(Application.persistentDataPath, "game.db");
            db = new SQLiteConnection(path);
            Debug.Log("db path: " + path);
            return db;
        }
    }
}
