using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public float attackRange = 1.5f;
    public LayerMask hitLayer;

    private Player player;

    void Awake()
    {
        player = GetComponent<Player>();
    }

    // ⚔️ Animation Event gọi
    public void OnAttackHit()
{
    Debug.Log("🔥 OnAttackHit");

    Debug.DrawRay(transform.position + Vector3.up, transform.forward * attackRange, Color.green, 1f);

    Ray ray = new Ray(transform.position + Vector3.up, transform.forward);

    if (Physics.Raycast(ray, out RaycastHit hit, attackRange))
    {
        Debug.Log("🎯 Hit object: " + hit.collider.name);
        Debug.Log("🎯 Hit layer: " + LayerMask.LayerToName(hit.collider.gameObject.layer));
        Debug.Log("🎯 CurrentAction: " + player.CurrentAction);

        if (player.CurrentAction == PlayerAction.Chop)
        {
            if (hit.collider.TryGetComponent<ChopableTree>(out var tree))
            {
                Debug.Log("🌳 ChopableTree FOUND");
                tree.OnHit();
            }
            else
            {
                Debug.Log("❌ Hit object has NO ChopableTree");
            }
        }
    }
    else
    {
        Debug.Log("❌ Raycast did NOT hit anything");
    }

    player.ClearAction();
}

}
