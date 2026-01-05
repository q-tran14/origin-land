using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void SetSpeed(float speed)
    {
        anim.SetFloat("Speed", speed);
    }

    public void Attack()
    {
        anim.SetTrigger("Attack");
    }

    public void Hit()
    {
        anim.SetTrigger("Hit");
    }

    public void Die()
    {
        anim.SetBool("IsDead", true);
    }

    public void Spawn()
    {
        anim.SetTrigger("Spawn");
    }
}
