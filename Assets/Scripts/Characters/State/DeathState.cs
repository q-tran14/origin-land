using UnityEngine;
using System.Collections;

public class DeathState : ICharacterState
{
    private bool isDead = false;

    public void EnterState(CharacterMovement c)
    {
        if (isDead) return;

        isDead = true;
        c.animator.ResetTrigger("Attack");
        c.animator.ResetTrigger("Cheer");
        c.animator.ResetTrigger("Jump");

        // Tắt toàn bộ layer khác, chỉ để base
        c.animator.SetLayerWeight(0, 1f);
        c.animator.SetLayerWeight(1, 0f);
        c.animator.SetTrigger("Death");

        // Dừng mọi di chuyển
        c.controller.enabled = false;

        // Debug.Log("Enter DeathState");
    }

    public void UpdateState(CharacterMovement c) { }

    public void ExitState(CharacterMovement c)
    {
        // Debug.Log("Exit DeathState");
    }
}
