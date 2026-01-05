using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public int slotCount = 20;
    public InventorySlot[] slots;

    private void Awake()
    {
        slots = new InventorySlot[slotCount];
        for (int i = 0; i < slots.Length; i++)
            slots[i] = new InventorySlot();
    }

    // =========================
    // ADD ITEM
    // =========================
    public bool AddItem(InventoryItemData item, int amount = 1)
    {
        // Stack trước
        if (item.stackable)
        {
            foreach (var slot in slots)
            {
                if (!slot.IsEmpty && slot.item == item && slot.amount < item.maxStack)
                {
                    slot.amount += amount;
                    return true;
                }
            }
        }

        // Slot trống
        foreach (var slot in slots)
        {
            if (slot.IsEmpty)
            {
                slot.item = item;
                slot.amount = amount;
                return true;
            }
        }

        Debug.Log("❌ Inventory full");
        return false;
    }

    // =========================
    // REMOVE ITEM
    // =========================
    public void RemoveItem(InventoryItemData item, int amount = 1)
    {
        foreach (var slot in slots)
        {
            if (!slot.IsEmpty && slot.item == item)
            {
                slot.amount -= amount;
                if (slot.amount <= 0)
                    slot.Clear();
                return;
            }
        }
    }
}
