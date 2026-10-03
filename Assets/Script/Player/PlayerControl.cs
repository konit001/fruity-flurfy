using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class PlayerControl : MonoBehaviour
{
    public float speed = 5f;
    public bool isFacingRight;
    public GameObject pivot;
    public GameObject attackPivot;
    private Rigidbody rb;
    private Health health;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        health = GetComponent<Health>();
        health.OnDeath += HandleDeath;
    }

    void OnDestroy()
    {
        health.OnDeath -= HandleDeath;
    }

    void HandleDeath()
    {
        enabled = false;
    }

    void FixedUpdate()
    {
        Move();
        Flip();
        // Look() ไม่ใช้แล้ว: PlayerAttack เล็งอัตโนมัติ
    }
    void Move()
    {
        Vector3 moveDirection = new Vector3(userInput.instance.move.x, 0f, userInput.instance.move.y);
        Vector3 movement = transform.TransformDirection(moveDirection) * speed;
        rb.linearVelocity = movement;
    }

    public void Flip()
    {
        if (rb.linearVelocity.x > 0.1)
        {
            isFacingRight = true;
            transform.localScale = new Vector3(1, 1, 1);
        }

        if (rb.linearVelocity.x < -0.1)
        {
            isFacingRight = false;
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    public void Look()
    {
        Vector2 lookInput = new Vector2(userInput.instance.look.x, userInput.instance.look.y);
        Vector3 direction = Vector3.zero;

        if (lookInput.sqrMagnitude > 2f) // PC - mouse screen position
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(lookInput.x, lookInput.y, 0f));
            Plane groundPlane = new Plane(Vector3.up, attackPivot.transform.position);

            if (groundPlane.Raycast(ray, out float enter))
            {
                Vector3 targetPoint = ray.GetPoint(enter);
                direction = targetPoint - attackPivot.transform.position;
                direction.y = 0f;
            }
        }

        else if (lookInput.sqrMagnitude > 0.01f) // Mobile / Gamepad stick direction
        {
            direction = new Vector3(lookInput.x, 0f, lookInput.y);
        }

        if (direction.sqrMagnitude > 0.0001f)
        {
            attackPivot.transform.rotation = Quaternion.LookRotation(direction);
        }
    }
} 

