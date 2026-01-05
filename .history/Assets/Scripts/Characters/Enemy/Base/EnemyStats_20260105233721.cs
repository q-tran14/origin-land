using UnityEngine;

[System.Serializable]
public class EnemyStats
{
    public int maxHP = 20;
    public int damage = 5;
    public float attackRange = 1.8f;
    public float detectRange = 10f;

    [HideInInspector] public int currentHP;

    public void Init()
    {
        currentHP = maxHP;
    }
}
