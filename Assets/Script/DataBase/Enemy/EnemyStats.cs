using SQLite;

// ตารางค่าสถานะศัตรู — แก้ได้ใน DB Browser แล้วมีผลตอนศัตรูเกิด (จับคู่ด้วย Name = ชื่อ prefab)
// ค่า <= 0 หมายถึงใช้ค่าจาก prefab แทน
[Table("Enemys")]
public class EnemyStats
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [MaxLength(50)]
    public string Name { get; set; }

    public int MaxHP { get; set; }
    public int ATK { get; set; }        // ดาเมจตอนชนผู้เล่น
    public int GoldDrop { get; set; }   // ทองที่ดรอปตายตัว (0 = สุ่มตามช่วง goldMin/goldMax บน prefab)
}
