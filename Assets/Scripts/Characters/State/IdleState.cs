using UnityEngine;

public class IdleState : BaseMovementState
{
    public override void EnterState(CharacterMovement c)
    {
        c.animator.SetFloat("Speed", 0f);
    }

    public override void UpdateState(CharacterMovement c)
    {
        HandleInput(c);
    }

    protected override void HandleInput(CharacterMovement c)
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        bool run = Input.GetKey(KeyCode.LeftShift) && c.allowRun;

        if (Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f)
            c.SwitchState(run ? c.runState : c.walkState);
    }
}
