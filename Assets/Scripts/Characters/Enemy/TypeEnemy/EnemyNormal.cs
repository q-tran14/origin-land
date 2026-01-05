using UnityEngine;

public class EnemyNormal : Enemy
{
    public float moveSpeed = 3f;
    float attackCooldown = 1.2f;
    float timer;

    protected override void Update()
    {
        base.Update();
        if (isDead || playerStats.IsDead) return;

        float dist = Vector3.Distance(transform.position, target.position);

        if (dist <= stats.attackRange)
            Attack();
        else if (dist <= stats.detectRange)
            Chase();
        else
            Idle();
    }

    void Chase()
    {
        animator.SetBool("Run", true);
        animator.SetFloat("Speed", 1);

        Vector3 dir = (target.position - transform.position).normalized;
        controller.Move(dir * moveSpeed * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(dir);
    }

    void Attack()
    {
        animator.SetBool("Run", false);
        animator.SetFloat("Speed", 0);

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            animator.SetTrigger("Attack");
            timer = attackCooldown;
        }
    }

    void Idle()
    {
        animator.SetBool("Run", false);
        animator.SetFloat("Speed", 0);
    }
}
