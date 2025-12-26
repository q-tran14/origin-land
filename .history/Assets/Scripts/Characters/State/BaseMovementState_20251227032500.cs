using UnityEngine;

public abstract class BaseMovementState : ICharacterState
{
    public virtual void EnterState(CharacterMovement c) { }
    public virtual void ExitState(CharacterMovement c) { }

    public virtual void UpdateState(CharacterMovement c)
    {
        HandleInput(c);
    }

    protected virtual void HandleInput(CharacterMovement c)
    {
        
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        bool isRunning =
            Input.GetKey(KeyCode.LeftShift)
            && c.allowRun
            && c.GetComponent<Player>()?.Stats.UseStamina(Time.deltaTime * 5f) == true;

        // 🔹 Nếu không có input → dừng animation & về Idle
        if (Mathf.Abs(h) < 0.1f && Mathf.Abs(v) < 0.1f)
        {
            c.animator.SetFloat("Speed", 0f); // ← thêm dòng này
            return;
        }

        // 🔹 Nếu có input → di chuyển và set Speed
        Move(c, h, v, isRunning ? c.runSpeed : c.walkSpeed);
        c.animator.SetFloat("Speed", isRunning ? 1f : 0.5f);
    }

    protected void Move(CharacterMovement c, float h, float v, float speed)
    {
        // Tính hướng dựa trên camera
        Vector3 camForward = c.mainCamera.transform.forward;
        Vector3 camRight = c.mainCamera.transform.right;

        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        // Vector di chuyển chuẩn hóa
        Vector3 move = (camForward * v + camRight * h);

        if (move.magnitude < 0.01f)
        {
            // Không di chuyển → giữ moveDirection = 0
            c.moveDirection = Vector3.zero;
            return;
        }

        move.Normalize();

        // Di chuyển CharacterController
        c.controller.Move(move * speed * Time.deltaTime);

        // Xoay theo hướng di chuyển mượt
        Quaternion targetRotation = Quaternion.LookRotation(move);
        c.transform.rotation = Quaternion.Slerp(c.transform.rotation, targetRotation, c.rotationSpeed * Time.deltaTime);

        // Cập nhật hướng di chuyển và Speed cho Animator
        c.moveDirection = move;
        c.animator.SetFloat("Speed", speed == c.runSpeed ? 1f : 0.5f);
    }

}
