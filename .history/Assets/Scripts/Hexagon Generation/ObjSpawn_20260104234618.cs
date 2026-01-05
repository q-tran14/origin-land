using System.Collections.Generic;
using UnityEngine;

public class ObjSpawn : MonoBehaviour
{
    [System.Serializable]
    public class objsToSpawn
    {
        [Tooltip("Name of your asset")]
        public string assetName;
        public GameObject asset;
        [Tooltip("Probability to spawn it")]
        public int probabilityToSpawn;
        [Tooltip("Which layer of tile this object can spawn on (Ground, Water, Wall)")]
        public string tileLayer;
    }

    public objsToSpawn[] obj;
    [Tooltip("Probability to spawn nothing")]
    public int probabilityToSpawnNothing = 50;

    [Tooltip("Parent transform to hold all spawned objects")]
    public Transform objectHolder;

    /// <summary>
    /// Hàm spawn object theo tỉ lệ và layer tile
    /// </summary>
    public ObjectData[] SpawnObject(TileData[,] mapData, GameObject holderObject, bool instantiateInScene = true)
    {
        // đảm bảo objectHolder hợp lệ
        objectHolder = holderObject != null ? holderObject.transform : objectHolder;

        List<ObjectData> spawnedObjects = new List<ObjectData>();
        System.Random prng = new System.Random();

        int width = mapData.GetLength(0);
        int height = mapData.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                TileData tile = mapData[x, z];
                if (tile == null) continue;
                Debug.Log($"[MapGen] Tile({x},{z}) => isWater={tile.isWater}, isWall={tile.isWall}");
                // ----- XÁC ĐỊNH LAYER CHÍNH XÁC HƠN -----
                // ưu tiên isWater / isWall, nếu không có thì fallback bằng tên biome/prefab
                string layer = "Ground";
                if (tile.isWater) layer = "Water";
                else if (tile.isWall) layer = "Wall";
                else if (!string.IsNullOrEmpty(tile.biomeName) && tile.biomeName.Equals("Wall", System.StringComparison.OrdinalIgnoreCase)) layer = "Wall";
                else if (!string.IsNullOrEmpty(tile.prefabName) && tile.prefabName.ToLower().Contains("wall")) layer = "Wall";
                // (nếu muốn, thêm điều kiện khác để nhận diện Wall)

                // ----- Lọc danh sách prefab phù hợp (so sánh không phân biệt hoa thường) -----
                List<objsToSpawn> validObjs = new List<objsToSpawn>();
                if (obj != null)
                {
                    foreach (var o in obj)
                    {
                        if (string.Equals((o.tileLayer ?? "").Trim(), layer, System.StringComparison.OrdinalIgnoreCase))
                            validObjs.Add(o);
                    }
                }

                if (validObjs.Count == 0) continue; // Không có object nào phù hợp layer

                // ----- Tính tổng xác suất -----
                int totalProb = probabilityToSpawnNothing;
                foreach (var o in validObjs) totalProb += Mathf.Max(0, o.probabilityToSpawn);

                int randomValue = prng.Next(0, totalProb);
                int cumulative = 0;

                // ----- Trường hợp không spawn gì -----
                if (randomValue < probabilityToSpawnNothing) continue;

                cumulative += probabilityToSpawnNothing;

                GameObject prefabToSpawn = null;
                string prefabName = "";

                foreach (var o in validObjs)
                {
                    cumulative += Mathf.Max(0, o.probabilityToSpawn);
                    if (randomValue < cumulative)
                    {
                        prefabToSpawn = o.asset;
                        prefabName = string.IsNullOrEmpty(o.assetName) ? (o.asset != null ? o.asset.name : "") : o.assetName;
                        break;
                    }
                }

                if (prefabToSpawn == null) continue;

                // ----- Tính vị trí spawn trong hex map -----
                float hexWidth = 2f;
                float hexHeight = 2.309f;
                float posX = x * hexWidth + (z % 2 == 1 ? hexWidth / 2f : 0f);
                float posZ = z * (hexHeight * 0.75f);
                float posY = 0f;
                Vector3 spawnPos = new Vector3(posX, posY, posZ);

                GameObject instance = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity, parent);


                // ----- Ghi lại ObjectData để lưu file map -----
                ObjectData objData = new ObjectData
                {
                    x = x,
                    z = z,
                    yScale = 1,
                    yRotation = 0,
                    objectName = string.IsNullOrEmpty(prefabName) ? prefabToSpawn.name : prefabName
                };

                spawnedObjects.Add(objData);
            }
        }

        return spawnedObjects.ToArray();
    }

}
