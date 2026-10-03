using TMPro;
using UnityEngine;
using UnityEngine.UI;

// ช่องอัพเกรดที่ซื้อแล้ว (แถวล่างของหน้า Upgrade) — ไอคอน + xจำนวน
public class UpgradeSlotUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text countText;
    [SerializeField] private TMP_Text fallbackText; // แสดงชื่อเมื่อไม่มีไอคอน (optional)

    public void Set(Buff buff, int count)
    {
        var sprite = UpgradeIcons.Get(buff.Stat);
        if (icon != null)
        {
            icon.sprite = sprite;
            icon.enabled = sprite != null;
        }
        if (fallbackText != null)
            fallbackText.text = sprite == null ? buff.Name : "";
        if (countText != null)
            countText.text = count > 1 ? "x" + count : "";
    }

    public void Clear()
    {
        if (icon != null) { icon.sprite = null; icon.enabled = false; }
        if (fallbackText != null) fallbackText.text = "";
        if (countText != null) countText.text = "";
    }
}
