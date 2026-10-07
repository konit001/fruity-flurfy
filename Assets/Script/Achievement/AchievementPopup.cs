using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementPopup : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Image iconImage;

    public void Setup(AchievementData data)
    {
        nameText.text = data._Name;

        if (iconImage != null)
        {
            iconImage.sprite = data._Icon;
            iconImage.enabled = data._Icon != null;
        }
    }
}
