using UnityEngine;

public interface ICharacterState
{
    void EnterState(CharacterMovement character);
    void UpdateState(CharacterMovement character);
    void ExitState(CharacterMovement character);
}
