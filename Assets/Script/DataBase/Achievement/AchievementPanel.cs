using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 1 แถวของ achievement (prefab) — ใช้ทั้งในรายการและในป๊อปอัพตอนปลดล็อก
public class AchievementPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private Image iconImage;
    [SerializeField] private GameObject unlockedMark; // เปิดเมื่อปลดแล้ว (ปิดไว้ตอนเริ่ม)
    [SerializeField] private GameObject newMark;      // ป้าย NEW (ปิดไว้ตอนเริ่ม)

    public void Setup(AchievementData data, int progress, bool unlocked, bool isNew)
    {
        nameText.text = data._Name;
        descriptionText.text = data._Description;

        if (progressText != null)
            progressText.text = Mathf.Min(progress, data.target) + "/" + data.target;

        if (iconImage != null)
        {
            iconImage.sprite = data._Icon;
            iconImage.enabled = data._Icon != null;
        }

        if (unlockedMark != null) unlockedMark.SetActive(unlocked);
        if (newMark != null) newMark.SetActive(isNew);
    }
}
