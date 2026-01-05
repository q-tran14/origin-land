using UnityEngine;

public class BossAxeShield : Boss
{
    public bool isBlocking;
    public float blockChance = 0.4f;

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
        Vector3 dir = (target.position - transform.position).normalized;
        controller.Move(dir * moveSpeed * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(dir);
    }

    void Attack()
    {
        animator.SetBool("Walk", false);
        animator.SetTrigger("Attack");
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
