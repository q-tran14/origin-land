using UnityEngine;

public class EnemyNormal : Enemy
{
    public Transform target;
    public float moveSpeed = 3f;

    private void Update()
    {
        if (isDead || target == null) return;

        float dist = Vector3.Distance(transform.position, target.position);

        if (dist <= stats.attackRange)
        {
            Attack();
        }
        else if (dist <= stats.detectRange)
        {
            Chase();
        }
        else
        {
            Idle();
        }
    }

    void Chase()
    {
        animator.SetBool("Run", true);
        Vector3 dir = (target.position - transform.position).normalized;
        controller.Move(dir * moveSpeed * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(dir);
    }

    void Attack()
    {
        animator.SetBool("Run", false);
        animator.SetTrigger("Attack");
    }

    void Idle()
    {
        animator.SetBool("Run", false);
    }
}
