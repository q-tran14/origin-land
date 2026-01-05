using UnityEngine;

public class ChopableTree : MonoBehaviour, IInteractable
{
    public int hitCount = 3;

    public InteractionType GetInteractionType()
    {
        return InteractionType.Chop;
    }

    public void Interact(Player player)
    {
        player.SetAction(PlayerAction.Chop);

        var movement = player.GetComponent<CharacterMovement>();
        movement.SwitchState(movement.attackState);
    }

    public void OnHit()
    {
        hitCount--;

        if (hitCount <= 0)
            Destroy(gameObject);
    }
}
