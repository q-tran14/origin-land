public class PlayerInteractionController : MonoBehaviour
{
    public float range = 2f;
    public LayerMask interactLayer;

    private Player player;
    private IInteractable target;

    void Awake()
    {
        player = GetComponent<Player>();
    }

    void Update()
    {
        if (player.IsBusy()) return;

        Detect();

        if (Input.GetKeyDown(KeyCode.F) && target != null)
            target.Interact(player);
    }

    void Detect()
    {
        Ray ray = new Ray(transform.position + Vector3.up, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, range, interactLayer))
            target = hit.collider.GetComponent<IInteractable>();
        else
            target = null;
    }
}

