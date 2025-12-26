public class PlayerStats : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100;
    public float CurrentHealth { get; private set; }

    [Header("Stamina")]
    public float maxStamina = 100;
    public float currentStamina;

    [Header("Survival")]
    public float hunger = 100;
    public float thirst = 100;

    private void Awake()
    {
        CurrentHealth = maxHealth;
        currentStamina = maxStamina;
    }

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;

        if (CurrentHealth <= 0)
        {
            GetComponent<Player>().StateMachine.ChangeState(
                GetComponent<Player>().StateMachine.DeathState
            );
        }
    }

    public bool UseStamina(float amount)
    {
        if (currentStamina < amount) return false;

        currentStamina -= amount;
        return true;
    }
}
