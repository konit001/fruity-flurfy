using System;
using System.Collections.Generic;
using NaughtyAttributes;
using Unity.Mathematics;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private float invincibilityDuration = 1f;
    [SerializeField] private RectTransform healthLayout;
    [SerializeField] private GameObject healthPrefab;
    private List<GameObject> healthPoint = new List<GameObject>();
    [ReadOnly][SerializeField] private int currentHealth;

    private float invincibleUntil;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead { get; private set; }

    public event Action<Hit> OnDamaged;
    public event Action OnDeath;

    void Awake()
    {
        currentHealth = maxHealth;
        RefreshUI();
    }

    // ตั้งค่า HP สูงสุดตรงๆ แล้วเติมเต็ม (ใช้กับศัตรูที่อ่านค่าจาก DB)
    public void SetMaxHealth(int value)
    {
        maxHealth = value;
        currentHealth = value;
        RefreshUI();
    }

    // ใช้กับ buff
    public void AddMaxHealth(int amount)
    {
        maxHealth += amount;
        currentHealth += amount;
        RefreshUI();
    }

    public void HealFull()
    {
        if (IsDead) return;
        currentHealth = maxHealth;
        RefreshUI();
    }
    private void RefreshUI()
    {
        if (healthLayout == null || healthPrefab == null) return;

        while (healthPoint.Count < maxHealth)
        {
            healthPoint.Add(Instantiate(healthPrefab, healthLayout));
        }

        for (int i = 0; i < healthPoint.Count; i++)
        {
            healthPoint[i].SetActive(i < maxHealth);
            healthPoint[i].transform.GetChild(1).gameObject.SetActive(i < currentHealth);
        }
    }

    public void TakeHit(Hit hit) // Player
    {
        if (IsDead || Time.time < invincibleUntil)
        {
            return;
        }

        currentHealth = Mathf.Max(0, currentHealth - hit.damage);
        invincibleUntil = Time.time + invincibilityDuration;
        RefreshUI();

        OnDamaged?.Invoke(hit);

        if (currentHealth <= 0)
        {
            IsDead = true;
            OnDeath?.Invoke();
        }
    }

    public void TakeDamage(float damage) // Enemy
    {
        if (IsDead || Time.time < invincibleUntil) return;
        currentHealth = currentHealth - (int)damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0)
        {
            IsDead = true;
            OnDeath?.Invoke();
        }
    }
}
