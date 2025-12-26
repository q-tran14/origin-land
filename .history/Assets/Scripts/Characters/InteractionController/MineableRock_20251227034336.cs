public class MineableRock : MonoBehaviour, IInteractable
{
    public int hp = 5;
    public GameObject stonePrefab;

    public void Interact(Player player)
    {
        if (!player.ToolSystem.HasTool(ToolType.Pickaxe)) return;

        player.Movement.SwitchState(player.Movement.attackState);
    }

    public void TakeDamage()
    {
        hp--;

        if (hp <= 0)
        {
            Instantiate(stonePrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    public string GetInteractText() => "Mine rock";
}
