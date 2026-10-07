using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Health))]
public class EnemyControl : MonoBehaviour
{
    public static readonly List<EnemyControl> All = new List<EnemyControl>();
    public List<ItemDrop> dropItems = new List<ItemDrop>();

    [Header("Drop Scatter")]
    [SerializeField] private float dropRadiusMin = 0.5f;
    [SerializeField] private float dropRadiusMax = 1.5f;

    [Header("Chase")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private bool chasePlayerAlways = true;

    [Header("Contact Damage")]
    [SerializeField] private int contactDamage = 1;
    [SerializeField] private float contactRange = 0.8f;


    [Header("Database (ตาราง Enemys)")]
    [Tooltip("ชื่อแถวในตาราง Enemys — เว้นว่างไว้จะใช้ชื่อ prefab")]
    [SerializeField] private string enemyName;

    private Rigidbody rb;
    private Health health;
    private Transform target;
    void OnEnable() => All.Add(this);
    void OnDisable() => All.Remove(this);
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        health = GetComponent<Health>();

        health.OnDeath += HandleDeath;
    }

    void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) target = player.transform;
    }
    void OnDestroy()
    {
        health.OnDeath -= HandleDeath;
    }

    void HandleDeath()
    {
        if (GameSession.IsLoggedIn)
            new PlayerRepository(DbProvider.Connection).AddKills(GameSession.PlayerId, 1);
        AchievementTracker.EvaluateKills();
        ItemDrop.SpawnAll(dropItems, transform.position, dropRadiusMin, dropRadiusMax);

        Destroy(gameObject);
    }

    // เดินตรงเข้าหาเป้า
    void FixedUpdate()
    {
        if (target == null)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        Vector3 dir = target.position - transform.position;
        dir.y = 0f;
        rb.linearVelocity = dir.normalized * speed;
        TryHitTarget();
    }

    // ชนผู้เล่นแล้วทำดาเมจ
    void TryHitTarget()
    {
        Vector3 diff = target.position - transform.position;
        diff.y = 0f;
        if (diff.sqrMagnitude > contactRange * contactRange) return;

        var targetHealth = target.GetComponent<Health>();
        if (targetHealth == null) return;

        targetHealth.TakeHit(new Hit
        {
            damage = contactDamage,
            point = transform.position,
            direction = diff.normalized,
            source = gameObject
        });
    }

}
