using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// รายการ buff ทั้งหมดเขียนในโค้ด (แทนตาราง Buffs) — Price = ราคาพื้นฐานในร้าน
// Stat ที่รองรับ: Damage, FireRate, BulletSpeed, MultiShot, Range, MoveSpeed, MaxHealth, Heal
public static class BuffCatalog
{
    private static readonly List<Buff> all = new List<Buff>
    {
        new Buff { Name = "Sharp Shot",  Description = "Damage +1",         Stat = "Damage",      Value = 1,     Price = 10 },
        new Buff { Name = "Quick Hands", Description = "Fire Rate +20%",    Stat = "FireRate",    Value = 0.2f,  Price = 12 },
        new Buff { Name = "Fast Bullet", Description = "Bullet Speed +25%", Stat = "BulletSpeed", Value = 0.25f, Price = 8 },
        new Buff { Name = "Swift Feet",  Description = "Move Speed +15%",   Stat = "MoveSpeed",   Value = 0.15f, Price = 8 },
        new Buff { Name = "Tough Skin",  Description = "Max HP +2",         Stat = "MaxHealth",   Value = 2,     Price = 10 },
        new Buff { Name = "Fresh Fruit", Description = "Heal to full HP",   Stat = "Heal",        Value = 0,     Price = 6 },
        new Buff { Name = "Wide Range",  Description = "Fire range +20%",   Stat = "Range",       Value = 0.2f,  Price = 10 },
        new Buff { Name = "Multi Shot",  Description = "+1 bullet per shot", Stat = "MultiShot",  Value = 1,     Price = 25 },
    };

    public static List<Buff> GetAll() => new List<Buff>(all);

    // สุ่ม buff ไม่ซ้ำกัน count อัน
    public static List<Buff> GetRandom(int count)
    {
        return all.OrderBy(_ => Random.value).Take(count).ToList();
    }
}
