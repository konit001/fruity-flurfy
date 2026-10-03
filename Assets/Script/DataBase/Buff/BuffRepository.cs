using SQLite;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class BuffRepository
{
    private SQLiteConnection db;

    public BuffRepository(SQLiteConnection connection)
    {
        db = connection;
        db.CreateTable<Buff>();
        SeedIfEmpty();
        db.Execute("DELETE FROM Buffs WHERE Stat = ?", "ArcAngle"); // ล้าง buff ที่เลิกใช้แล้ว
        EnsureBuff(new Buff { Name = "Wide Range", Description = "Fire range +20%", Stat = "Range", Value = 0.2f });
        EnsureBuff(new Buff { Name = "Multi Shot", Description = "+1 bullet per shot", Stat = "MultiShot", Value = 1 });

        // ราคาเริ่มต้นสำหรับร้านอัพเกรด (เติมเฉพาะแถวที่ยังเป็น 0 — แก้ราคาใน DB Browser ได้)
        SetDefaultPrice("Damage", 10);
        SetDefaultPrice("FireRate", 12);
        SetDefaultPrice("BulletSpeed", 8);
        SetDefaultPrice("MoveSpeed", 8);
        SetDefaultPrice("MaxHealth", 10);
        SetDefaultPrice("Heal", 6);
        SetDefaultPrice("Range", 10);
        SetDefaultPrice("MultiShot", 25);
    }

    private void SetDefaultPrice(string stat, int price)
    {
        db.Execute("UPDATE Buffs SET Price = ? WHERE Stat = ? AND Price = 0", price, stat);
    }

    // เติม buff ใหม่ให้ DB เก่าที่ seed ไปแล้ว (เช็คจาก Stat)
    private void EnsureBuff(Buff buff)
    {
        if (db.Table<Buff>().Where(b => b.Stat == buff.Stat).Count() == 0)
            db.Insert(buff);
    }

    // ใส่ buff ชุดเริ่มต้นครั้งแรกที่ตารางยังว่าง
    private void SeedIfEmpty()
    {
        if (db.Table<Buff>().Count() > 0) return;

        db.InsertAll(new[]
        {
            new Buff { Name = "Sharp Shot",  Description = "Damage +1",        Stat = "Damage",      Value = 1 },
            new Buff { Name = "Quick Hands", Description = "Fire Rate +20%",   Stat = "FireRate",    Value = 0.2f },
            new Buff { Name = "Fast Bullet", Description = "Bullet Speed +25%", Stat = "BulletSpeed", Value = 0.25f },
            new Buff { Name = "Swift Feet",  Description = "Move Speed +15%",  Stat = "MoveSpeed",   Value = 0.15f },
            new Buff { Name = "Tough Skin",  Description = "Max HP +2",         Stat = "MaxHealth",   Value = 2 },
            new Buff { Name = "Fresh Fruit", Description = "Heal to full HP",   Stat = "Heal",        Value = 0 },
        });
    }

    public List<Buff> GetAll()
    {
        return db.Table<Buff>().ToList();
    }

    // สุ่ม buff ไม่ซ้ำกัน count อัน
    public List<Buff> GetRandom(int count)
    {
        return GetAll().OrderBy( x => Random.value).Take(count).ToList();
    }
}
