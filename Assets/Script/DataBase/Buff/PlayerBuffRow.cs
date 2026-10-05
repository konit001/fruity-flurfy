using SQLite;

[Table("PlayerBuffs")]
public class PlayerBuffRow
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed("PlayerBuff", 0, Unique = true)]
    public int PlayerId { get; set; }

    [Indexed("PlayerBuff", 1, Unique = true), MaxLength(50)]
    public string BuffId { get; set; }
    public int Quantity { get; set; }
    public int TotalCount { get; set; }
}
