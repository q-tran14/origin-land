using UnityEngine;

public class WeaponSummonButton : MonoBehaviour
{
    public EnemyType summonType;
    bool used;

    void OnMouseDown()
    {
        if (used) return;

        used = true;
        EnemySpawner.Instance.SpawnBoss(summonType);
    }
}
