using UnityEngine;

// กระสุนชนด้วย trigger collider (OnTriggerEnter)
// Rigidbody (kinematic, ไม่ใช้แรงโน้มถ่วง) และ Collider ต้องมีบน prefab เอง
[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private int damage = 1;

    private GameObject owner;
    private Rigidbody rb;
    private bool hasHit;

    public void Init(GameObject owner, int damage, float speed)
    {
        this.owner = owner;
        this.damage = damage;
        this.speed = speed;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + transform.forward * speed * Time.fixedDeltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;

        // ศัตรู
        var enemy = other.GetComponentInParent<EnemyControl>();
        if (enemy != null)
        {
            enemy.GetComponent<Health>().TakeDamage(damage);
            Consume();
            return;
        }

        // ทรัพยากร (ต้นไม้/หิน)
        var node = other.GetComponentInParent<MaterialItem>();
        if (node != null)
        {
            node.TakeDamage(damage);
            Consume();
        }
    }

    void Consume()
    {
        hasHit = true;
        Destroy(gameObject);
    }
}
