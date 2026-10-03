using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject Bullet;
    public GameObject AttackPoint;

    [Header("Shoot Stats")]
    public int damage = 1;
    public float fireRate = 3f;     // นัดต่อวินาที
    public float bulletSpeed = 20f;

    [Header("Multi Shot")]
    public int projectileCount = 1;   // กระสุนต่อการยิง 1 ครั้ง
    public float spreadAngle = 12f;   // มุมห่างระหว่างนัด (องศา)
    public float maxFan = 180f;       // พัดกว้างสุด — นัดเยอะจะบีบมุมห่างลงไม่ให้วนรอบตัว

    [Header("Auto Aim (ครึ่งวงกลมหันตามสไปรต์)")]
    public float range = 8f;
    [Range(10f, 360f)] public float arcAngle = 180f; // มุมเต็มของกรวย 180 = ครึ่งวงกลม

    private float nextShootTime;
    private Health health;
    private PlayerControl control;

    void Awake()
    {
        health = GetComponent<Health>();
        control = GetComponent<PlayerControl>();
    }

    void Update()
    {
        if (Time.timeScale == 0f) return; // หยุดตอนเลือก buff
        if (health != null && health.IsDead) return;

        Vector3 facing = FacingDirection();
        Transform target = FindTarget(facing);

        // ไม่มีเป้าหมาย: ไม่ยิง แต่ให้ปากกระบอกชี้ทิศครึ่งวงกลม
        Vector3 aim = facing;
        if (target != null)
        {
            aim = target.transform.position - transform.position;
            aim.y = 0f;
        }
        if (control != null && control.attackPivot != null && aim.sqrMagnitude > 0.0001f)
            control.attackPivot.transform.rotation = Quaternion.LookRotation(aim);

        if (target != null && Time.time >= nextShootTime)
        {
            Shoot();
            nextShootTime = Time.time + 1f / fireRate;
        }
    }

    Vector3 FacingDirection()
    {
        bool right = control == null || control.isFacingRight;
        return right ? Vector3.right : Vector3.left;
    }

    // เป้าหมายในครึ่งวงกลม: ศัตรูก่อน ถ้าไม่มีศัตรูค่อยยิงต้นไม้/ทรัพยากร
    Transform FindTarget(Vector3 facing)
    {
        Transform target = Nearest(EnemyControl.All, facing);
        if (target == null) target = Nearest(MaterialItem.All, facing);
        return target;
    }

    // ตัวที่ใกล้ที่สุดภายในครึ่งวงกลม (ระนาบ XZ)
    Transform Nearest(IEnumerable<Component> candidates, Vector3 facing)
    {
        Transform best = null;
        float bestSqr = range * range;
        float halfAngle = arcAngle * 0.5f;
        Vector3 origin = transform.position;

        foreach (Component c in candidates)
        {
            if (c == null) continue;
            Health h = c.GetComponent<Health>();
            if (h != null && h.IsDead) continue;

            Vector3 to = c.transform.position - origin;
            to.y = 0f;
            float sqr = to.sqrMagnitude;
            if (sqr > bestSqr) continue;
            if (sqr > 0.0001f && Vector3.Angle(facing, to) > halfAngle) continue;

            best = c.transform;
            bestSqr = sqr;
        }
        return best;
    }

    void Shoot()
    {
        int n = Mathf.Max(1, projectileCount);
        float step = n > 1 ? Mathf.Min(spreadAngle, maxFan / (n - 1)) : 0f;
        for (int i = 0; i < n; i++)
        {
            float offset = (i - (n - 1) * 0.5f) * step;
            Quaternion rot = AttackPoint.transform.rotation * Quaternion.Euler(0f, offset, 0f);
            GameObject bullet = Instantiate(Bullet, AttackPoint.transform.position, rot);
            bullet.GetComponent<Bullet>().Init(gameObject, damage, bulletSpeed);
        }
    }

    void OnDrawGizmosSelected()
    {
        Vector3 facing = Application.isPlaying ? FacingDirection() : Vector3.right;
        float half = arcAngle * 0.5f;
        const int steps = 32;

        Gizmos.color = Color.red;
        Vector3 origin = transform.position;
        Vector3 prev = origin + Quaternion.AngleAxis(-half, Vector3.up) * facing * range;
        Gizmos.DrawLine(origin, prev);
        for (int i = 1; i <= steps; i++)
        {
            float a = Mathf.Lerp(-half, half, i / (float)steps);
            Vector3 p = origin + Quaternion.AngleAxis(a, Vector3.up) * facing * range;
            Gizmos.DrawLine(prev, p);
            prev = p;
        }
        Gizmos.DrawLine(prev, origin);
    }
}
