using UnityEngine;
using UnityEngine.InputSystem;

public class UiManager : MonoBehaviour
{
    public GameObject setting;
    public bool isSetting;

    void Awake()
    {
        setting.SetActive(false);
        isSetting = false;
    }
    void Update()
    {
        UpdateUi();
    }

    private void UpdateUi()
    {

        if (Keyboard.current.escapeKey.wasPressedThisFrame && isSetting == false)
        {
            isSetting = true;
            setting.SetActive(isSetting);
        }
        else if (Keyboard.current.escapeKey.wasPressedThisFrame && isSetting == true)
        {
            isSetting = false;
            setting.SetActive(isSetting);
        }
    }
}
