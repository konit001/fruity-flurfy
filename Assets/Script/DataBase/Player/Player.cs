using SQLite;

[Table("Players")]
public class Player
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [MaxLength(50)]
    public string Name { get; set; }

    public int Level { get; set; } = 1;
    public int Wave { get; set; } = 1;
    public int Gold { get; set; } = 0;
    public int Kill { get; set; } = 0;

}
