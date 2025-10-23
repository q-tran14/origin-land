using UnityEngine;

public class FemaleCharacterController : BaseCharacterController
{
    public float runMultiplier = 1.5f;

    protected override void HandleMovement()
    {
        base.HandleMovement();

        // Nhấn Shift để chạy nhanh hơn
        if (moveDirection.magnitude > 0 && Input.GetKey(KeyCode.LeftShift))
        {
            controller.Move(moveDirection * moveSpeed * runMultiplier * Time.deltaTime);
        }
    }
}
