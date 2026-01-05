using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ChopableTree : MonoBehaviour, IInteractable
{
    [Header("Tree Settings")]
    public int maxHitCount = 3;

    private int currentHit;

    public GameObject woodDropPrefab;

    private void Awake()
    {
        currentHit = maxHitCount;
    }

    // =========================
    // INTERACT SYSTEM
    // =========================
    public InteractionType GetInteractionType()
    {
        return InteractionType.Chop;
    }

    public void Interact(Player player)
    {
        if (player == null) return;

        Debug.Log("🌳 Start chopping tree");

        // Đánh dấu hành động hiện tại
        player.SetAction(PlayerAction.Chop);

        // Ép player vào trạng thái Attack
        CharacterMovement movement = player.Movement;
        if (movement != null)
            movement.SwitchState(movement.attackState);
    }

    // =========================
    // COMBAT HIT (ANIMATION EVENT)
    // =========================
    public void OnHit()
    {
        currentHit--;
        Debug.Log($"🌳 Tree hit! Remaining: {currentHit}");

        if (currentHit <= 0)
        {
            BreakTree;
        }
    }

    // =========================
    // TREE DESTROY / FALL
    // =========================
    private void FallDown()
    {
        Debug.Log("🌳 Tree destroyed");

        // 👉 Ở đây bạn có thể:
        // - Play animation đổ cây
        // - Spawn wood item
        // - Disable collider trước khi destroy

        Destroy(gameObject);
    }

    // =========================
    // OPTIONAL (UI)
    // =========================
    public string GetInteractText()
    {
        return "Chop Tree (F)";
    }
    void BreakTree()
    {
        Instantiate(woodDropPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

}
