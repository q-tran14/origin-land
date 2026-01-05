using UnityEngine;

public class WeaponSummonButton : MonoBehaviour
{
    public EnemyType summonType;

    void OnMouseDown()
    {
        if (EnemySpawner.Instance == null) return;

        EnemySpawner.Instance.SpawnBoss(summonType);

        gameObject.SetActive(false); // chỉ summon 1 lần
    }
}
