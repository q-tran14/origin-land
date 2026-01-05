using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public float attackRange = 2f;
    public LayerMask hitLayer;

    public void OnAttackHit()
    {
        Ray ray = new Ray(
            transform.position + Vector3.up * 1.2f,
            transform.forward
        );

        if (!Physics.Raycast(ray, out RaycastHit hit, attackRange, hitLayer))
            return;

        // Ưu tiên resource
        if (hit.collider.TryGetComponent(out ChopableTree tree))
        {
            tree.TakeDamage();
            return;
        }

        if (hit.collider.TryGetComponent(out MineableRock rock))
        {
            rock.TakeDamage();
            return;
        }

        if (hit.collider.TryGetComponent(out Enemy enemy))
        {
            enemy.TakeDamage(1);
        }
    }
}
