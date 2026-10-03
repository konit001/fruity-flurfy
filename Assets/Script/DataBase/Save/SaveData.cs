using SQLite;

[Table("SaveData")]
public class SaveData
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int PlayerId { get; set; }

    [MaxLength(50)]
    public string SceneName { get; set; }

    public int Day { get; set; } = 1;
    public int Gold { get; set; } = 0;
    public float PosX { get; set; }
    public float PosY { get; set; }
    public float PosZ { get; set; }
}
