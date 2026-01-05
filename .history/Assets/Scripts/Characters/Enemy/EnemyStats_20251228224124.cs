using UnityEngine;

[System.Serializable]
public class EnemyStats
{
    public int maxHP = 10;
    public int damage = 1;
    public float attackRange = 1.8f;
    public float detectRange = 8f;

    [HideInInspector] public int currentHP;

    public void Init()
    {
        currentHP = maxHP;
    }
}
