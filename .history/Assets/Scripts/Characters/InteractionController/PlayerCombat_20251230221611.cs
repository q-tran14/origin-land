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

    Vector3 origin = transform.position + Vector3.up * 1.2f;
    Vector3 direction = transform.forward;

    Debug.DrawRay(origin, direction * 2f, Color.red, 1f);

    if (Physics.Raycast(origin, direction, out RaycastHit hit, 2f))
    {
        Debug.Log("✅ Raycast hit: " + hit.collider.name);

        ChopableTree tree = hit.collider.GetComponent<ChopableTree>();
        if (tree != null)
        {
            tree.OnHit();
        }
    }
    else
    {
        Debug.Log("❌ Raycast did NOT hit anything");
    }
    }

}
