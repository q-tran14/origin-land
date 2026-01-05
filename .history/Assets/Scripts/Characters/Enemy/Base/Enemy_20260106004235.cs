using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public abstract class Enemy : MonoBehaviour
{
    [Header("References")]
    public EnemyStats stats;
    public Animator animator;

    protected CharacterController controller;
    protected Transform target;
    protected PlayerStats playerStats;
    protected EnemyAnimatorController animCtrl;

    protected bool isDead;

    // ================= AWAKE =================
    protected virtual void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (!animator)
            animator = GetComponentInChildren<Animator>();

        animCtrl = GetComponentInChildren<EnemyAnimatorController>();

        stats.Init();
        FindPlayer();
    }

    // ================= START =================
    protected virtual void Start()
    {
        // Play spawn animation once
        animCtrl?.PlaySpawn();
    }

    // ================= UPDATE =================
    protected virtual void Update()
    {
        if (isDead || target == null || playerStats == null)
            return;

        // Player chết → enemy dừng
        if (playerStats.IsDead)
        {
            StopEnemy();
            return;
        }
    }

    // ================= PLAYER =================
    protected void FindPlayer()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (!p) return;

        target = p.transform;
        playerStats = p.GetComponent<PlayerStats>();
    }

    // ================= MOVEMENT =================
    protected virtual void StopEnemy()
    {
        animator.SetFloat("Speed", 0f);
    }

    // ================= DAMAGE =================
    public virtual void TakeDamage(int damage)
    {
        if (isDead) return;

        stats.currentHP -= damage;

        animCtrl?.PlayHit();

        if (stats.currentHP <= 0)
            Die();
    }

    // ================= DEATH =================
    protected virtual void Die()
    {
        if (isDead) return;

        isDead = true;

        animCtrl?.PlayDeath();

        controller.enabled = false;

        // báo cho hệ thống encounter
        EnemySpawner.Instance?.NotifyEnemyKilled(this);

        Destroy(gameObject, 5f);
    }
}
