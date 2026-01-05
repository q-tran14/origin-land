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

    [Header("Spawn Point")]
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
            Debug.LogError("EnemySpawner: Missing boss prefab!");
            return;
        }

        GameObject boss = Instantiate(
            prefab,
            bossSpawnPoint.position,
            bossSpawnPoint.rotation
        );

        RegisterEnemy(boss.GetComponent<Enemy>());
    }

    // ================= SPAWN AT POSITION =================
    public GameObject SpawnAt(GameObject prefab, Vector3 position)
    {
        if (prefab == null)
        {
            Debug.LogError("EnemySpawner: Prefab is NULL");
            return null;
        }

        GameObject obj = Instantiate(prefab, position, Quaternion.identity);
        RegisterEnemy(obj.GetComponent<Enemy>());

        return obj;
    }

    // ================= REGISTER =================
    public void RegisterEnemy(Enemy enemy)
    {
        if (enemy == null) return;

        if (!aliveEnemies.Contains(enemy))
            aliveEnemies.Add(enemy);
    }

    // ================= ENEMY KILLED =================
    public void NotifyEnemyKilled(Enemy enemy)
    {
        if (aliveEnemies.Contains(enemy))
            aliveEnemies.Remove(enemy);

        CheckEncounterComplete();
    }

    void CheckEncounterComplete()
    {
        if (aliveEnemies.Count == 0)
        {
            Debug.Log("🎉 ALL ENEMIES DEFEATED – ENCOUNTER COMPLETE");
            // reward / quest / mở cổng
        }
    }
}
