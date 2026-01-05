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
        DetectTarget();

        if (currentTarget != null)
        {
            Debug.Log("Target detected: " + currentTarget);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("Pressed F");

            if (currentTarget != null)
                currentTarget.Interact(player);
        }
}   

    void DetectTarget()
    {
        Ray ray = new Ray(transform.position + Vector3.up, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, range, interactLayer))
            currentTarget = hit.collider.GetComponent<IInteractable>();
        else
            currentTarget = null;
    }
}
