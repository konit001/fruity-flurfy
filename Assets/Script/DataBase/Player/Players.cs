using SQLite;

[Table("Players")]
public class Players
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [MaxLength(50), Unique]
    public string Name { get; set; }

    public int Wave { get; set; } = 1;
    public int Gold { get; set; } = 0;
    public int Kill { get; set; } = 0;
}
