using UnityEngine;

[System.Serializable]
public class EnemyStats
{
    [Header("Health")]
    public int maxHP = 20;
    [HideInInspector] public int currentHP;

    [Header("Combat")]
    public int damage = 5;
    public float attackRange = 1.8f;
    public float detectRange = 10f;

    // ================= INIT =================
    public void Init()
    {
        currentHP = maxHP;
    }

    // ================= UTILS =================
    public bool IsDead => currentHP <= 0;
}
