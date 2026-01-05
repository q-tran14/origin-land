using UnityEngine;

public class BossMage : Boss
{
    float attackCooldown = 2f;
    float timer;

    protected override void BossLogic()
    {
        float dist = Vector3.Distance(transform.position, target.position);

        if (dist <= stats.attackRange)
            Attack();
        else
            Chase();
    }

    void Chase()
    {
        animator.SetFloat("Speed", 1);

        Vector3 dir = (target.position - transform.position).normalized;
        controller.Move(dir * moveSpeed * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(dir);
    }

    void Attack()
    {
        animator.SetFloat("Speed", 0);

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            animator.SetTrigger("Attack");
            timer = attackCooldown;
        }
    }
}
