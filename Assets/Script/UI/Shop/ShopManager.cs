using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// หน้าร้านอัพเกรด: จบเวฟ (หลังเลือก buff ฟรี) → เปิดร้าน → ซื้อด้วยเงิน → NEXT WAVE
public class ShopManager : MonoBehaviour
{
    private const string HealStat = "Heal"; // ใช้ทันที ไม่เก็บลงช่อง

    [Header("UI")]
    [SerializeField] private GameObject panel;
    [SerializeField] private ShopCardUI[] cards;
    [SerializeField] private UpgradeSlotUI[] upgradeSlots;
    [SerializeField] private Button rerollButton;
    [SerializeField] private TMP_Text rerollText;
    [SerializeField] private Button nextWaveButton;

    [Header("Price")]
    [SerializeField] private float priceWaveScale = 0.25f; // ราคา = base x (1 + scale x (wave-1))
    [SerializeField] private int rerollBase = 2;
    [SerializeField] private int rerollStep = 2;           // แพงขึ้นทุกครั้งที่ reroll ในร้านเดียว

    [Header("Refs")]
    [SerializeField] private BuffManager buffManager;

    private Action onNext;
    private int wave = 1;
    private int rerolls;

    private Buff[] offers;
    private int[] prices;
    private bool[] sold;

    // อัพเกรดที่ซื้อแล้ว: ลำดับตามที่ซื้อครั้งแรก (แสดงใน upgradeSlots) + จำนวนต่อ stat
    private readonly List<Buff> ownedOrder = new List<Buff>();
    private readonly Dictionary<string, int> ownedCount = new Dictionary<string, int>();

    void Awake()
    {
        offers = new Buff[cards.Length];
        prices = new int[cards.Length];
        sold = new bool[cards.Length];

        for (int i = 0; i < cards.Length; i++)
        {
            int index = i;
            cards[i].Button.onClick.AddListener(() => Buy(index));
        }
        if (rerollButton != null) rerollButton.onClick.AddListener(Reroll);
        if (nextWaveButton != null) nextWaveButton.onClick.AddListener(NextWave);

        panel.SetActive(false);
    }

    public void Open(int currentWave, Action next)
    {
        wave = currentWave;
        onNext = next;
        rerolls = 0;

        Roll();
        panel.SetActive(true);
        Time.timeScale = 0f;
        RefreshAll();
    }

    void NextWave()
    {
        panel.SetActive(false);
        Time.timeScale = 1f;
        onNext?.Invoke();
    }

    void Roll()
    {
        // ช่องอัพเกรดเต็มแล้ว → ขายเฉพาะชนิดที่มีอยู่ (หรือ Heal) ไม่ให้ซื้อของที่ไม่มีช่องแสดง
        bool full = ownedOrder.Count >= upgradeSlots.Length;
        var pool = BuffCatalog.GetAll()
            .Where(b => b.Price > 0)
            .Where(b => !full || b.Stat == HealStat || ownedCount.ContainsKey(b.Stat))
            .OrderBy(_ => UnityEngine.Random.value)
            .Take(cards.Length)
            .ToList();

        for (int i = 0; i < cards.Length; i++)
        {
            sold[i] = false;
            offers[i] = i < pool.Count ? pool[i] : null;
            prices[i] = offers[i] != null ? PriceOf(offers[i]) : 0;
        }
    }

    int PriceOf(Buff buff) => Mathf.CeilToInt(buff.Price * (1f + priceWaveScale * (wave - 1)));

    int RerollCost => rerollBase + rerollStep * rerolls + wave;

    void Buy(int index)
    {
        if (sold[index] || offers[index] == null) return;

        var money = MoneyManager.Instance;
        if (money == null)
        {
            Debug.LogWarning("ShopManager: ไม่มี MoneyManager ในฉาก ซื้อไม่ได้");
            return;
        }
        if (!money.TrySpend(prices[index])) return;

        var buff = offers[index];
        buffManager.Apply(buff);
        Own(buff);

        sold[index] = true;
        RefreshAll();
    }

    void Own(Buff buff)
    {
        if (buff.Stat == HealStat) return;

        if (ownedCount.ContainsKey(buff.Stat)) ownedCount[buff.Stat]++;
        else
        {
            ownedCount[buff.Stat] = 1;
            ownedOrder.Add(buff);
        }
    }

    void Reroll()
    {
        var money = MoneyManager.Instance;
        if (money == null || !money.TrySpend(RerollCost)) return;

        rerolls++;
        Roll();
        RefreshAll();
    }

    void RefreshAll()
    {
        int gold = 0;
        if (MoneyManager.Instance != null)
        {
            gold = MoneyManager.Instance.GetGold();
            MoneyManager.Instance.RefreshGoldUI(gold);
        }

        for (int i = 0; i < cards.Length; i++)
        {
            if (offers[i] == null) { cards[i].Hide(); continue; }

            cards[i].Set(offers[i], prices[i], gold >= prices[i]);
            if (sold[i]) cards[i].SetSold();
        }

        if (rerollButton != null) rerollButton.interactable = gold >= RerollCost;
        if (rerollText != null) rerollText.text = RerollCost.ToString();

        for (int i = 0; i < upgradeSlots.Length; i++)
        {
            if (i < ownedOrder.Count)
                upgradeSlots[i].Set(ownedOrder[i], ownedCount[ownedOrder[i].Stat]);
            else
                upgradeSlots[i].Clear();
        }
    }
}
