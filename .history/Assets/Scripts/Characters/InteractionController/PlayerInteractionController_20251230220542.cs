using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerInteractionController : MonoBehaviour
{
    [Header("Interact Settings")]
    public float range = 2f;
    public LayerMask interactLayer;

    private Player player;
    private IInteractable currentTarget;

    void Awake()
    {
        player = GetComponent<Player>();
    }

    void Update()
    {
        // ❌ Không cho interact khi đang bận (attack, jump, chết...)
        if (player.IsBusy())
        {
            ClearTarget();
            return;
        }

        DetectTarget();

        // 🔑 Nhấn F để tương tác
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (currentTarget != null)
            {
                Debug.Log("Interact with: " + currentTarget);
                currentTarget.Interact(player);
            }
            else
            {
                Debug.Log("No interactable target");
            }
        }
    }

    // =========================
    // DETECT INTERACTABLE
    // =========================
    void DetectTarget()
    {
        Ray ray = new Ray(transform.position + Vector3.up, transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * range, Color.red);

        if (Physics.Raycast(ray, out RaycastHit hit, range, interactLayer))
        {
            // ⚠️ RẤT QUAN TRỌNG:
            // Lấy IInteractable ở CHA, tránh lỗi collider nằm ở child
            IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();

            if (interactable != null)
            {
                if (currentTarget != interactable)
                {
                    currentTarget = interactable;
                    Debug.Log("Target detected: " + hit.collider.name);
                }
                return;
            }
        }

        ClearTarget();
    }

    void ClearTarget()
    {
        currentTarget = null;
    }

    // =========================
    // OPTIONAL (CHO UI SAU NÀY)
    // =========================
    public IInteractable GetCurrentTarget()
    {
        return currentTarget;
    }
}
