using UnityEngine;
using System.Collections.Generic;

public class BossController : Enemy
{
    [Header("Minion Settings")]
    public GameObject minionPrefab;
    public int minionCount = 3;
    public float spawnRadius = 2.5f;

    List<Enemy> minions = new List<Enemy>();
    bool spawned;

    // 🔥 GỌI TỪ ANIMATION EVENT (Spawn clip)
    public void SpawnMinions()
    {
        if (spawned || minionPrefab == null) return;
        spawned = true;

        for (int i = 0; i < minionCount; i++)
        {
            Vector3 offset = Random.insideUnitSphere * spawnRadius;
            offset.y = 0;

            GameObject m = EnemySpawner.Instance.SpawnAt(
                minionPrefab,
                transform.position + offset
            );

            if (m == null) continue;

            Enemy e = m.GetComponent<Enemy>();
            if (e != null)
                minions.Add(e);
        }
    }

    protected override void Die()
    {
        base.Die();
        minions.Clear();
    }
}
