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
    private Dictionary<string, BuffRow> definitions;     // key = BuffName
    private Dictionary<int, BuffRow> definitionsById;    // key = BuffRow.BuffId

    void Awake()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        playerAttack = player.GetComponent<PlayerAttack>();
        playerControl = player.GetComponent<PlayerControl>();
        playerHealth = player.GetComponent<Health>();

        BuffUi.SetActive(false);
        LoadDefinitions();
    }

    // stat/value ของ buff
    void LoadDefinitions()
    {
        repo = new PlayerBuffRepository(DbProvider.Connection);
        repo.SeedDefinitions(buffList);
        definitions = repo.GetDefinitions();
        definitionsById = definitions.Values.ToDictionary(r => r.BuffId);

        if (GameSession.IsLoggedIn)
        {
            repo.SetupRow(GameSession.PlayerId, definitions.Values);
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
        BuffRow row = definitions[buff._Name];
        ApplyStat(row.Stat, row.Value);
        picked[row.BuffId] = GetPickedCount(row.BuffId) + 1;

        if (GameSession.IsLoggedIn)
        {
            repo.AddPick(GameSession.PlayerId, row);
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
        if (!definitionsById.TryGetValue(buffId, out BuffRow row)) return;

        if (row.Stat != BuffStat.Heal)
        {
            for (int i = 0; i < quantity; i++)
            {
                ApplyStat(row.Stat, row.Value);
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