using SQLite;

// ของที่ซื้อ — 1 แถวต่อไอเทมต่อผู้เล่น, composite PK (PlayerId, ItemId), ItemId = ShopData.id
[Table("Inventory")]
public class InventoryRow
{
    // composite PK (PlayerId, ItemId)
    public int PlayerId { get; set; }
    public int ItemId { get; set; }
    [MaxLength(50)]
    public string PlayerName { get; set; }
    [MaxLength(50)]
    public string ItemName { get; set; }
    public int CurrentRun { get; set; }
    public int AllRun { get; set; }
}
