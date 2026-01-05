using UnityEngine;

public class BossController : Enemy
{
    [Header("Minion")]
    public GameObject minionPrefab;
    public int minionCount = 3;

    EnemySpawner spawner;

    protected override void Awake()
    {
        base.Awake();
        spawner = FindObjectOfType<EnemySpawner>();
    }

    protected override void Start()
    {
        base.Start();
        SpawnMinions();
    }

    void SpawnMinions()
    {
        for (int i = 0; i < minionCount; i++)
        {
            spawner.Spawn(minionPrefab);
        }
    }
}
