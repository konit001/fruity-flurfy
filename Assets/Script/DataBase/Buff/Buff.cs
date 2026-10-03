using SQLite;

// ตาราง buff ที่สุ่มให้เลือกตอนจบเวฟ — เพิ่ม/แก้ได้ใน DB Browser
// Stat ที่รองรับ: Damage, FireRate, BulletSpeed, MultiShot, Range, MoveSpeed, MaxHealth, Heal
[Table("Buffs")]
public class Buff
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [MaxLength(50)]
    public string Name { get; set; }

    [MaxLength(200)]
    public string Description { get; set; }

    [MaxLength(30)]
    public string Stat { get; set; }

    public float Value { get; set; }

    public int Price { get; set; } // ราคาพื้นฐานในร้านอัพเกรด (คูณตามเวฟอีกที)
}
