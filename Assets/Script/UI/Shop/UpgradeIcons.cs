using UnityEngine;

// ไอคอนอัพเกรดโหลดจาก Resources/UpgradeIcons/<Stat> เช่น Damage, FireRate (ไม่มีไฟล์ = ไม่มีไอคอน)
public static class UpgradeIcons
{
    public static Sprite Get(string stat) => Resources.Load<Sprite>("UpgradeIcons/" + stat);
}
