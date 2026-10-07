using UnityEngine;

public enum AchievementMetric { Kills, BestWave, ItemsBought, BuffsPicked }

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
