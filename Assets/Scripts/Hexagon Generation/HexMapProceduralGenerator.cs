using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using System.IO;
using System.Linq;
using System.Collections.Generic;

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
        public bool isWall;
    }

    [Header("Map Settings")]
    public BiomeType[] biomes;
    public int mapWidth = 10;
    public int mapHeight = 10;
    public float scale = 1.0f;
    public float oceanWidth = 0.3f;
    public float tileSpacing = 1.0f;

    [Header("Coast Tiles")]
    public GameObject[] coastTiles;      // 0–5 cạnh giáp nước
    public GameObject coastCornerTile;   // tile đỉnh biển

    [Header("NavMesh Settings")]
    public NavMeshSurface navMeshSurface;

    [Header("Seed Settings")]
    public int seed = -1;
    private TileData[,] mapData;
    private const float xOffsetRange = 200f;
    private const float yOffsetRange = 200f;

    private static readonly Vector2Int[] hexNeighborsEven = {
        new Vector2Int(+1, 0), new Vector2Int(0, +1), new Vector2Int(-1, +1),
        new Vector2Int(-1, 0), new Vector2Int(-1, -1), new Vector2Int(0, -1)
    };

    private static readonly Vector2Int[] hexNeighborsOdd = {
        new Vector2Int(+1, 0), new Vector2Int(+1, +1), new Vector2Int(0, +1),
        new Vector2Int(-1, 0), new Vector2Int(0, -1), new Vector2Int(+1, -1)
    };

    void Start() => GenerateMap();

    public void ClearMap()
    {
        Transform oldHolder = transform.Find("MapHolder");
        if (oldHolder != null)
        {
            // Dọn sạch trong Editor mà không cần Play
            if (Application.isPlaying) Destroy(oldHolder.gameObject);
            else DestroyImmediate(oldHolder.gameObject);
        }
    }

    public void GenerateMap()
    {
        ClearMap();
#region CHECK EXISTED SEED
        if (seed == -1)
        {
            seed = System.Environment.TickCount;
            Debug.Log($"🧬 Generated new random seed: {seed}");
            Random.InitState(seed);
        }
        else
        {
            var savedData = HexMapSaveSystem.LoadMapBySeed(seed);
            if (savedData != null)
            {
                Debug.Log($"====== Found saved map for seed {seed}, loading...");
                LoadMapFromSavedData(savedData);
                return;
            }
        }
#endregion

        Debug.Log($"===== No saved map for seed {seed}, generating new map...");

        System.Random prng = new System.Random(seed);

        GameObject holderObject = new GameObject("MapHolder");
        holderObject.transform.parent = transform;

        float xOffset = (float)(prng.NextDouble() * 2 * xOffsetRange - xOffsetRange);
        float yOffset = (float)(prng.NextDouble() * 2 * yOffsetRange - yOffsetRange);

        float[,] falloffMap = GenerateFalloffMap(mapWidth, mapHeight);
        List<TileData> tiles = new List<TileData>();

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                float xCoord = ((float)x / mapWidth + xOffset) * scale;
                float yCoord = ((float)y / mapHeight + yOffset) * scale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord) - falloffMap[x, y];

                GameObject mapTilePrefab = DetermineMapTile(sample, prng);
                if (mapTilePrefab == null) continue;

                float hexWidth = 2f;
                float hexHeight = 2.309f;
                float posX = x * hexWidth + (y % 2 == 1 ? hexWidth / 2f : 0f);
                float posZ = y * (hexHeight * 0.75f);
                Vector3 position = new Vector3(posX, 0, posZ);

                GameObject tileInstance = Instantiate(mapTilePrefab, position, Quaternion.identity, holderObject.transform);

                tileInstance.name = mapTilePrefab.name; // xóa (Clone)

                ApplyLayerAndNavModifier(tileInstance);
                tiles.Add(new TileData
                {
                    x = x,
                    z = y,
                    yRotation = 0,
                    biomeName = GetBiomeNameFromPrefab(mapTilePrefab),
                    prefabName = mapTilePrefab.name,
                    isWater = IsWaterPrefab(mapTilePrefab),
                    isWall = false
                });
            }
        }

#region NAVMESH BUILD & SAVE MAP DATA
        // Rebuild NavMesh after map is generated
        if (navMeshSurface != null)
        {
            Debug.Log("Rebuilding NavMesh...");
            navMeshSurface.RemoveData();
            navMeshSurface.BuildNavMesh();
        }
        else Debug.LogWarning("NavMeshSurface not assigned!");

        MapSaveData saveData = new MapSaveData
        {
            width = mapWidth,
            height = mapHeight,
            seed = seed,
            mapName = "?????",
            tiles = tiles.ToArray()
        };
        HexMapSaveSystem.SaveMap(saveData);
