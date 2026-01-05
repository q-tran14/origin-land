using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform[] spawnPoints;

    public GameObject Spawn(GameObject prefab)
    {
        Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
        return Instantiate(prefab, point.position, point.rotation);
    }
}
