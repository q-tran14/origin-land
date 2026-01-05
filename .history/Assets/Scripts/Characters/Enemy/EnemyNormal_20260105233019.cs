using UnityEngine;

public class EnemyNormal : Enemy
{
    public float moveSpeed = 3f;

    protected override void Update()
    {
        base.Update();
        if (isDead || target == null || playerStats.IsDead) return;

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
        animator.SetTrigger("Attack");
    }

    void Idle()
    {
        animator.SetBool("Run", false);
        animator.SetFloat("Speed", 0);
    }
}
