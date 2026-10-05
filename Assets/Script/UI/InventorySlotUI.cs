using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 1 ช่องใน inventory: รูปไอเทม + จำนวนที่มี
public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text countText;

    public void Set(ShopData data, int count)
    {
        icon.sprite = data._Icon;
        icon.enabled = data._Icon != null;
        countText.text = count.ToString();
    }

    public void SetEmpty()
    {
        icon.sprite = null;
        icon.enabled = false;
        countText.text = "0";
    }

    // สร้างช่องแบบพื้นฐานด้วยโค้ด — ใช้เมื่อ InventoryManager ไม่ได้ใส่ slot prefab
    public static InventorySlotUI CreateDefault(Transform parent)
    {
        var root = new GameObject("Inventory Slot", typeof(RectTransform), typeof(Image));
        root.transform.SetParent(parent, false);
        root.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.25f);

        var iconGo = new GameObject("Icon", typeof(RectTransform), typeof(Image));
        iconGo.transform.SetParent(root.transform, false);
        Stretch((RectTransform)iconGo.transform, 8f);
        var iconImage = iconGo.GetComponent<Image>();
        iconImage.preserveAspect = true;
        iconImage.raycastTarget = false;

        var textGo = new GameObject("Count", typeof(RectTransform), typeof(TextMeshProUGUI));
        textGo.transform.SetParent(root.transform, false);
        Stretch((RectTransform)textGo.transform, 4f);
        var text = textGo.GetComponent<TextMeshProUGUI>();
        text.alignment = TextAlignmentOptions.BottomRight;
        text.fontSize = 28;
        text.raycastTarget = false;

        var slot = root.AddComponent<InventorySlotUI>();
        slot.icon = iconImage;
        slot.countText = text;
        return slot;
    }

    static void Stretch(RectTransform rt, float inset)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = new Vector2(inset, inset);
        rt.offsetMax = new Vector2(-inset, -inset);
    }
}
