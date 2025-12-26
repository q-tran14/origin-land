using UnityEngine;

public class ChopableTree : MonoBehaviour, IInteractable
{
    public int hp = 3;
    public GameObject woodPrefab;

    public void Interact(Player player)
    {
        if (!player.ToolSystem.HasTool(ToolType.Axe)) return;

        player.Movement.SwitchState(player.Movement.attackState);
    }

    // GỌI TỪ ANIMATION EVENT
    public void TakeDamage()
    {
        hp--;

        if (hp <= 0)
        {
            Instantiate(woodPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    public string GetInteractText()
    {
        return "Chop tree";
    }
}
