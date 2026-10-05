using SQLite;

[Table("SaveData")]
public class SaveData
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Unique]
    public int PlayerId { get; set; }

    [MaxLength(50)]
    public string SceneName { get; set; }

    public int Wave { get; set; } = 1;
    public int Gold { get; set; } = 0;
}
