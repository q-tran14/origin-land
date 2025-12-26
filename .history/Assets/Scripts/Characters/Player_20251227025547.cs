using UnityEngine;

[RequireComponent(typeof(CharacterMovement))]
public class Player : MonoBehaviour
{
    public CharacterMovement Movement { get; private set; }
    public PlayerInteraction Interaction { get; private set; }
    public PlayerStats Stats { get; private set; }
    public InventorySystem Inventory { get; private set; }
    public ToolSystem ToolSystem { get; private set; }

    private void Awake()
    {
        Movement = GetComponent<CharacterMovement>();
        Interaction = GetComponent<PlayerInteraction>();
        Stats = GetComponent<PlayerStats>();
        Inventory = GetComponent<InventorySystem>();
        ToolSystem = GetComponent<ToolSystem>();
    }

    public bool IsBusy()
    {
        return Movement.CurrentState is AttackState
            || Movement.CurrentState is CheerState
            || Movement.CurrentState is DeathState;
    }
}
