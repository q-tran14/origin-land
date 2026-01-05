public enum InteractionType
{
    None,
    Chop,      // chặt cây
    Mine,      // đập đá
    PickUp,    // nhặt item
    Combat     // đánh enemy
}

public interface IInteractable
{
    InteractionType GetInteractionType();
    void Interact(Player player);
}
