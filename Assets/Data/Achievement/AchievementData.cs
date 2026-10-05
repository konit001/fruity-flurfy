using UnityEngine;

// ค่าที่ใช้วัดความคืบหน้าของ achievement (นับตลอดกาลของผู้เล่น)
public enum AchievementMetric { Kills, BestWave, ItemsBought, BuffsPicked }

// นิยาม achievement — ลากเข้า list "achievements" ของ AchievementUI
// DB เก็บแค่ความคืบหน้า (ตาราง Achievements) ใช้ id เป็น key ห้ามแก้ id หลังใช้งาน
[CreateAssetMenu(fileName = "AchievementData", menuName = "Achievement/AchievementData")]
public class AchievementData : ScriptableObject
{
    [Header("Info")]
    public string id;
    public string _Name;
    public Sprite _Icon;
    [TextArea(3, 6)]
    public string _Description;

    [Header("Goal")]
    public AchievementMetric metric;
    public int target = 1;
}
