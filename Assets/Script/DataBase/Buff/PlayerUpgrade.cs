using SQLite;

// อัพเกรดที่ผู้เล่นซื้อจากร้านในรันปัจจุบัน — 1 แถวต่อ Stat, Quantity = จำนวนครั้งที่ซื้อ
[Table("PlayerUpgrades")]
public class PlayerUpgrade
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int PlayerId { get; set; }

    [MaxLength(30)]
    public string Stat { get; set; }

    public int Quantity { get; set; }
}
