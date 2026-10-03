using UnityEngine;
using SQLite;
using System.Linq;
using System.Collections.Generic;

public class PlayerRepository
{
    private SQLiteConnection db;

    public PlayerRepository(SQLiteConnection connection)
    {
        db = connection;
        db.CreateTable<Player>();
    }

    public Player AddPlayer(string playerName)
    {
        var player = new Player { Name = playerName };
        db.Insert(player);
        return player;
    }

    public List<Player> GetAllPlayers()
    {
        return db.Query<Player>("SELECT * FROM Players");
    }

    public void DeleteAll()
    {
        db.DeleteAll<Player>();
    }

    public Player GetByName(string name)
    {
        return db.Query<Player>("SELECT * FROM Players WHERE Name = ? ORDER BY Id DESC LIMIT 1", name).FirstOrDefault();
    }

    public Player GetById(int id)
    {
        return db.Query<Player>("SELECT * FROM Players WHERE Id = ?", id).FirstOrDefault();
    }

    public void Update(int id, int gold, int wave, int level)
    {
        db.Execute("UPDATE Players SET Gold = ?, Wave = ? , Level = ? WHERE Id = ?", gold, wave, level, id);
    }

    public void UpdateGold(int id, int gold)
    {
        db.Execute("UPDATE Players SET Gold = ? WHERE Id = ?", gold, id);
    }

    public void UpdateLevel(int id, int level)
    {
        db.Execute("UPDATE Players SET Level = ? WHERE Id = ?", level, id);
    }

    public void UpdateWave(int id, int wave)
    {
        db.Execute("UPDATE Players SET Wave = ? WHERE Id = ?", wave, id);
    }

    public void UpdateKill(int id, int kill)
    {
        db.Execute("UPDATE Players SET Kill = ? WHERE Id = ?", kill, id);
    }


}
