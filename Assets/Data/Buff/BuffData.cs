using UnityEngine;

public enum BuffStat { Damage, FireRate, BulletSpeed, MultiShot, Range, MoveSpeed, MaxHealth, Heal }

[CreateAssetMenu(fileName = "BuffData", menuName = "Buff/BuffData")]
public class BuffData : ScriptableObject
{
    [Header("Info")]
    public string id;
    public string _Name;
    public Sprite _Icon;
    [TextArea(3, 6)]
    public string _Description;

    [Header("Buff Type")]
    public BuffStat stat;
    public float value;
}