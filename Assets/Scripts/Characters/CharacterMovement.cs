using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Tốc độ đi bộ cơ bản")]
    public float walkSpeed = 3f;

    [Tooltip("Tốc độ chạy khi giữ Shift")]
    public float runSpeed = 6f;

    [Tooltip("Tốc độ xoay nhân vật")]
    public float rotationSpeed = 10f;

    [Tooltip("Cho phép nhấn Shift để chạy nhanh")]
    public bool allowRun = true;

    [Header("References")]
    public Animator animator;

    private CharacterController controller;
    private Vector3 moveDirection = Vector3.zero;

    // 6 hướng cho hex map
    private readonly Vector3[] hexDirections = new Vector3[]
    {
        new Vector3(0, 0, 1),                          // 0° - lên
        new Vector3(Mathf.Sqrt(3)/2, 0, 0.5f),         // 60°
        new Vector3(Mathf.Sqrt(3)/2, 0, -0.5f),        // 120°
        new Vector3(0, 0, -1),                         // 180°
        new Vector3(-Mathf.Sqrt(3)/2, 0, -0.5f),       // 240°
        new Vector3(-Mathf.Sqrt(3)/2, 0, 0.5f)         // 300°
    };

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        HandleMovement();
        UpdateAnimation();
    }

    private void HandleMovement()
    {
        int moveIndex = -1;

        // 6 phím tương ứng 6 hướng
        if (Input.GetKey(KeyCode.W)) moveIndex = 0;
        else if (Input.GetKey(KeyCode.E)) moveIndex = 1;
        else if (Input.GetKey(KeyCode.D)) moveIndex = 2;
        else if (Input.GetKey(KeyCode.S)) moveIndex = 3;
        else if (Input.GetKey(KeyCode.A)) moveIndex = 4;
        else if (Input.GetKey(KeyCode.Q)) moveIndex = 5;

        bool isRunning = allowRun && Input.GetKey(KeyCode.LeftShift);

        if (moveIndex >= 0)
        {
            moveDirection = hexDirections[moveIndex];
            float currentSpeed = isRunning ? runSpeed : walkSpeed;

            controller.Move(moveDirection * currentSpeed * Time.deltaTime);

           
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            moveDirection = Vector3.zero;
        }
    }

    private void UpdateAnimation()
    {
        if (animator == null) return;

        float speedValue = 0f;

        if (moveDirection.magnitude > 0.1f)
        {
            bool isRunning = allowRun && Input.GetKey(KeyCode.LeftShift);
            // 0 = idle, 0.5 = walk, 1 = run
            speedValue = isRunning ? 1f : 0.5f;
        }

        animator.SetFloat("Speed", speedValue, 0.1f, Time.deltaTime);
    }
}
