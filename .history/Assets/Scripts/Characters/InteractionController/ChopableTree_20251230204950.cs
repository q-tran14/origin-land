using UnityEngine;

public class ChopableTree : MonoBehaviour, IInteractable
{
    public int maxHp = 5;
    private int currentHp;

    public GameObject woodPrefab;

    private void Awake()
    {
        currentHp = maxHp;
    }

    public void Interact(Player player)
    {
        if (player == null) return;

        player.Movement.SwitchState(player.Movement.attackState);
    }

    public void TakeDamage(int damage = 1)
    {
        currentHp -= damage;

        if (currentHp <= 0)
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
