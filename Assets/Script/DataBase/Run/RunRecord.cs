using SQLite;

[Table("RunHistory")]
public class RunRecord
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int PlayerId { get; set; }

    public int WaveReached { get; set; }
    public int Kills { get; set; }
    public int GoldEarned { get; set; }

    [MaxLength(30)]
    public string EndedAt { get; set; } // ISO 8601
}
