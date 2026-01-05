using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EnemyAnimatorController : MonoBehaviour
{
    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    // ================= LOCOMOTION =================
    public void SetSpeed(float speed)
    {
        anim.SetFloat("Speed", speed);
    }

    // ================= ACTION =================
    public void PlayAttack()
    {
        anim.SetTrigger("Attack");
    }

    public void PlayHit()
    {
        anim.SetTrigger("Hit");
    }

    public void PlaySpawn()
    {
        anim.SetTrigger("Spawn");
    }

    public void PlayDeath()
    {
        anim.SetBool("IsDead", true);
    }

    // ================= UTIL =================
    public bool IsInAttack()
    {
        return anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack");
    }
}
