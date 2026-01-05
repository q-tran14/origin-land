using UnityEngine;

public abstract class Boss : Enemy
{
    public float moveSpeed = 2.5f;

    protected override void Update()
    {
        base.Update();
        if (isDead || playerStats.IsDead) return;

        BossLogic();
    }

    protected abstract void BossLogic();
}
