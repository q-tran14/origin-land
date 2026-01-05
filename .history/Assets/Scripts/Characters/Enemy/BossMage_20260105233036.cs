using UnityEngine;

public class BossMage : Boss
{
    public GameObject spellPrefab;
    public Transform castPoint;
    public float castCooldown = 3f;

    float timer;

    protected override void BossLogic()
    {
        timer -= Time.deltaTime;

        float dist = Vector3.Distance(transform.position, target.position);

        if (dist <= stats.detectRange && timer <= 0f)
        {
            CastSpell();
            timer = castCooldown;
        }

        LookAtTarget();
    }

    void CastSpell()
    {
        animator.SetTrigger("Cast");

        Instantiate(
            spellPrefab,
            castPoint.position,
            Quaternion.LookRotation(target.position - castPoint.position)
        );
    }

    void LookAtTarget()
    {
        Vector3 dir = target.position - transform.position;
        dir.y = 0;
        transform.rotation = Quaternion.LookRotation(dir);
    }
}
