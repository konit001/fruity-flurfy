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
        db.CreateTable<InventoryRow>();
    }

    public List<InventoryRow> GetActive(int playerId)
    {
        return db.Table<InventoryRow>().Where(r => r.PlayerId == playerId && r.CurrentRun > 0).ToList();
    }
    public List<InventoryRow> GetAll(int playerId)
    {
        return db.Table<InventoryRow>().Where(r => r.PlayerId == playerId).ToList();
    }

    public void SetupRow(int playerId, IEnumerable<string> itemIds)
    {
        db.RunInTransaction(() =>
        {
            var existing = GetAll(playerId).Select(r => r.ShopItemId).ToHashSet();

            db.InsertAll(itemIds.Distinct().Where(id => !existing.Contains(id)).Select(id => new InventoryRow
            {
                PlayerId = playerId,
                ShopItemId = id,
                CurrentRun = 0,
                AllRun = 0
            }));
        });
    }

    public void SaveQuantities(int playerId, IEnumerable<(string ItemId, int Quantity)> entries)
    {
        var current = entries.Where(e => e.Quantity > 0).ToDictionary(e => e.ItemId, e => e.Quantity);

        db.RunInTransaction(() =>
        {
            var rows = GetAll(playerId).ToList();

            foreach (var row in rows)
            {
                current.TryGetValue(row.ShopItemId, out var qty);
                row.AllRun += Mathf.Max(0, qty - row.CurrentRun);
                row.CurrentRun = qty;
            }
            db.UpdateAll(rows);

            var existing = rows.Select(r => r.ShopItemId).ToHashSet();
            db.InsertAll(current.Where(p => !existing.Contains(p.Key)).Select(p => new InventoryRow
            {
                PlayerId = playerId,
                ShopItemId = p.Key,
                CurrentRun = p.Value,
                AllRun = p.Value
            }));
        });
    }

    public void ResetRun(int playerId)
    {
        db.Execute("UPDATE Inventory SET CurrentRun = 0 WHERE PlayerId = ?", playerId);
    }
}
