using UnityEngine;

public class DroppedItem : MonoBehaviour, IInteractable
{
    public InventoryItemData itemData;
    public int amount = 1;

    public InteractionType GetInteractionType()
    {
        return InteractionType.Pick;
    }

    public void Interact(Player player)
    {
        PlayerInventory inventory = player.GetComponent<PlayerInventory>();

        if (inventory.AddItem(itemData, amount))
        {
            Debug.Log("Picked: " + itemData.itemName);
            Destroy(gameObject);
        }
    }
}
