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

    // จำนวนที่ถือในรอบปัจจุบัน — ตายแล้วตั้งเป็น 0 (ไม่ลบแถว)
    public int Quantity { get; set; }

    // จำนวนครั้งที่ซื้อตลอดกาล — ไม่รีเซ็ต
    public int TotalCount { get; set; }
}
