[System.Serializable]
public class InventorySlot
{
    public InventoryItemData item;
    public int amount;

    public bool IsEmpty => item == null;

    public void Clear()
    {
        item = null;
        amount = 0;
    }
}
