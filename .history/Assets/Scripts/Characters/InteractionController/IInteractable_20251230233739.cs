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
    Pick,     // đập đá
    Combat     // đánh enemy
}
