using System.Collections.Generic;
using System.Linq;
using SQLite;
using UnityEngine;

public class InventoryRepository
{
    private SQLiteConnection db;

    public InventoryRepository(SQLiteConnection connection)
    {
        db = connection;
        // สร้างเพื่อให้ได้ composite PK ก่อน CreateTable
        db.Execute(@"CREATE TABLE IF NOT EXISTS Inventory (
            PlayerId   INTEGER NOT NULL,
            ItemId     INTEGER NOT NULL,
            PlayerName VARCHAR(50),
            ItemName   VARCHAR(50),
            CurrentRun INTEGER NOT NULL DEFAULT 0,
            AllRun     INTEGER NOT NULL DEFAULT 0,
            PRIMARY KEY (PlayerId, ItemId))");
        db.CreateTable<InventoryRow>();
    }

    private string GetPlayerName(int playerId)
    {
        return db.Find<Players>(playerId)?.Name;
    }

    //  Inventory table
    public List<InventoryRow> GetActive(int playerId)
    {
        return db.Table<InventoryRow>().Where(r => r.PlayerId == playerId && r.CurrentRun > 0).ToList();
    }
    public List<InventoryRow> GetAll(int playerId)
    {
        return db.Table<InventoryRow>().Where(r => r.PlayerId == playerId).ToList();
    }

    // สร้างแถว (0/0) ให้ไอเทมที่ผู้เล่นยังไม่มี
    public void SetupRow(int playerId, IEnumerable<ShopData> items)
    {
        db.RunInTransaction(() =>
        {
            var existing = GetAll(playerId).Select(r => r.ItemId).ToHashSet();
            var playerName = GetPlayerName(playerId);

            db.InsertAll(items.Where(i => i != null && !existing.Contains(i.id)).Select(i => new InventoryRow
            {
                PlayerId = playerId,
                ItemId = i.id, // ดึง id จาก ShopData มาเป็น key ใน DB
                PlayerName = playerName,
                ItemName = i._Name,
                CurrentRun = 0,
                AllRun = 0
            }));
        });
    }

    public void SaveQuantities(int playerId, IEnumerable<(ShopData Item, int Quantity)> entries)
    {
        var current = entries.Where(e => e.Quantity > 0).ToDictionary(e => e.Item.id); // key = ShopData.id ใช้เทียบกับ ItemId ใน DB

        db.RunInTransaction(() =>
        {
            var rows = GetAll(playerId);

            foreach (var row in rows)
            {
                int qty = current.TryGetValue(row.ItemId, out var e) ? e.Quantity : 0;
                int gained = Mathf.Max(0, qty - row.CurrentRun);
                
                db.Execute("UPDATE Inventory SET CurrentRun = ?, AllRun = AllRun + ? WHERE PlayerId = ? AND ItemId = ?",
                    qty, gained, playerId, row.ItemId);
            }

            var existing = rows.Select(r => r.ItemId).ToHashSet();
            var playerName = GetPlayerName(playerId);

            db.InsertAll(current.Values.Where(e => !existing.Contains(e.Item.id)).Select(e => new InventoryRow
            {
                PlayerId = playerId,
                ItemId = e.Item.id, // ดึง id จาก ShopData มาเป็น key ใน DB
                PlayerName = playerName,
                ItemName = e.Item._Name,
                CurrentRun = e.Quantity,
                AllRun = e.Quantity
            }));
        });
    }

    public void ResetRun(int playerId)
    {
        db.Execute("UPDATE Inventory SET CurrentRun = 0 WHERE PlayerId = ?", playerId);
    }
}
