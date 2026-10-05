using System;
using System.Collections.Generic;
using UnityEngine;

public enum ShopCategory { Weapon, Material }

// ค่าที่ไอเทมเพิ่มให้ status — ใช้ความหมาย value เดียวกับ BuffManager.Apply
// Damage/MultiShot/MaxHealth = บวกเป็นจำนวนเต็ม, FireRate/BulletSpeed/Range/MoveSpeed = เปอร์เซ็นต์ (0.1 = +10%), Heal = ไม่ใช้ value
[Serializable]
public struct StatModifier
{
    public BuffStat stat;
    public float value;
}

[CreateAssetMenu(fileName = "ShopData", menuName = "Shop/ShopData")]
public class ShopData : ScriptableObject
{
    [Header("Info")]
    public string id;
    public string _Name;
    public ShopCategory category;
    public int _Price;
    public Sprite _Icon;
    [TextArea(3, 6)]
    public string _Description;

    [Header("Effect")]
    public List<StatModifier> modifiers = new List<StatModifier>();
}
