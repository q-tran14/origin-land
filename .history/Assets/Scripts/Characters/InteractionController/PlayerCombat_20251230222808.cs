using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public float attackRange = 1.5f;
    public LayerMask hitLayer;

    private ChopableTree lastHitTree;

    private Player player;

    void Awake()
    {
        player = GetComponent<Player>();
    }

    // ⚔️ Animation Event gọi
    public void OnAttackHit()
{
    Debug.Log("🔥 OnAttackHit");

    Vector3 center = transform.position + transform.forward * 1.2f;

    Collider[] hits = Physics.OverlapSphere(
        center,
        1f,
        hitLayer
    );

    foreach (Collider col in hits)
    {
        Debug.Log("✅ Hit: " + col.name);

        ChopableTree tree = col.GetComponentInParent<ChopableTree>();
        if (tree != null)
        {
            tree.OnHit();
            break;
        }
    }
}


}
