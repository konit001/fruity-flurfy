using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlotUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private GameObject highlight;

    public void Set(EquipmentData data)
    {
        if (icon == null)
        {
            Debug.LogError($"EquipmentSlotUI on '{name}' has no icon Image assigned.", this);
            return;
        }
        icon.sprite = data.icon;
        icon.enabled = data.icon != null;
    }

    public void SetSelected(bool selected)
    {
        if (highlight != null) highlight.SetActive(selected);
    }
}
