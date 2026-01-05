using UnityEngine;
using System.Collections.Generic;

public class BossController : Enemy
{
    [Header("Minion Settings")]
    public GameObject minionPrefab;
    public int minionCount = 3;
    public float minionSpawnRadius = 2.5f;

    List<Enemy> minions = new List<Enemy>();
    EnemySpawner spawner;

    bool hasSpawnedMinions;

    // ================= INIT =================
    protected override void Awake()
    {
        base.Awake();
        spawner = EnemySpawner.Instance;
    }

    protected override void Start()
    {
        base.Start();
        PlaySpawn();
    }

    void PlaySpawn()
    {
        animator.SetTrigger("Spawn");
    }

    // ================= SPAWN MINIONS =================
    // 🔥 GỌI BẰNG ANIMATION EVENT
    public void SpawnMinions()
    {
        if (hasSpawnedMinions || minionPrefab == null) return;

        hasSpawnedMinions = true;

        for (int i = 0; i < minionCount; i++)
        {
            Vector3 offset = Random.insideUnitSphere * minionSpawnRadius;
            offset.y = 0;

            GameObject m = spawner.SpawnAt(
                minionPrefab,
                transform.position + offset
            );

            if (m == null) continue;

            Enemy enemy = m.GetComponent<Enemy>();
            if (enemy != null)
                minions.Add(enemy);
        }
    }

    // ================= CHECK WIN =================
    protected override void Die()
    {
        base.Die();

        CheckEncounterComplete();
    }

    void CheckEncounterComplete()
    {
        minions.RemoveAll(m => m == null);

        if (minions.Count == 0)
        {
            Debug.Log("✅ Boss & Minions defeated!");
            // Gọi quest, reward, open door, v.v...
        }
    }
}
