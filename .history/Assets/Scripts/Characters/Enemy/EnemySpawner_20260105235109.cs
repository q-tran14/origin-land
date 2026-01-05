using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;

    [Header("Boss Prefabs")]
    public GameObject axeShieldBossPrefab;
    public GameObject mageBossPrefab;

    [Header("Minion Prefab")]
    public GameObject minionPrefab;

    [Header("Spawn Points")]
    public Transform bossSpawnPoint;

    List<Enemy> aliveEnemies = new List<Enemy>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // ================= SPAWN BOSS =================
    public void SpawnBoss(EnemyType type)
    {
        GameObject prefab = null;

        switch (type)
        {
            case EnemyType.SkeletonWarrior:
                prefab = axeShieldBossPrefab;
                break;

            case EnemyType.SkeletonMage:
                prefab = mageBossPrefab;
                break;
        }

        if (prefab == null)
        {
            Debug.LogError("EnemySpawner: Boss prefab missing!");
            return;
        }

        GameObject bossObj = Instantiate(
            prefab,
            bossSpawnPoint.position,
            bossSpawnPoint.rotation
        );

        RegisterEnemy(bossObj.GetComponent<Enemy>());
    }

    // ================= REGISTER =================
    public void RegisterEnemy(Enemy enemy)
    {
        if (enemy == null) return;

        if (!aliveEnemies.Contains(enemy))
            aliveEnemies.Add(enemy);
    }

    // ================= KILL NOTIFY =================
    public void NotifyEnemyKilled(Enemy enemy)
    {
        if (aliveEnemies.Contains(enemy))
            aliveEnemies.Remove(enemy);

        CheckEncounterComplete();
    }

    // ================= CHECK WIN =================
    void CheckEncounterComplete()
    {
        if (aliveEnemies.Count == 0)
        {
            Debug.Log("🎉 ALL ENEMIES DEFEATED – ENCOUNTER COMPLETE");
            // Reward, quest, mở cửa, v.v...
        }
    }
}
