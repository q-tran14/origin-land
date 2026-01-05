using UnityEngine;
public class InventoryToggle : MonoBehaviour
{
    public GameObject inventoryUI;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            inventoryUI.SetActive(!inventoryUI.activeSelf);
        }
    }
}
