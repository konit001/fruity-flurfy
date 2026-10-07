using SQLite;

// ของที่ซื้อ — 1 แถวต่อไอเทม, ShopItemId = ShopData.id
[Table("Inventory")]
public class InventoryRow
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed("PlayerItem", 0, Unique = true)]
    public int PlayerId { get; set; }

    [Indexed("PlayerItem", 1, Unique = true), MaxLength(50)]
    public string ShopItemId { get; set; }
    public int CurrentRun { get; set; }
    public int AllRun { get; set; }
}
