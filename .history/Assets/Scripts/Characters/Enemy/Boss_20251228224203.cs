using UnityEngine;

public abstract class Boss : Enemy
{
    public Transform target;
    public float moveSpeed = 2.5f;

    protected virtual void Update()
    {
        if (isDead || target == null) return;
        BossLogic();
    }

    protected abstract void BossLogic();
}
