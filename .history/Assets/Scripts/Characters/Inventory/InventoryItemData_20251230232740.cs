using UnityEngine;

public enum ItemType
{
    Resource,
    Consumable,
    Weapon,
    Quest
}

[CreateAssetMenu(menuName = "Inventory/Item Data")]
public class InventoryItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public ItemType itemType;
    public bool stackable = true;
    public int maxStack = 99;
}
