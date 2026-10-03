using UnityEngine;
using TMPro;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }

    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text goldUiText;
    [SerializeField] private TMP_Text shopGoldText;

    // ทองเก็บในหน่วยความจำ รีเซ็ตเมื่อปิดเกม (ไม่ผูกกับ DB)
    private int gold;

    void Awake() => Instance = this;

    void Start()
    {
        RefreshGoldUI();
    }

    public void AddGold(int amount)
    {
        if (amount == 0) return;

        gold = Mathf.Max(0, gold + amount);
        RefreshGoldUI(gold);
    }

    // หักเงินถ้ามีพอ — คืน true เมื่อจ่ายสำเร็จ
    public bool TrySpend(int amount)
    {
        if (amount <= 0) return true;
        if (gold < amount) return false;

        AddGold(-amount);
        return true;
    }

    public int GetGold() => gold;

    public void RefreshGoldUI(int? value = null)
    {
        string text = (value ?? gold).ToString();
        if (goldText != null) goldText.text = text;
        if (goldUiText != null) goldUiText.text = text;
        if (shopGoldText != null) shopGoldText.text = text;
    }
}
