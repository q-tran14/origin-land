using UnityEngine;

public class EnemyAnimatorController : MonoBehaviour
{
    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void PlaySpawn()  => anim.SetTrigger("Spawn");
    public void PlayHit()    => anim.SetTrigger("Hit");
    public void PlayDeath()  => anim.SetTrigger("Death");
    public void PlayAttack() => anim.SetTrigger("Attack");

    public void SetSpeed(float speed)
    {
        anim.SetFloat("Speed", speed);
    }
}
