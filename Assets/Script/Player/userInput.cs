using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class userInput : MonoBehaviour
{
    public static userInput instance;
    [SerializeField] private InputActionAsset inputActions;
    
    [Header("InputSystem")]
    public Vector2 move; 
    public Vector2 look;
    public bool attack;
    public bool attackHeld;
    public bool interact;
    public bool sprint;
    private InputAction _move; 
    private InputAction _look;
    private InputAction _attack;
    private InputAction _interact;
    private InputAction _sprint;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        var playerInput = inputActions.FindActionMap("Player");
        setupInputSystem(playerInput);
    }
    public void Update()
    {
        updateInputSystem();
    }
    public void setupInputSystem(InputActionMap Player)
    {
        _move = Player.FindAction("Move");
        _look = Player.FindAction("Look");
        _attack = Player.FindAction("Attack");
        _interact = Player.FindAction("Interact");
        _sprint = Player.FindAction("Sprint");
    }
    public void updateInputSystem()
    {
        move = _move.ReadValue<Vector2>();
        look = _look.ReadValue<Vector2>();
        attack = _attack.WasPressedThisFrame();
        attackHeld = _attack.IsPressed();
        interact = _interact.WasReleasedThisFrame();
        sprint = _sprint.IsPressed();
    }
    public void OnEnable() => inputActions.Enable();
    public void OnDisable() => inputActions.Disable();
}
