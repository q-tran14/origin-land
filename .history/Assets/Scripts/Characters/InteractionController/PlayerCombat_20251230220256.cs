using UnityEngine;

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

        Ray ray = new Ray(transform.position + Vector3.up, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, attackRange, hitLayer))
        {
            // 🌳 Chặt cây
            if (hit.collider.TryGetComponent(out ChopableTree tree))
            {
                tree.OnHit();
                return;
            }

            // 🪨 Đập đá
            if (hit.collider.TryGetComponent(out MineableRock rock))
            {
                rock.OnHit();
                return;
            }

            // 👹 Đánh enemy
            if (hit.collider.TryGetComponent(out Enemy enemy))
            {
                enemy.TakeDamage(10);
                return;
            }
        }

        player.ClearAction();
    }
}
