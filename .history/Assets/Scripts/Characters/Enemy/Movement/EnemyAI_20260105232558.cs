using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public float detectRange = 15f;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;

    Transform player;
    NavMeshAgent agent;
    EnemyAnimator enemyAnim;
    PlayerStats playerStats;

    float attackTimer;
    bool isDead;

    void Start()
    {
        playerStats = GameObject.FindGameObjectWithTag("Player")
                            .GetComponent<PlayerStats>();
        agent = GetComponent<NavMeshAgent>();
        enemyAnim = GetComponent<EnemyAnimator>();

        enemyAnim.Spawn();
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > attackRange)
        {
            ChasePlayer();
        }
        else
        {
            AttackPlayer();
        }
    }

    void ChasePlayer()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);

        enemyAnim.SetSpeed(agent.velocity.magnitude);
    }

    void AttackPlayer()
    {
        agent.isStopped = true;
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

        enemyAnim.SetSpeed(0);

        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
        {
            enemyAnim.Attack();
            attackTimer = attackCooldown;
        }
    }

    public void Die()
    {
        isDead = true;
        agent.isStopped = true;
        enemyAnim.Die();
    }
}
