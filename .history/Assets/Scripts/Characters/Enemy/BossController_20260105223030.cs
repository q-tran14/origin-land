using UnityEngine;

public class BossController : MonoBehaviour
{
    public Transform[] summonPoints;

    public void SummonMinions(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Transform point = summonPoints[i];
            Instantiate(
                EnemySpawner.Instance.minionPrefab,
                point.position,
                Quaternion.identity
            );
        }
    }
}
