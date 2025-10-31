using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

[ExecuteInEditMode] // cho phép chạy trong Editor để test dễ hơn
public class HexMapProceduralGenerator : MonoBehaviour
{
    [System.Serializable]
    public class BiomeType
    {
        public string name;
        public GameObject[] mapTiles;
        [Range(0f, 1f)] public float percentage;
        public bool isWater;
    }

    [Header("Map Settings")]
    public BiomeType[] biomes;
    public int mapWidth = 10;
    public int mapHeight = 10;
    public float scale = 1.0f;
    public float oceanWidth = 0.3f;
    public float tileSpacing = 1.0f;

    [Header("NavMesh Settings")]
    public NavMeshSurface navMeshSurface;

    private const float xOffsetRange = 200f;
    private const float yOffsetRange = 200f;

    void Start()
    {
        Random.InitState(System.Environment.TickCount);
        GenerateMap();
    }

    public void ClearMap()
    {
        Transform oldHolder = transform.Find("MapHolder");
        if (oldHolder != null)
        {
            // Dọn sạch trong Editor mà không cần Play
            if (Application.isPlaying)
                Destroy(oldHolder.gameObject);
            else
                DestroyImmediate(oldHolder.gameObject);
        }
    }

    public void GenerateMap()
    {
        ClearMap();

        GameObject holderObject = new GameObject("MapHolder");
        holderObject.transform.parent = transform;

        float xOffset = Random.Range(-xOffsetRange, xOffsetRange);
        float yOffset = Random.Range(-yOffsetRange, yOffsetRange);

        float[,] falloffMap = GenerateFalloffMap(mapWidth, mapHeight);

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                float xCoord = ((float)x / mapWidth + xOffset) * scale;
                float yCoord = ((float)y / mapHeight + yOffset) * scale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord) - falloffMap[x, y];

                GameObject mapTilePrefab = DetermineMapTile(sample);
                if (mapTilePrefab == null) continue;

                float hexWidth = 2f;
                float hexHeight = 2.309f;
                float posX = x * hexWidth + (y % 2 == 1 ? hexWidth / 2f : 0f);
                float posZ = y * (hexHeight * 0.75f);
                Vector3 position = new Vector3(posX, 0, posZ);

                GameObject tileInstance = Instantiate(mapTilePrefab, position, Quaternion.identity, holderObject.transform);
                tileInstance.name = mapTilePrefab.name; // xóa (Clone)

                ApplyLayerAndNavModifier(tileInstance);
            }
        }

        // ✅ Sau khi sinh map, build lại NavMesh
        if (navMeshSurface != null)
        {
            Debug.Log("Rebuilding NavMesh...");
            navMeshSurface.RemoveData(); // tránh build chồng
            navMeshSurface.BuildNavMesh();
        }
        else
        {
            Debug.LogWarning("NavMeshSurface not assigned!");
        }
    }

    void ApplyLayerAndNavModifier(GameObject tileInstance)
    {
        // Tìm biome tương ứng
        BiomeType biomeType = null;
        foreach (BiomeType biome in biomes)
        {
            foreach (var prefab in biome.mapTiles)
            {
                if (prefab != null && prefab.name == tileInstance.name)
                {
                    biomeType = biome;
                    break;
                }
            }
            if (biomeType != null) break;
        }

        if (biomeType == null)
        {
            Debug.LogWarning($"Không tìm thấy biome cho tile {tileInstance.name}");
            return;
        }

        // ✅ Gán layer Ground hoặc Water
        string targetLayer = biomeType.isWater ? "Water" : "Ground";
        int layerIndex = LayerMask.NameToLayer(targetLayer);
        if (layerIndex == -1)
        {
            Debug.LogWarning($"⚠ Layer '{targetLayer}' chưa tồn tại! Tạo trong Edit > Project Settings > Tags and Layers.");
            return;
        }
        tileInstance.layer = layerIndex;

        // ✅ NavMeshModifier để loại Water
        var modifier = tileInstance.GetComponent<NavMeshModifier>();
        if (modifier == null) modifier = tileInstance.AddComponent<NavMeshModifier>();

        modifier.overrideArea = true;
        modifier.area = biomeType.isWater
            ? NavMesh.GetAreaFromName("Not Walkable")
            : NavMesh.GetAreaFromName("Walkable");

        // ✅ Đảm bảo mesh có thể đọc NavMesh runtime (fix lỗi read access)
        var meshFilters = tileInstance.GetComponentsInChildren<MeshFilter>();
        foreach (var mf in meshFilters)
        {
            if (mf.sharedMesh != null && !mf.sharedMesh.isReadable)
            {
                Debug.LogWarning($"Mesh '{mf.sharedMesh.name}' của {tileInstance.name} không cho đọc. " +
                                 $"→ Mở import settings của nó và tick 'Read/Write Enabled'.");
            }
        }
    }

    GameObject DetermineMapTile(float sample)
    {
        if (sample <= 0)
        {
            return GetWaterTile();
        }

        float cumulativePercentage = 0f;
        foreach (BiomeType biome in biomes)
        {
            cumulativePercentage += biome.percentage;
            if (sample * 100f <= cumulativePercentage)
            {
                int index = Random.Range(0, biome.mapTiles.Length);
                return biome.mapTiles[index];
            }
        }

        BiomeType defaultBiome = biomes[biomes.Length - 1];
        int defaultIndex = Random.Range(0, defaultBiome.mapTiles.Length);
        return defaultBiome.mapTiles[defaultIndex];
    }

    GameObject GetWaterTile()
    {
        foreach (BiomeType biome in biomes)
        {
            if (biome.isWater && biome.mapTiles.Length > 0)
            {
                int index = Random.Range(0, biome.mapTiles.Length);
                return biome.mapTiles[index];
            }
        }
        return null;
    }

    float[,] GenerateFalloffMap(int width, int height)
    {
        float[,] map = new float[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                float x = i / (float)width * 2 - 1;
                float y = j / (float)height * 2 - 1;
                float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                map[i, j] = Evaluate(value);
            }
        }
        return map;
    }

    float Evaluate(float value)
    {
        float a = 3f;
        float b = 2.2f;
        return Mathf.Pow(value * oceanWidth, a) /
               (Mathf.Pow(value * oceanWidth, a) + Mathf.Pow(b - b * value * oceanWidth, a));
    }
}
