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
        }

}
