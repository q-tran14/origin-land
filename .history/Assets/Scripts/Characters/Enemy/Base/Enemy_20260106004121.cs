using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public abstract class Enemy : MonoBehaviour
{
    public EnemyStats stats;
    public Animator animator;

    protected CharacterController controller;
    protected Transform target;
    protected PlayerStats playerStats;

    protected float attackCooldown = 1.5f;
    protected float lastAttackTime;
    protected bool isDead;

    protected virtual void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (!animator) animator = GetComponentInChildren<Animator>();

        stats.Init();
        FindPlayer();
    }

    protected virtual void Start()
    {
        animator.SetTrigger("Spawn");
    }

    protected virtual void Update()
    {
        if (isDead || target == null || playerStats == null) return;

        if (playerStats.currentHealth <= 0)
        {
            StopEnemy();
            return;
        }

        float dist = Vector3.Distance(transform.position, target.position);

        if (dist > stats.attackRange)
            MoveToTarget();
        else
            Attack();
    }

    protected void MoveToTarget()
    {
        Vector3 dir = (target.position - transform.position).normalized;
        controller.Move(dir * 3f * Time.deltaTime);

        animator.SetFloat("Speed", 1);
        transform.LookAt(target);
    }

    protected void Attack()
    {
        animator.SetFloat("Speed", 0);

        if (Time.time < lastAttackTime + attackCooldown) return;

        lastAttackTime = Time.time;
        animator.SetTrigger("Attack");

        playerStats.TakeDamage(stats.damage);
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
        Destroy(gameObject, 4f);
    }

    void FindPlayer()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (!p) return;

        target = p.transform;
        playerStats = p.GetComponent<PlayerStats>();
    }
}
