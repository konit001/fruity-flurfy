using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HudManager : MonoBehaviour
{
    public List<EquipmentData> equippedTools = new List<EquipmentData>();

    [Header("Always-visible HUD hotbar")]
    public RectTransform SlotLayout;
    public GameObject SlotPrefab;

    [Header("Mirror inside Inventory panel (optional)")]
    public RectTransform InventorySlotLayout;
    public GameObject InventorySlotPrefab;

    private List<EquipmentSlotUI> SlotList = new List<EquipmentSlotUI>();
    private List<EquipmentSlotUI> InventorySlotList = new List<EquipmentSlotUI>();
    private int selectedIndex;

    void Start()
    {
        SetupSlot();
        SelectIndex(0);
    }

    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.digit1Key.wasPressedThisFrame) SelectIndex(0);
        else if (keyboard.digit2Key.wasPressedThisFrame) SelectIndex(1);
        else if (keyboard.digit3Key.wasPressedThisFrame) SelectIndex(2);
        else if (keyboard.digit4Key.wasPressedThisFrame) SelectIndex(3);
        else if (keyboard.digit5Key.wasPressedThisFrame) SelectIndex(4);
    }

    private void SetupSlot()
    {
        foreach (var data in equippedTools)
        {
            var slotUI = InstantiateSlot(SlotPrefab, SlotLayout, data);
            if (slotUI != null) SlotList.Add(slotUI);

            var inventorySlotUI = InstantiateSlot(InventorySlotPrefab, InventorySlotLayout, data);
            if (inventorySlotUI != null) InventorySlotList.Add(inventorySlotUI);
        }
    }

    private EquipmentSlotUI InstantiateSlot(GameObject prefab, RectTransform layout, EquipmentData data)
    {
        if (prefab == null || layout == null) return null;

        GameObject newSlot = Instantiate(prefab, layout);
        var slotUI = newSlot.GetComponent<EquipmentSlotUI>();
        if (slotUI == null)
        {
            Debug.LogError($"HudManager: prefab '{prefab.name}' has no EquipmentSlotUI component.", prefab);
            return null;
        }
        if (data == null)
        {
            Debug.LogError("HudManager: equippedTools contains an empty (null) entry.", this);
            return slotUI;
        }
        slotUI.Set(data);
        return slotUI;
    }

    public void SelectIndex(int index)
    {
        if (index < 0 || index >= equippedTools.Count) return;

        selectedIndex = index;
        RefreshUI();
    }

    private void RefreshUI()
    {
        for (int i = 0; i < SlotList.Count; i++)
            SlotList[i].SetSelected(i == selectedIndex);

        for (int i = 0; i < InventorySlotList.Count; i++)
            InventorySlotList[i].SetSelected(i == selectedIndex);
    }
}
