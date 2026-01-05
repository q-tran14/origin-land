using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;

    public GameObject warriorBossPrefab;
    public GameObject mageBossPrefab;
    public GameObject minionPrefab;

    public Transform bossSpawnPoint;

    private void Awake()
    {
        Instance = this;
    }

    public void SpawnBoss(EnemyType type)
    {
        GameObject boss = null;

        if (type == EnemyType.SkeletonWarrior)
            boss = Instantiate(warriorBossPrefab, bossSpawnPoint.position, Quaternion.identity);

        if (type == EnemyType.SkeletonMage)
            boss = Instantiate(mageBossPrefab, bossSpawnPoint.position, Quaternion.identity);

        boss.GetComponent<BossController>().SummonMinions(3);
    }
}
