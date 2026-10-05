using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Button button;

    public void Setup(BuffData data, Action<BuffData> onClick)
    {
        nameText.text = data._Name;
        descriptionText.text = data._Description;

        iconImage.sprite = data._Icon;
        iconImage.enabled = data._Icon != null;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick(data));
    }
}