using UnityEngine;

public class RunState : BaseMovementState
{
    public override void EnterState(CharacterMovement c)
    {
        c.animator.SetFloat("Speed", 1f);
    }

    protected override void HandleInput(CharacterMovement c)
    {
        if (!Input.GetKey(KeyCode.LeftShift) || !c.allowRun)
        {
            c.SwitchState(c.walkState);
            return;
        }

        base.HandleInput(c);
    }
}
