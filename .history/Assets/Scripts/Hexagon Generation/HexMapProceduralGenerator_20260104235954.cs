using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using Range = UnityEngine.RangeAttribute;

[ExecuteInEditMode] // cho phép chạy trong Editor để test dễ hơn
public class HexMapProceduralGenerator : MonoBehaviour
{
    #region PROPERTIES
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
    [SerializeField] private TileData[,] mapData;
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
    [SerializeField] private ObjSpawn objSpawner;

    #endregion

    public void Start() => GenerateMap();

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
            Debug.Log($"Generated new random seed: {seed}");
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
                    isWall = IsWallPrefab(mapTilePrefab)
                });
            }
        }

        // Loại bỏ các tile không phù hợp
        // FixInvalidTile(tiles, holderObject);

        // Gán vào mapData để truy cập nhanh
        mapData = new TileData[mapWidth, mapHeight];
        foreach (var t in tiles) mapData[t.x, t.z] = t;

        // Thay các tile ven biển trong scene và cập nhật tiles
        DetectAndMarkCoasts(tiles, holderObject);

        #region NAVMESH BUILD & SAVE MAP DATA
        // Rebuild NavMesh after map is generated
        if (navMeshSurface != null)
        {
            Debug.Log("Rebuilding NavMesh...");
            navMeshSurface.RemoveData();
            navMeshSurface.BuildNavMesh();
        }
        else Debug.LogWarning("NavMeshSurface not assigned!");

        // Gọi Spawn Obj ở đây và trả về ObjectData[]
        ObjectData[] objs = objSpawner.SpawnObject(mapData, holderObject, instantiateInScene: true);

        MapSaveData saveData = new MapSaveData
        {
            width = mapWidth,
            height = mapHeight,
            seed = seed,
            mapName = "?????",
            tiles = tiles.ToArray(),
            objects = objs
        };
        HexMapSaveSystem.SaveMap(saveData);
        #endregion
    }
    #region FIX INVALID TILE
    void FixInvalidTile(List<TileData> tiles, GameObject holderObject)
    {
        System.Random prng = new System.Random();

        foreach (var tile in tiles)
        {
            if (tile == null || tile.isWater) continue;

            int x = tile.x;
            int z = tile.z;

            var neighbors = GetNeighbors(x, z);
            List<int> waterDirs = new List<int>();

            if (waterDirs.Count == 0) continue; // không ven biển

            bool needsFix = false;

            // Tile có 5 hoặc 6 cạnh giáp nước → cần thay
            if (waterDirs.Count >= 5) needsFix = true;
            else if (waterDirs.Count > 1)
            {
                // Thứ duyệt neighbor từ 0 - 5: phải, phải trên, trái trên, trái, trái dưới, phải dưới
                int interruptions = 0;

                for (int i = 0; i < 6; i++)
                {
                    bool currentIsWater = neighbors[i] != null && neighbors[i].isWater;
                    bool nextIsWater = neighbors[(i + 1) % 6] != null && neighbors[(i + 1) % 6].isWater;

                    // Nếu nước bị ngắt bởi đất liền giữa 2 neighbor liên tiếp
                    if (currentIsWater && !nextIsWater)
                    {
                        interruptions++;
                    }
                }

                // Nếu có nhiều hơn 1 đoạn nước liên tiếp => có khoảng trống => không liền mạch
                if (interruptions > 1) needsFix = true;
            }

            if (!needsFix) continue;

            // --- Xác định vị trí tile trong scene ---
            float hexWidth = 2f;
            float hexHeight = 2.309f;
            float posX = x * hexWidth + (z % 2 == 1 ? hexWidth / 2f : 0f);
            float posZ = z * (hexHeight * 0.75f);
            Vector3 position = new Vector3(posX, 0, posZ);

            // --- Xóa tile cũ trong scene ---
            Transform oldTile = null;
            foreach (Transform child in holderObject.transform)
            {
                if (Vector3.Distance(child.position, position) < 0.01f)
                {
                    oldTile = child;
                    break;
                }
            }
            if (oldTile != null) GameObject.DestroyImmediate(oldTile.gameObject);

            // --- Spawn tile nước mới ---
            GameObject waterTile = GetWaterTile();
            if (waterTile != null)
            {
                GameObject newTile = GameObject.Instantiate(waterTile, position, Quaternion.identity, holderObject.transform);
                newTile.name = waterTile.name;

                // --- Cập nhật dữ liệu ---
                tile.prefabName = waterTile.name;
                tile.biomeName = GetBiomeNameFromPrefab(waterTile); ;
                tile.yRotation = 0;
                tile.isWater = true;
                tile.isWall = false;
            }
        }
    }



    #endregion
    #region UTILITIES FUNCTION FOR COAST DETECT
    void DetectAndMarkCoasts(List<TileData> tiles, GameObject holderObject)
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int z = 0; z < mapHeight; z++)
            {
                var tile = mapData[x, z];
                if (tile == null || tile.isWater) continue;

                var neighbors = GetNeighbors(x, z);
                List<int> waterDirs = new List<int>();

                for (int dir = 0; dir < neighbors.Count; dir++)
                {
                    var n = neighbors[dir];
                    if (n != null && n.isWater) waterDirs.Add(dir);
                }

                if (waterDirs.Count == 0) continue; // không ven biển

                // Chọn prefab bờ biển phù hợp
                GameObject coastPrefab = null;
                if (waterDirs.Count == 1) coastPrefab = coastTiles[0];
                else if (waterDirs.Count == 2) coastPrefab = coastTiles[1];
                else if (waterDirs.Count == 3) coastPrefab = coastTiles[2];
                else if (waterDirs.Count == 4) coastPrefab = coastTiles[3];
                else coastPrefab = coastCornerTile;

                //! Tính góc xoay trung bình hướng ra biển
                float dirAngle = GetCoastRotation(neighbors, waterDirs.Count - 1);

                // --- Xác định vị trí tile trong scene ---
                float hexWidth = 2f;
                float hexHeight = 2.309f;
                float posX = x * hexWidth + (z % 2 == 1 ? hexWidth / 2f : 0f);
                float posZ = z * (hexHeight * 0.75f);
                Vector3 position = new Vector3(posX, 0, posZ);

                // --- Tìm & xóa tile cũ ---
                Transform oldTile = null;
                foreach (Transform child in holderObject.transform)
                {
                    if (Vector3.Distance(child.position, position) < 0.01f)
                    {
                        oldTile = child;
                        break;
                    }
                }

                if (oldTile != null) GameObject.DestroyImmediate(oldTile.gameObject);

                // --- Spawn tile bờ biển mới ---
                GameObject newTile = GameObject.Instantiate(coastPrefab, position, Quaternion.Euler(0, dirAngle, 0), holderObject.transform);
                newTile.name = coastPrefab.name;

                // --- Cập nhật dữ liệu ---
                tile.prefabName = coastPrefab.name;
                tile.biomeName = "Coast";
                tile.yRotation = dirAngle;
                tile.isWater = false;
                tile.isWall = false;
            }
        }
    }

    float GetCoastRotation(List<TileData> neighbors, int tileIndex)
    {
        if (neighbors == null || neighbors.Count != 6) return 0f;
        // Thứ duyệt neighbor từ 0 - 5: phải, phải trên, trái trên, trái, trái dưới, phải dưới
        // --- Định nghĩa mặt bờ biển mặc định của từng tile (spawn ra 0°) ---
        // true = cạnh có bờ biển, false = cạnh không bờ biển
        bool[][] coastFaces = new bool[5][]
        {
            new bool[6] { false, false, true, false, false, false }, // Tile 1 (phải, phải trên, trái trên, trái, trái dưới, phải dưới)
            new bool[6] { false, true, true, false, false, false },  // Tile 2
            new bool[6] { false, true, true, true, false, false },   // Tile 3
            new bool[6] { false, true, true, true, true, false },    // Tile 4 
            new bool[6] { false, true, true, false, false, false }   // Tile 5 (corner)
        };

        bool[] faces = coastFaces[tileIndex];

        for (int rotStep = 0; rotStep < 6; rotStep++)
        {
            bool match = true;

            for (int edge = 0; edge < 6; edge++)
            {
                int checkEdge = (edge + rotStep) % 6;
                var neighbor = neighbors[edge];

                if (faces[checkEdge] != neighbor.isWater)
                {
                    match = false;
                    break;
                }
            }

            if (match) return rotStep * 60f; // mỗi bước xoay là 60 độ
        }

        return 0;
    }

    List<TileData> GetNeighbors(int x, int z)
    {
        var result = new List<TileData>();
        var dirs = (z % 2 == 0) ? hexNeighborsEven : hexNeighborsOdd;
        foreach (var dir in dirs)
        {
            int nx = x + dir.x;
            int nz = z + dir.y;
            if (nx >= 0 && nx < mapWidth && nz >= 0 && nz < mapHeight) result.Add(mapData[nx, nz]);
            else result.Add(null);
        }
        return result;
    }
    #endregion

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

    GameObject GetWaterTile(System.Random prng = null)
    {
        foreach (BiomeType biome in biomes)
        {
            if (biome.isWater && biome.mapTiles.Length > 0)
            {
                int index = prng != null ? prng.Next(0, biome.mapTiles.Length) : 0;
                return biome.mapTiles[index];
            }
        }
        return null;
    }

    string GetBiomeNameFromPrefab(GameObject prefab)
    {
        foreach (var biome in biomes)
        {
            if (biome.mapTiles.Contains(prefab)) return biome.name;
        }
        return "Unknown";
    }

    bool IsWaterPrefab(GameObject prefab)
    {
        foreach (var biome in biomes)
        {
            if (biome.isWater && biome.mapTiles.Contains(prefab)) return true;
        }
        return false;
    }

    bool IsWallPrefab(GameObject prefab)
    {
        foreach (var biome in biomes)
        {
            if (biome.isWall && biome.mapTiles.Contains(prefab)) return true;
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

    #region NOT TOUCH
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

        string targetLayer = "";

        if (tileInstance.name.Contains("_coast_")) targetLayer = "Ground";

        if (biomeType != null) targetLayer = biomeType.isWater ? "Water" : biomeType.isWall ? "Wall" : "Ground";

        // Gán layer Ground hoặc Water hoặc Wall
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
        modifier.area = (tileInstance.layer != LayerMask.NameToLayer("Ground"))
            ? NavMesh.GetAreaFromName("Not Walkable")
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

    private void LoadMapFromSavedData(MapSaveData data)
    {
        GameObject holderObject = new GameObject("MapHolder");
        holderObject.transform.parent = transform;

        mapWidth = data.width;
        mapHeight = data.height;

        foreach (var tile in data.tiles)
        {
            GameObject prefab = null;

            if (tile.biomeName == "Coast")
            {
                // Nếu tile là coast
                if (tile.prefabName.Contains("coast_E") && coastCornerTile != null) prefab = coastCornerTile;
                else if (coastTiles != null && coastTiles.Length > 0)
                {
                    prefab = coastTiles.FirstOrDefault(p => p != null && p.name == tile.prefabName);
                    if (prefab == null)
                    {
                        // fallback: nếu không tìm thấy prefab, lấy prefab đầu tiên
                        prefab = coastTiles[0];
                        Debug.LogWarning($"⚠️ Coast prefab {tile.prefabName} không còn tồn tại, thay bằng {prefab.name}");
                    }
                }
            }
            else
            {
                // tile bình thường dựa trên biomes
                BiomeType biome = biomes.FirstOrDefault(b => b.name == tile.biomeName);
                if (biome == null || biome.mapTiles.Length == 0) continue;

                prefab = biome.mapTiles.FirstOrDefault(p => p != null && p.name == tile.prefabName);
                if (prefab == null)
                {
                    prefab = biome.mapTiles[0];
                    Debug.LogWarning($"⚠️ Prefab {tile.prefabName} không còn tồn tại, thay bằng {prefab.name}");
                }
            }

            if (prefab == null) continue; // nếu vẫn null thì bỏ qua

            // Tính vị trí
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

    #endregion
}
