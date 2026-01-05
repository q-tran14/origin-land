using UnityEngine;

public class MineableRock : MonoBehaviour, IInteractable
{
    [Header("Rock Settings")]
    public int maxHit = 4;
    private int currentHit;

    private void Awake()
    {
        currentHit = maxHit;
    }

    public InteractionType GetInteractionType()
    {
        return InteractionType.Mine;
    }

    public void Interact(Player player)
    {
        // Set hành động hiện tại
        player.SetAction(PlayerAction.Mine);

        // Ép player vào trạng thái attack
        CharacterMovement movement = player.GetComponent<CharacterMovement>();
        movement.SwitchState(movement.attackState);
    }

    public void OnHit()
    {
        currentHit--;
        Debug.Log("⛏ Rock hit, remain: " + currentHit);

        if (currentHit <= 0)
        {
            BreakRock();
        }
    }

    void BreakRock()
    {
        Instantiate(stoneDropPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
