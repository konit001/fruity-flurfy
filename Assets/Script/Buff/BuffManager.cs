using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    [Header("Data")]
    public List<BuffData> buffList = new List<BuffData>();
    public int choiceCount = 3;

    [Header("UI")]
    public GameObject BuffUi;
    public Transform buffPanelParent;
    public BuffPanel buffPanelPrefab;

    private PlayerAttack playerAttack;
    private PlayerControl playerControl;
    private Health playerHealth;
    private Action onPicked;
    private readonly Dictionary<string, int> picked = new Dictionary<string, int>();

    void Awake()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        playerAttack = player.GetComponent<PlayerAttack>();
        playerControl = player.GetComponent<PlayerControl>();
        playerHealth = player.GetComponent<Health>();

        BuffUi.SetActive(false);
    }

    // หยุดเกมแล้วโชว์ buff ให้เลือก — เลือกเสร็จเรียก picked
    public void Show(Action picked = null)
    {
        onPicked = picked;
        RefreshUi();
        BuffUi.SetActive(true);
        Time.timeScale = 0f;
    }

    void RefreshUi()
    {
        foreach (Transform child in buffPanelParent)
            Destroy(child.gameObject);

        foreach (BuffData buff in RollBuffs())
        {
            BuffPanel panel = Instantiate(buffPanelPrefab, buffPanelParent);
            panel.Setup(buff, Pick);
        }
    }

    // สุ่มไม่ซ้ำกัน
    List<BuffData> RollBuffs()
    {
        return buffList.OrderBy(_ => UnityEngine.Random.value).Take(choiceCount).ToList();
    }

    void Pick(BuffData buff)
    {
        Apply(buff);

        BuffUi.SetActive(false);
        Time.timeScale = 1f;
        onPicked?.Invoke();
    }

    public void Apply(BuffData buff)
    {
        ApplyStat(buff.stat, buff.value);
        picked[buff.id] = GetPickedCount(buff.id) + 1;
        Debug.Log($"Buff: {buff._Name} ({buff._Description})");
    }

    // buff ที่เลือกในรอบนี้ (key = BuffData.id, value = จำนวนครั้ง) — ใช้ตอนเซฟ
    public IReadOnlyDictionary<string, int> Picked => picked;

    int GetPickedCount(string buffId) => picked.TryGetValue(buffId, out int n) ? n : 0;

    // ใส่ buff กลับตามเซฟ (Heal เป็นผลครั้งเดียว ไม่ใส่ซ้ำ) — ไม่เจอ id ใน buffList ข้ามไป
    public void Restore(string buffId, int quantity)
    {
        BuffData buff = buffList.FirstOrDefault(b => b.id == buffId);
        if (buff == null) return;

        if (buff.stat != BuffStat.Heal)
        {
            for (int i = 0; i < quantity; i++)
                ApplyStat(buff.stat, buff.value);
        }
        picked[buffId] = GetPickedCount(buffId) + quantity;
    }

    // ใช้ร่วมกับ Shop
    public void ApplyStat(BuffStat stat, float value)
    {
        switch (stat)
        {
            case BuffStat.Damage:      playerAttack.damage += Mathf.RoundToInt(value); break;
            case BuffStat.FireRate:    playerAttack.fireRate *= 1f + value; break;
            case BuffStat.BulletSpeed: playerAttack.bulletSpeed *= 1f + value; break;
            case BuffStat.MultiShot:   playerAttack.projectileCount += Mathf.RoundToInt(value); break;
            case BuffStat.Range:       playerAttack.range *= 1f + value; break;
            case BuffStat.MoveSpeed:   playerControl.speed *= 1f + value; break;
            case BuffStat.MaxHealth:   playerHealth.AddMaxHealth(Mathf.RoundToInt(value)); break;
            case BuffStat.Heal:        playerHealth.HealFull(); break;
        }
    }
}