public interface IInteractable
{
    void Interact(Player player);
    string GetInteractText();
}

public enum InteractionType
{
    None,
    Chop,      // chặt cây
    Mine,      // đập đá
    PickUp,    // nhặt item
    Combat     // đánh enemy
}
