using UnityEngine;

public class DroppableItem : MonoBehaviour
{
    public GameObject itemPrefab;

    public void Drop(Vector3 position)
    {
        Instantiate(itemPrefab, position, Quaternion.identity);
    }
}
