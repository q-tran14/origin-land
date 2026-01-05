using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnemyAI : MonoBehaviour
{
    [Header("Target")]
    public float detectRange = 20f;
    public float attackRange = 2.2f;

    [Header("Attack")]
    public float attackCooldown = 1.5f;
    public float damage = 10f;

    [Header("Rotation")]
    public float rotateSpeed = 10f;

    NavMeshAgent agent;
    Animator anim;

    Transform player;
    PlayerStats playerStats;

    float attackTimer;
    bool isDead;
    bool hasSpawned;

    // ================= INIT =================
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        anim.applyRootMotion = false;
    }

    void Start()
    {
        FindPlayer();

        if (playerStats != null)
            playerStats.OnPlayerDeath += OnPlayerDeath;

        Spawn();
    }

    void OnDestroy()
    {
        if (playerStats != null)
            playerStats.OnPlayerDeath -= OnPlayerDeath;
    }

    // ================= UPDATE =================
    void Update()
    {
        if (isDead || player == null || playerStats == null)
            return;

        if (playerStats.IsDead)
        {
            StopEnemy();
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > detectRange)
        {
            Idle();
            return;
        }

        if (distance > attackRange)
        {
            ChasePlayer();
        }
        else
        {
            AttackPlayer();
        }
    }

    // ================= STATES =================
    void Spawn()
    {
        anim.SetTrigger("Spawn");
        hasSpawned = true;
    }

    void Idle()
    {
        agent.isStopped = true;
        anim.SetFloat("Speed", 0);
    }

    void ChasePlayer()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);

        anim.SetFloat("Speed", agent.velocity.magnitude);
    }

    void AttackPlayer()
    {
        agent.isStopped = true;
        anim.SetFloat("Speed", 0);

        RotateToPlayer();

        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            anim.SetTrigger("Attack");
            attackTimer = attackCooldown;
        }
    }

    void RotateToPlayer()
    {
        Vector3 dir = player.position - transform.position;
        dir.y = 0;

        if (dir == Vector3.zero) return;

        Quaternion rot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            rot,
            rotateSpeed * Time.deltaTime
        );
    }

    // ================= DAMAGE (Animation Event) =================
    public void DealDamage()
    {
        if (playerStats == null || playerStats.IsDead) return;

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= attackRange + 0.3f)
        {
            playerStats.TakeDamage(damage);
        }
    }

    // ================= DEATH =================
    public void TakeDamage(float amount)
    {
        if (isDead) return;

        // nếu enemy có HP thì trừ ở đây
        // ví dụ: currentHealth -= amount;

        // demo: chết ngay
        Die();
    }

    void Die()
    {
        isDead = true;
        agent.isStopped = true;
        anim.SetBool("IsDead", true);
    }

    // ================= PLAYER DEAD =================
    void OnPlayerDeath()
    {
        StopEnemy();
    }

    void StopEnemy()
    {
        agent.isStopped = true;
        anim.SetFloat("Speed", 0);
    }

    // ================= UTIL =================
    void FindPlayer()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p == null) return;

        player = p.transform;
        playerStats = p.GetComponent<PlayerStats>();
    }
}
