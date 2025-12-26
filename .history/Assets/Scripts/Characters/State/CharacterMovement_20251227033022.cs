using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Player))]
public class CharacterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpForce = 10f;
    public float rotationSpeed = 10f;
    public bool allowRun = true;

    [Header("References")]
    public Animator animator;
    public Camera mainCamera;
    [HideInInspector] public CharacterController controller;
    [HideInInspector] public Vector3 moveDirection;

    // FSM
    private ICharacterState currentState;
    [HideInInspector] public IdleState idleState = new();
    [HideInInspector] public WalkState walkState = new();
    [HideInInspector] public RunState runState = new();
    [HideInInspector] public JumpState jumpState = new();
    [HideInInspector] public AttackState attackState = new();
    [HideInInspector] public CheerState cheerState = new();
    [HideInInspector] public DeathState deathState = new();
    public ICharacterState CurrentState => currentState;
    private Player player;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        if (!animator) animator = GetComponentInChildren<Animator>();
        if (!mainCamera) mainCamera = Camera.main;
        player = GetComponent<Player>();
            if (player != null)
        
        
        player = GetComponent<Player>();
        animator.applyRootMotion = false;

        animator.SetLayerWeight(1, 0f);
        SwitchState(idleState);

    }

    private void Update()
    {
        if (currentState is DeathState) return;

        // --- Attack ---
        if (Input.GetMouseButtonDown(0))
        {
            if (player != null && !player.Stats.UseStamina(10f))
                return;

            if (currentState is AttackState attack)
                attack.OnClick(this);
            else
                SwitchState(attackState);
        }
        // --- Cheer ---
        if (Input.GetKeyDown(KeyCode.G) && currentState is not CheerState)
            SwitchState(cheerState);

        // --- Jump ---
        if (Input.GetKeyDown(KeyCode.Space))
{
    if (player != null && player.Stats.UseStamina(15f))
    {
        SwitchState(jumpState);
    }
}

        // --- Death ---
        if (Input.GetKeyDown(KeyCode.K) && currentState is not DeathState)
            Die();

        currentState?.UpdateState(this);
    }

    public void SwitchState(ICharacterState newState)
    {
        if (currentState == newState) return;
        currentState?.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);
    }
        public void Die()
    {
        if (currentState is not DeathState)
            SwitchState(deathState);
    }

}
