using UnityEngine;

public class ObjSpawn : MonoBehaviour
{
    [System.Serializable]
    public class objsToSpawn
    {
        [Tooltip("Name of your asset")]
        public string assetName;
        public GameObject asset;
        [Tooltip("probability to spawn it")]
        public int probabilityToSpawn;
    }

    public objsToSpawn[] obj;
    [Tooltip("Probability to spawn nothing")]
    public int probabilityToSpawnNothing;

    void Start()
    {
        GameObject selectedAsset = SelectRandomAsset();

        if (selectedAsset != null)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();
            Instantiate(selectedAsset, spawnPosition, Quaternion.identity, transform);
        }
    }

    GameObject SelectRandomAsset()
    {
        if (obj == null || obj.Length == 0) return null;
        int totalProbability = probabilityToSpawnNothing;
        foreach (objsToSpawn asset in obj)
        {
            totalProbability += asset.probabilityToSpawn;
        }

        int randomValue = Random.Range(0, totalProbability);
        int cumulativeProbability = 0;

        if (randomValue < probabilityToSpawnNothing)
        {
            return null;
        }

        cumulativeProbability += probabilityToSpawnNothing;

        foreach (objsToSpawn asset in obj)
        {
            cumulativeProbability += asset.probabilityToSpawn;
            if (randomValue < cumulativeProbability)
            {
                return asset.asset;
            }
        }

        return obj[0].asset;
    }

    Vector3 GetRandomSpawnPosition()
    {
        Bounds bounds = GetComponent<Renderer>().bounds;

        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomZ = Random.Range(bounds.min.z, bounds.max.z);
        float spawnY = bounds.max.y;

        return new Vector3(randomX, spawnY, randomZ);
    }
}
