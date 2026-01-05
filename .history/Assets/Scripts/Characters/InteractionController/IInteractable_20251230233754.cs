public interface IInteractable
{
    InteractionType GetInteractionType();
    void Interact(Player player);
}

public enum InteractionType
{
    None,
    Chop,      // chặt cây
    Mine, 
    Pick,
    Combat     // đánh enemy
}
