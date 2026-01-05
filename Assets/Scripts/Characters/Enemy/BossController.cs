using UnityEngine;
using System.Collections.Generic;

public class BossController : MonoBehaviour
{
    [Header("Minion Settings")]
    public GameObject minionPrefab;
    public int minionCount = 3;
    public float spawnRadius = 2.5f;

    List<Enemy> minions = new List<Enemy>();
    bool spawned;

    // 🔥 gọi từ Animation Event của boss (Spawn animation)
    public void SpawnMinions()
    {
        if (spawned || minionPrefab == null) return;
        spawned = true;

        for (int i = 0; i < minionCount; i++)
        {
            Vector3 offset = Random.insideUnitSphere * spawnRadius;
            offset.y = 0;

            GameObject m = Instantiate(
                minionPrefab,
                transform.position + offset,
                Quaternion.identity
            );

            Enemy e = m.GetComponent<Enemy>();
            if (e != null)
                minions.Add(e);

            EnemySpawner.Instance.RegisterEnemy(e);
        }
    }
}
