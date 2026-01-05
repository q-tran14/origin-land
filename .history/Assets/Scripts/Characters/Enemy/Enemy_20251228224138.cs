using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public abstract class Enemy : MonoBehaviour
{
    public EnemyStats stats;
    public Animator animator;

    protected CharacterController controller;
    protected bool isDead;

    protected virtual void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (!animator) animator = GetComponentInChildren<Animator>();

        stats.Init();
    }

    public virtual void TakeDamage(int damage)
    {
        if (isDead) return;

        stats.currentHP -= damage;
        animator.SetTrigger("Hit");

        if (stats.currentHP <= 0)
            Die();
    }

    protected virtual void Die()
    {
        isDead = true;
        animator.SetTrigger("Death");
        controller.enabled = false;
        Destroy(gameObject, 5f);
    }
}
