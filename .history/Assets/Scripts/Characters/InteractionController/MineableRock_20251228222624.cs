using UnityEngine;
public class MineableRock : MonoBehaviour, IInteractable
{
    public int hp = 5;
    public GameObject stonePrefab;

    public void Interact(Player player)
    {
public void Mine(Player player)
{
    hp -= 1;
}

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
