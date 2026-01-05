using System.Collections.Generic;
using UnityEngine;

public enum BiomeType
{
    Grassland,
    Desert,
    Snow,
    Mountain,
    Water
}

[CreateAssetMenu(fileName = "TileLibrary", menuName = "HexMap/Tile Library")]
public class TileLibrary : ScriptableObject
{
    [System.Serializable]
    public class BiomeEntry
    {
        public BiomeType name;
        public GameObject[] mapTiles;
        public bool isWater;
        public bool isWall;
    }

    [Header("Biome Prefabs")]
    public List<BiomeEntry> biomes = new List<BiomeEntry>();

    [Header("Coast Prefabs (0–5 cạnh nước)")]
    public GameObject[] coastTiles;

    [Header("Corner Coast Prefab (1 góc nước)")]
    public GameObject coastCornerTile;

    /// <summary>
    /// Tìm prefab theo tên (cho việc load map)
    /// </summary>
    public GameObject GetPrefabByName(string prefabName)
    {
        // 🔍 Tìm trong biomes
        foreach (var biome in biomes)
        {
            foreach (var prefab in biome.mapTiles)
            {
                if (prefab != null && prefab.name == prefabName)
                    return prefab;
            }
        }

        // 🔍 Tìm trong coast tiles
        foreach (var coast in coastTiles)
        {
            if (coast != null && coast.name == prefabName)
                return coast;
        }

        // 🔍 Tìm trong corner
        if (coastCornerTile != null && coastCornerTile.name == prefabName)
            return coastCornerTile;

        Debug.LogWarning($"Prefab '{prefabName}' không tồn tại trong TileLibrary!");
        return null;
    }

    /// <summary>
    /// Lấy thông tin biome theo tên
    /// </summary>
    public BiomeEntry GetBiomeByName(BiomeType biomeName)
    {
        return biomes.Find(b => b.name == biomeName);
    }
}
