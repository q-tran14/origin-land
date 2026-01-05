using UnityEngine;
using System;

public class PlayerStats : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;

    public bool IsDead { get; private set; }   // ⭐ ENEMY DÙNG

    public event Action OnPlayerDeath;         // ⭐ AI / UI / GAME MANAGER

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaRegenRate = 15f;

    [Header("Survival")]
    public float hunger = 100f;
    public float thirst = 100f;

    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
        InitStats();
    }

    // ================= INIT =================
    public void InitStats()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        hunger = 100f;
        thirst = 100f;
        IsDead = false;
    }

    private void Update()
    {
        if (IsDead) return;

        RegenerateStamina();
        DrainSurvival();
    }

    // ================= STAMINA =================
    void RegenerateStamina()
    {
        if (currentStamina >= maxStamina) return;

        currentStamina += staminaRegenRate * Time.deltaTime;
        currentStamina = Mathf.Min(currentStamina, maxStamina);
    }

    public bool UseStamina(float amount)
    {
        if (currentStamina < amount) return false;

        currentStamina -= amount;
        return true;
    }

    // ================= SURVIVAL =================
    void DrainSurvival()
    {
        hunger -= Time.deltaTime * 0.5f;
        thirst -= Time.deltaTime * 0.8f;

        hunger = Mathf.Clamp(hunger, 0, 100);
        thirst = Mathf.Clamp(thirst, 0, 100);

        if (hunger <= 0 || thirst <= 0)
        {
            TakeDamage(Time.deltaTime * 2f);
        }
    }

    // ================= HEALTH =================
    public void TakeDamage(float damage)
    {
        if (IsDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (IsDead) return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    void Die()
    {
        IsDead = true;

        OnPlayerDeath?.Invoke();   // 🔥 ENEMY + UI + GAME MANAGER

        if (player != null)
            player.Die();
    }
}
