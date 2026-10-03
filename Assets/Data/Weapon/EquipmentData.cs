using UnityEngine;

// เลิกใช้การเปลี่ยนเครื่องมือแล้ว (ยิงอย่างเดียว) — เก็บ enum ไว้ไม่ให้ asset เดิมพัง
public enum ToolType { Hand, Axe, Pickaxe, Scythe, WateringCan }

[CreateAssetMenu(fileName = "EquipmentData", menuName = "Equipment/EquipmentData")]
public class EquipmentData : ScriptableObject
{
    public ToolType type;
    public string displayName;
    public Sprite icon;
}
