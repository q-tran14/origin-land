using UnityEngine;

public class WeaponSummonButton : MonoBehaviour
{
    public EnemyType summonType;

    private void OnMouseDown()
    {
        EnemySpawner.Instance.SpawnBoss(summonType);
    }
}
public enum EnemyType
{
    SkeletonWarrior,
    SkeletonMage
}
