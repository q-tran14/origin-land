using UnityEngine;

public class JumpState : ICharacterState
{
    private float verticalVelocity;
    private float groundedLockTimer = 0f;
    private const float groundedLockDuration = 0.1f;

    public void EnterState(CharacterMovement c)
    {
        // Trigger animation Jump
        
        c.animator.SetTrigger("Jump");

        // Khởi tạo verticalVelocity
        verticalVelocity = c.jumpForce;
        groundedLockTimer = 0f;
    }

    public void UpdateState(CharacterMovement c)
    {
        // --- Input ngang ---
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && c.allowRun;

        Vector3 move = (c.mainCamera.transform.forward * v + c.mainCamera.transform.right * h);
        move.y = 0f;
        if (move.magnitude > 0.1f) move.Normalize();

        // --- Gravity ---
        verticalVelocity += Physics.gravity.y * Time.deltaTime;
        // Debug.Log($"VerticalVelocity: {verticalVelocity}");

        // --- Move ---
        Vector3 velocity = move * (isRunning ? c.runSpeed : c.walkSpeed) + Vector3.up * verticalVelocity;
        c.controller.Move(velocity * Time.deltaTime);

        // --- Xoay ---
        if (move.magnitude > 0.1f)
        {
            c.transform.rotation = Quaternion.Slerp(c.transform.rotation, Quaternion.LookRotation(move), c.rotationSpeed * Time.deltaTime);
        }

        // --- Timer để tránh grounded báo true ngay frame đầu ---
        if (groundedLockTimer > 0f)
        {
            groundedLockTimer -= Time.deltaTime;
            return;
        }

        // --- Khi chạm đất ---
        if (c.controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = 0f;
            if (move.magnitude > 0.1f)
                c.SwitchState(isRunning ? c.runState : c.walkState);
            else
                c.SwitchState(c.idleState);
        }
    }

    public void ExitState(CharacterMovement c)
    {
        c.animator.ResetTrigger("Jump");
    }
}