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
    private readonly Dictionary<int, int> picked = new Dictionary<int, int>();
    private PlayerBuffRepository repo;
    private Dictionary<int, BuffData> buffsById;    // key = BuffData.id

    void Awake()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        playerAttack = player.GetComponent<PlayerAttack>();
        playerControl = player.GetComponent<PlayerControl>();
        playerHealth = player.GetComponent<Health>();

        BuffUi.SetActive(false);
        LoadDefinitions();
    }

    void LoadDefinitions()
    {
        repo = new PlayerBuffRepository(DbProvider.Connection);
        // ใช้ BuffData.id เป็น key เพื่อ map BuffId ใน DB กลับมาเป็น BuffData
        buffsById = buffList.Where(b => b != null).ToDictionary(b => b.id);

        if (GameSession.IsLoggedIn)
        {
            repo.SetupRow(GameSession.PlayerId, buffsById.Values);
        }
    }


    // หยุดเกมแล้วโชว์ buff
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
        {
            Destroy(child.gameObject);
        }

        foreach (BuffData buff in RollBuffs())
        {
            BuffPanel panel = Instantiate(buffPanelPrefab, buffPanelParent);
            panel.Setup(buff, Pick);
        }
    }

    // สุ่ม buff
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
        // นับจำนวนครั้งที่เลือก buff นี้ในรอบนี้ (+1 ต่อการเลือกหนึ่งครั้ง)
        picked[buff.id] = GetPickedCount(buff.id) + 1;

        if (GameSession.IsLoggedIn)
        {
            repo.AddPick(GameSession.PlayerId, buff);
        }

        Debug.Log($"Buff: {buff._Name} ({buff._Description})");
    }

    public IReadOnlyDictionary<int, int> Picked => picked;

    int GetPickedCount(int buffId)
    {
        return picked.TryGetValue(buffId, out int n) ? n : 0;
    }

    public void Load()
    {
        if (!GameSession.IsLoggedIn) return;

        foreach (PlayerBuffRow row in repo.GetActive(GameSession.PlayerId))
        {
            Restore(row.BuffId, row.CurrentRun);
        }
    }

    public void Restore(int buffId, int quantity)
    {
        if (!buffsById.TryGetValue(buffId, out BuffData buff)) return;

        if (buff.stat != BuffStat.Heal)
        {
            for (int i = 0; i < quantity; i++)
            {
                ApplyStat(buff.stat, buff.value);
            }
        }
        picked[buffId] = GetPickedCount(buffId) + quantity;
    }

    public void ApplyStat(BuffStat stat, float value)
    {
        switch (stat)
        {
            case BuffStat.Damage: playerAttack.damage += Mathf.RoundToInt(value); break;
            case BuffStat.FireRate: playerAttack.fireRate *= 1f + value; break;
            case BuffStat.BulletSpeed: playerAttack.bulletSpeed *= 1f + value; break;
            case BuffStat.MultiShot: playerAttack.projectileCount += Mathf.RoundToInt(value); break;
            case BuffStat.Range: playerAttack.range *= 1f + value; break;
            case BuffStat.MoveSpeed: playerControl.speed *= 1f + value; break;
            case BuffStat.MaxHealth: playerHealth.AddMaxHealth(Mathf.RoundToInt(value)); break;
            case BuffStat.Heal: playerHealth.HealFull(); break;
        }
    }
}