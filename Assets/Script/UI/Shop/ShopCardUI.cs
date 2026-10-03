using TMPro;
using UnityEngine;
using UnityEngine.UI;

// การ์ดอัพเกรด 1 ใบในร้าน (ซ้ายของหน้า Upgrade)
public class ShopCardUI : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private GameObject soldOverlay; // optional

    public Button Button => button;

    public void Set(Buff buff, int price, bool canAfford)
    {
        gameObject.SetActive(true);
        if (soldOverlay != null) soldOverlay.SetActive(false);

        if (nameText != null) nameText.text = buff.Name;
        if (descriptionText != null) descriptionText.text = buff.Description;
        if (priceText != null) priceText.text = price.ToString();

        var sprite = UpgradeIcons.Get(buff.Stat);
        if (icon != null)
        {
            icon.sprite = sprite;
            icon.enabled = sprite != null;
        }
        button.interactable = canAfford;
    }

    public void SetSold()
    {
        if (soldOverlay != null) soldOverlay.SetActive(true);
        else gameObject.SetActive(false);
        button.interactable = false;
    }

    public void Hide() => gameObject.SetActive(false);
}
