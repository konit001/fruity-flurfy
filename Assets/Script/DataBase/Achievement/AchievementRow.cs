using SQLite;

// ความคืบหน้า achievement — 1 แถวต่อ achievement, AchievementId = AchievementData.id
[Table("Achievements")]
public class AchievementRow
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed("PlayerAchievement", 0, Unique = true)]
    public int PlayerId { get; set; }

    [Indexed("PlayerAchievement", 1, Unique = true), MaxLength(50)]
    public string AchievementId { get; set; }

    public int Progress { get; set; }
    [MaxLength(30)]
    public string UnlockedAt { get; set; }
}
