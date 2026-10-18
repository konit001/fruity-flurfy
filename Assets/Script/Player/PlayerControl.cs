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
}

