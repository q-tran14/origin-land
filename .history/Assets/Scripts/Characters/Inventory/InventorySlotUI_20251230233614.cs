using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    public Image icon;
    public Text amountText;

    public void UpdateSlot(InventorySlot slot)
    {
        if (slot.IsEmpty)
        {
            icon.enabled = false;
            amountText.text = "";
        }
        else
        {
            icon.enabled = true;
            icon.sprite = slot.item.icon;
            amountText.text = slot.amount.ToString();
        }
    }
}
