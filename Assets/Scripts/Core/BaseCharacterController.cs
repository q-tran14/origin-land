using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public abstract class BaseCharacterController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float rotationSpeed = 10f;
    public Animator animator;

    protected CharacterController controller;
    protected Vector3 moveDirection = Vector3.zero;

    // 6 hướng cho hex (pointy-top)
    protected Vector3[] hexDirections = new Vector3[]
    {
        new Vector3(0, 0, 1),                          // 0° - lên
        new Vector3(Mathf.Sqrt(3)/2, 0, 0.5f),         // 60°
        new Vector3(Mathf.Sqrt(3)/2, 0, -0.5f),        // 120°
        new Vector3(0, 0, -1),                         // 180°
        new Vector3(-Mathf.Sqrt(3)/2, 0, -0.5f),       // 240°
        new Vector3(-Mathf.Sqrt(3)/2, 0, 0.5f)         // 300°
    };

    protected virtual void Start()
    {
        controller = GetComponent<CharacterController>();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    protected virtual void Update()
    {
        HandleMovement();
        UpdateAnimation();
    }

    protected virtual void HandleMovement()
    {
        int moveIndex = -1;

        // 6 phím tương ứng 6 hướng
        if (Input.GetKey(KeyCode.W)) moveIndex = 0;
        else if (Input.GetKey(KeyCode.E)) moveIndex = 1;
        else if (Input.GetKey(KeyCode.D)) moveIndex = 2;
        else if (Input.GetKey(KeyCode.S)) moveIndex = 3;
        else if (Input.GetKey(KeyCode.A)) moveIndex = 4;
        else if (Input.GetKey(KeyCode.Q)) moveIndex = 5;

        if (moveIndex >= 0)
        {
            moveDirection = hexDirections[moveIndex];
            controller.Move(moveDirection * moveSpeed * Time.deltaTime);

            // Xoay theo hướng di chuyển
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            moveDirection = Vector3.zero;
        }
    }

    protected virtual void UpdateAnimation()
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", moveDirection.magnitude);
        }
    }
}
