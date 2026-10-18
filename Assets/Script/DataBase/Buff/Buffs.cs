using SQLite;

// buff ที่ผู้เล่นเลือก — composite PK (PlayerId, BuffId), BuffId = BuffData.id
[Table("PlayerBuffs")]
public class PlayerBuffRow
{
    // composite PK (PlayerId, BuffId)
    public int PlayerId { get; set; }
    public int BuffId { get; set; }
    [MaxLength(50)]
    public string PlayerName { get; set; }
    [MaxLength(50)]
    public string BuffName { get; set; }
    public int CurrentRun { get; set; }
    public int AllRun { get; set; }
}
