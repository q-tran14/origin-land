using UnityEngine;

public class CheerState : ICharacterState
{
    private static readonly int CheerTrigger = Animator.StringToHash("Cheer");
    private float cheerDuration = 2.0f; // thời gian animation cheer
    private float timer;

    public void EnterState(CharacterMovement c)
    {
        // Debug.Log("Enter CheerState");
        c.animator.SetTrigger(CheerTrigger);
        timer = 0f;

        // Dừng chuyển động trong lúc ăn mừng
        c.moveDirection = Vector3.zero;
    }

    public void UpdateState(CharacterMovement c)
    {
        timer += Time.deltaTime;
        if (timer >= cheerDuration)
        {
            // Sau khi kết thúc animation, quay lại idle
            c.SwitchState(c.idleState);
            c.animator.SetFloat("Speed", 0f);
        }
    }

    public void ExitState(CharacterMovement c)
    {
        // Debug.Log("Exit CheerState");
        // Reset trigger để tránh lỗi animation bị kẹt
        c.animator.ResetTrigger(CheerTrigger);
        c.animator.SetLayerWeight(1, 0f);
    }
}
