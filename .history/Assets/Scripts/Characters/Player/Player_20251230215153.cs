using UnityEngine;

[RequireComponent(typeof(CharacterMovement))]
public class Player : MonoBehaviour
{
    [Header("Core")]
    public CharacterMovement Movement { get; private set; }

    [Header("Systems")]
    public PlayerStats Stats { get; private set; }
    public PlayerAction CurrentAction { get; private set; }
    public void SetAction(PlayerAction action)
    {
        CurrentAction = action;
    }

    public void ClearAction()
    {
        CurrentAction = PlayerAction.None;
    }

    public bool IsBusy()
    {
        return CurrentAction != PlayerAction.None;
    }
    public bool isBusy;

    public void SetBusy()
    {
        isBusy = true;
    }

    public void ClearAction()
    {
        isBusy = false;
    }

    private void Awake()
    {
        Movement = GetComponent<CharacterMovement>();
        Stats = GetComponent<PlayerStats>();
    //     Interaction = GetComponent<PlayerInteraction>();
    //     Inventory = GetComponent<InventorySystem>();
        // ToolSystem = GetComponent<ToolSystem>();
    }
    /// <summary>
    /// Gọi khi player chết
    /// </summary>
    public void Die()
    {
        Movement.Die();
    }
}
public enum PlayerAction
{
    None,
    Chop,
    Mine,
    PickUp,
    Combat
}

