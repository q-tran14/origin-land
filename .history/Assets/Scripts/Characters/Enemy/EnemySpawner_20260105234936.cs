using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // ================= SPAWN =================
    public GameObject Spawn(GameObject prefab)
    {
        if (prefab == null || spawnPoints.Length == 0)
        {
            Debug.LogError("EnemySpawner: Missing prefab or spawn points!");
            return null;
        }

        Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject obj = Instantiate(prefab, point.position, point.rotation);

        return obj;
    }

    public GameObject SpawnAt(GameObject prefab, Vector3 pos)
    {
        if (prefab == null)
        {
            Debug.LogError("EnemySpawner: Missing prefab!");
            return null;
        }

        return Instantiate(prefab, pos, Quaternion.identity);
    }
}
