using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// แสดง buff 3 อันจาก BuffCatalog ให้เลือก แล้วใส่ค่าให้ผู้เล่น
public class BuffManager : MonoBehaviour
{
    [SerializeField] private GameObject buffPanel;
    [SerializeField] private Button[] buffButtons;

    private List<Buff> choices;
    private Action onPicked;

    private PlayerAttack playerAttack;
    private PlayerControl playerControl;
    private Health playerHealth;

    void Awake()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        playerAttack = player.GetComponent<PlayerAttack>();
        playerControl = player.GetComponent<PlayerControl>();
        playerHealth = player.GetComponent<Health>();

        for (int i = 0; i < buffButtons.Length; i++)
        {
            int index = i;
            buffButtons[i].onClick.AddListener(() => Pick(index));
        }
        buffPanel.SetActive(false);
    }

    // หยุดเกมแล้วโชว์ตัวเลือก — เลือกเสร็จเรียก picked
    public void Show(Action picked)
    {
        onPicked = picked;
        choices = BuffCatalog.GetRandom(buffButtons.Length);

        for (int i = 0; i < buffButtons.Length; i++)
        {
            bool hasBuff = i < choices.Count;
            buffButtons[i].gameObject.SetActive(hasBuff);
            if (hasBuff)
                buffButtons[i].GetComponentInChildren<TMP_Text>().text = choices[i].Name + "\n" + choices[i].Description;
        }

        buffPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    void Pick(int index)
    {
        Apply(choices[index]);

        buffPanel.SetActive(false);
        Time.timeScale = 1f;
        onPicked?.Invoke();
    }

    public void Apply(Buff buff)
    {
        switch (buff.Stat)
        {
            case "Damage":      playerAttack.damage += Mathf.RoundToInt(buff.Value); break;
            case "FireRate":    playerAttack.fireRate *= 1f + buff.Value; break;
            case "BulletSpeed": playerAttack.bulletSpeed *= 1f + buff.Value; break;
            case "MultiShot":   playerAttack.projectileCount += Mathf.RoundToInt(buff.Value); break;
            case "Range":       playerAttack.range *= 1f + buff.Value; break;
            case "MoveSpeed":   playerControl.speed *= 1f + buff.Value; break;
            case "MaxHealth":   playerHealth.AddMaxHealth(Mathf.RoundToInt(buff.Value)); break;
            case "Heal":        playerHealth.HealFull(); break;
            default: Debug.LogWarning($"BuffManager: ไม่รู้จัก Stat '{buff.Stat}' ของ buff {buff.Name}"); break;
        }
        Debug.Log($"Buff: {buff.Name} ({buff.Description})");
    }
}
