using UnityEngine;

[RequireComponent(typeof(CharacterMovement))]
public class Player : MonoBehaviour
{
    [Header("Core")]
    public CharacterMovement Movement { get; private set; }

    [Header("Systems")]
    public PlayerStats Stats { get; private set; }
    // public PlayerInteraction Interaction { get; private set; }
    // public InventorySystem Inventory { get; private set; }
    public ToolSystem ToolSystem { get; private set; }

    private void Awake()
    {
        Movement = GetComponent<CharacterMovement>();
        Stats = GetComponent<PlayerStats>();
    //     Interaction = GetComponent<PlayerInteraction>();
    //     Inventory = GetComponent<InventorySystem>();
        ToolSystem = GetComponent<ToolSystem>();
    }

    /// <summary>
    /// Player đang bận animation quan trọng (không cho tương tác)
    /// </summary>
    public bool IsBusy()
    {
        var state = Movement.CurrentState;
        return state is AttackState
            || state is CheerState
            || state is DeathState;
    }

    /// <summary>
    /// Gọi khi player chết
    /// </summary>
    public void Die()
    {
        Movement.Die();
    }
}
