using UnityEngine;
using UnityEngine.UI;

public class MapUiSlot : MonoBehaviour
{
    public Image _icon;
    public Image _background;

    public void Setup(Sprite iconSprite)
    {
        if (iconSprite != null)
        {
            _icon.sprite = iconSprite;
            _icon.gameObject.SetActive(true);
        }
        else
        {
            _icon.gameObject.SetActive(false);
        }
    }
}