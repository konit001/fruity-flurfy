using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Button button;
    [SerializeField] private GameObject soldLabel; // ป้าย Sold (ปิดไว้ตอนเริ่ม)

    public ShopData Data { get; private set; }

    public void Setup(ShopData data, Func<ShopData, bool> onBuy)
    {
        Data = data;
        nameText.text = data._Name;
        descriptionText.text = data._Description;
        priceText.text = data._Price.ToString();

        iconImage.sprite = data._Icon;
        iconImage.enabled = data._Icon != null;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onBuy(data));
    }

    public void SetAffordable(bool affordable) => button.interactable = affordable;

    // ขึ้นป้าย Sold เมื่อซื้อแล้ว
    public void SetSold(bool sold)
    {
        if (soldLabel != null) soldLabel.SetActive(sold);
    }
}
