using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public PlayerInventory inventory;
    public InventorySlotUI[] slotsUI;

    void Update()
    {
        for (int i = 0; i < slotsUI.Length; i++)
        {
            slotsUI[i].UpdateSlot(inventory.slots[i]);
        }
    }
}