#endregion
    }

    private void LoadMapFromSavedData(MapSaveData data)
    {
        GameObject holderObject = new GameObject("MapHolder");
        holderObject.transform.parent = transform;

        mapWidth = data.width;
        mapHeight = data.height;

        foreach (var tile in data.tiles)
        {
            BiomeType biome = biomes.FirstOrDefault(b => b.name == tile.biomeName);
            if (biome == null || biome.mapTiles.Length == 0) continue;

            GameObject prefab = biome.mapTiles.FirstOrDefault(p => p != null && p.name == tile.prefabName);
            if (prefab == null)
            {
                // fallback: nếu không tìm thấy thì lấy prefab đầu tiên
                prefab = biome.mapTiles[0];
                Debug.LogWarning($"⚠️ Prefab {tile.prefabName} không còn tồn tại, thay bằng {prefab.name}");
            }

            float hexWidth = 2f;
            float hexHeight = 2.309f;
            float posX = tile.x * hexWidth + (tile.z % 2 == 1 ? hexWidth / 2f : 0f);
            float posZ = tile.z * (hexHeight * 0.75f);
            Vector3 position = new Vector3(posX, 0, posZ);

            Quaternion rotation = Quaternion.Euler(0, tile.yRotation, 0);
            GameObject instance = Instantiate(prefab, position, rotation, holderObject.transform);
            instance.name = prefab.name;

            ApplyLayerAndNavModifier(instance);
        }

        if (navMeshSurface != null)
        {
            navMeshSurface.RemoveData();
            navMeshSurface.BuildNavMesh();
        }
    }

    GameObject DetermineMapTile(float sample, System.Random prng)
    {
        if (sample <= 0) return GetWaterTile(prng);

        float cumulative = 0f;
        foreach (BiomeType biome in biomes)
        {
            cumulative += biome.percentage;
            if (sample * 100f <= cumulative)
            {
                int index = prng.Next(0, biome.mapTiles.Length);
                return biome.mapTiles[index];
            }
        }

        BiomeType defaultBiome = biomes[biomes.Length - 1];
        int defaultIndex = prng.Next(0, defaultBiome.mapTiles.Length);
        return defaultBiome.mapTiles[defaultIndex];
    }

    GameObject GetWaterTile(System.Random prng)
    {
        foreach (BiomeType biome in biomes)
        {
            if (biome.isWater && biome.mapTiles.Length > 0)
            {
                int index = prng.Next(0, biome.mapTiles.Length);
                return biome.mapTiles[index];
            }
        }
        return null;
    }

    string GetBiomeNameFromPrefab(GameObject prefab)
    {
        foreach (var biome in biomes)
        {
            if (biome.mapTiles.Contains(prefab))
                return biome.name;
        }
        return "Unknown";
    }

    bool IsWaterPrefab(GameObject prefab)
    {
        foreach (var biome in biomes)
        {
            if (biome.isWater && biome.mapTiles.Contains(prefab))
                return true;
        }
        return false;
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
        return Mathf.Pow(value * oceanWidth, a) / (Mathf.Pow(value * oceanWidth, a) + Mathf.Pow(b - b * value * oceanWidth, a));
    }


    /// <summary>
    /// Áp dụng Layer và NavMeshModifier cho tile
    /// </summary>
    /// <param name="tileInstance">Hexagon Tile Game Object</param>
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

        // Gán layer Ground hoặc Water hoặc Wall
        string targetLayer = biomeType.isWater ? "Water" : biomeType.isWall ? "Wall" : "Ground";
        int layerIndex = LayerMask.NameToLayer(targetLayer);
        if (layerIndex == -1)
        {
            Debug.LogWarning($"Layer '{targetLayer}' chưa tồn tại! Tạo trong Edit > Project Settings > Tags and Layers.");
            return;
        }
        tileInstance.layer = layerIndex;

        // NavMeshModifier để loại Water và Wall
        var modifier = tileInstance.GetComponent<NavMeshModifier>();
        if (modifier == null) modifier = tileInstance.AddComponent<NavMeshModifier>();

        modifier.overrideArea = true;
        modifier.area = biomeType.isWater
            ? NavMesh.GetAreaFromName("Not Walkable")
            : biomeType.isWall ? NavMesh.GetAreaFromName("Not Walkable")
            : NavMesh.GetAreaFromName("Walkable");

        // Đảm bảo mesh có thể đọc NavMesh runtime (fix lỗi read access)
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

    public void LoadMapFromSeed(int newSeed)
    {
        seed = newSeed;
        GenerateMap();
    }
}
