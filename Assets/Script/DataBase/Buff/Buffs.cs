using SQLite;

[Table("PlayerBuffs")]
public class PlayerBuffRow
{
    [PrimaryKey, AutoIncrement]
    public int PlayerBuffId { get; set; }
    public int PlayerId { get; set; } // FK
    public int BuffId { get; set; } // FK
    [MaxLength(50)]
    public string BuffName { get; set; }
    public int CurrentRun { get; set; }
    public int AllRun { get; set; }
}
