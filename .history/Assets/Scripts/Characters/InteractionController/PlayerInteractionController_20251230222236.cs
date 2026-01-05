using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerInteractionController : MonoBehaviour
{
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
        if (player.IsBusy())
        {
            ClearTarget();
            return;
        }

        DetectTarget();

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (currentTarget != null)
            {
                Debug.Log("🟢 Interact with " + currentTarget);
                currentTarget.Interact(player);
            }
        }
    }

    void DetectTarget()
    {
        Ray ray = new Ray(transform.position + Vector3.up, transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * range, Color.green);

        if (Physics.Raycast(ray, out RaycastHit hit, range, interactLayer))
        {
            currentTarget = hit.collider.GetComponentInParent<IInteractable>();
            return;
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
