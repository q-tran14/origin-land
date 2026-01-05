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
        Vector3 origin = transform.position + Vector3.up * 1.2f;
        Vector3 dir = transform.forward;

        Debug.DrawRay(origin, dir * attackRange, Color.red, 1f);

        if (Physics.Raycast(origin, dir, out RaycastHit hit, attackRange, hitLayer))
        {
            ChopableTree tree = hit.collider.GetComponentInParent<ChopableTree>();
            if (tree != null)
            {
                tree.OnHit();
            }
        }
    }

}
