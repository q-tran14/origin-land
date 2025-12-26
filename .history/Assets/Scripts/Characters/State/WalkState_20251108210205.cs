public class WalkState : BaseMovementState
{
    public override void EnterState(CharacterMovement c)
    {
        c.animator.SetFloat("Speed", 0.5f);
    }
}
