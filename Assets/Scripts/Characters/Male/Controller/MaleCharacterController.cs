using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MaleCharacterController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float rotationSpeed = 10f;
    public Animator animator;

    private CharacterController controller;
    private Vector3 moveDirection = Vector3.zero;

    // 6 hướng cho hex map (pointy-top)
    private Vector3[] hexDirections = new Vector3[]
    {
        new Vector3(0, 0, 1),                          // 0° - lên
        new Vector3(Mathf.Sqrt(3)/2, 0, 0.5f),         // 60°
        new Vector3(Mathf.Sqrt(3)/2, 0, -0.5f),        // 120°
        new Vector3(0, 0, -1),                         // 180°
        new Vector3(-Mathf.Sqrt(3)/2, 0, -0.5f),       // 240°
        new Vector3(-Mathf.Sqrt(3)/2, 0, 0.5f)         // 300°
    };

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        int moveIndex = -1;

        // Bấm 6 phím tương ứng 6 hướng
        if (Input.GetKey(KeyCode.W)) moveIndex = 0;
        else if (Input.GetKey(KeyCode.E)) moveIndex = 1;
        else if (Input.GetKey(KeyCode.D)) moveIndex = 2;
        else if (Input.GetKey(KeyCode.S)) moveIndex = 3;
        else if (Input.GetKey(KeyCode.A)) moveIndex = 4;
        else if (Input.GetKey(KeyCode.Q)) moveIndex = 5;

        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        float speed = 0f;

        if (moveIndex >= 0)
        {
            moveDirection = hexDirections[moveIndex];
            float moveSpeed = isRunning ? runSpeed : walkSpeed;
            controller.Move(moveDirection * moveSpeed * Time.deltaTime);

            // Xoay hướng di chuyển
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Tính speed cho animator (0–1)
            speed = isRunning ? 1f : 0.5f;
        }
        else
        {
            moveDirection = Vector3.zero;
            speed = 0f;
        }

        if (animator != null)
            animator.SetFloat("Speed", speed, 0.1f, Time.deltaTime);
    }
}
