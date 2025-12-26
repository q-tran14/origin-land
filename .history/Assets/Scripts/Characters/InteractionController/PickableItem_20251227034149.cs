using UnityEngine;

public class PickableItem : MonoBehaviour, IInteractable
{
    public string itemId;

    public void Interact(Player player)
    {
        // player.Inventory.AddItem(itemId);
        Destroy(gameObject);
    }

    public string GetInteractText()
    {
        return "Nhặt vật phẩm";
    }
}
