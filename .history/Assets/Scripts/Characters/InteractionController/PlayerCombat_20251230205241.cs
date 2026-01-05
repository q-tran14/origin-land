using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public float attackRange = 1.5f;
    public LayerMask interactLayer;

    public void OnAttackHit()
    {
        Ray ray = new Ray(transform.position + Vector3.up, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, attackRange, interactLayer))
        {
            if (hit.collider.TryGetComponent(out ChopableTree tree))
                tree.OnHit();
        }

        GetComponent<Player>().ClearAction();
    }
}
