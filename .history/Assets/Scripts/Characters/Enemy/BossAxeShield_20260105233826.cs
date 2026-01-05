using UnityEngine;

public class BossAxeShield : Boss
{
    public float blockChance = 0.4f;
    float attackCooldown = 1.5f;
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
        animator.SetBool("Walk", true);
        animator.SetFloat("Speed", 1);

        Vector3 dir = (target.position - transform.position).normalized;
        controller.Move(dir * moveSpeed * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(dir);
    }

    void Attack()
    {
        animator.SetBool("Walk", false);
        animator.SetFloat("Speed", 0);

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            animator.SetTrigger("Attack");
            timer = attackCooldown;
        }
    }

    public override void TakeDamage(int damage)
    {
        if (Random.value < blockChance)
        {
            animator.SetTrigger("Block");
            return;
        }

        base.TakeDamage(damage);
    }
}
