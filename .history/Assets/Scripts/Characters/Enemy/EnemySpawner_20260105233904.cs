using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;

    public GameObject bossAxePrefab;
    public GameObject bossMagePrefab;
    public GameObject enemyNormalPrefab;

    public Transform bossSpawnPoint;
    public Transform[] minionSpawnPoints;

    List<Enemy> aliveEnemies = new List<Enemy>();

    void Awake()
    {
        Instance = this;
    }

    public void SpawnBoss(EnemyType type)
    {
        GameObject boss = null;

        if (type == EnemyType.SkeletonWarrior)
            boss = Instantiate(bossAxePrefab, bossSpawnPoint.position, Quaternion.identity);

        if (type == EnemyType.SkeletonMage)
            boss = Instantiate(bossMagePrefab, bossSpawnPoint.position, Quaternion.identity);

        RegisterEnemy(boss.GetComponent<Enemy>());

        // Spawn minions
        for (int i = 0; i < 3; i++)
        {
            Transform p = minionSpawnPoints[i];
            GameObject minion = Instantiate(enemyNormalPrefab, p.position, Quaternion.identity);
            RegisterEnemy(minion.GetComponent<Enemy>());
        }
    }

    void RegisterEnemy(Enemy enemy)
    {
        aliveEnemies.Add(enemy);
    }

    public void NotifyEnemyKilled(Enemy enemy)
    {
        aliveEnemies.Remove(enemy);

        if (aliveEnemies.Count == 0)
        {
            Debug.Log("🎉 PLAYER WIN – ALL ENEMIES DEAD");
        }
    }
}
