using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public abstract class Enemy : MonoBehaviour
{
    public EnemyStats stats;
    public Animator animator;

    protected CharacterController controller;
    protected Transform target;
    protected PlayerStats playerStats;
    protected EnemyAnimatorController animCtrl;

    protected bool isDead;

    protected virtual void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (!animator) animator = GetComponentInChildren<Animator>();

        stats.Init();
        FindPlayer();
    }
    protected override void Awake()
{
    base.Awake();
    animCtrl = GetComponent<EnemyAnimatorController>();
    animCtrl.PlaySpawn();
}
    protected virtual void Update()
    {
        if (isDead || target == null || playerStats == null) return;

        if (playerStats.IsDead)
        {
            StopEnemy();
            return;
        }
    }

    void FindPlayer()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (!p) return;

        target = p.transform;
        playerStats = p.GetComponent<PlayerStats>();
    }

    protected virtual void StopEnemy()
    {
        animator.SetFloat("Speed", 0);
    }

    public virtual void TakeDamage(int damage)
    {
        if (isDead) return;

        stats.currentHP -= damage;
        animator.SetTrigger("Hit");

        if (stats.currentHP <= 0)
            Die();
    }

    protected virtual void Die()
    {
        isDead = true;
        animator.SetTrigger("Death");
        controller.enabled = false;

        EnemySpawner.Instance.NotifyEnemyKilled(this);
        Destroy(gameObject, 5f);
    }
}
