using System.IO;
using System.Linq;
using UnityEngine;

public static class HexMapSaveSystem
{
    private static readonly string SaveFolder = Path.Combine(Application.dataPath, "Maps/Seeds");

    public static void SaveMap(MapSaveData mapData)
    {
        if (!Directory.Exists(SaveFolder)) Directory.CreateDirectory(SaveFolder);
        string path = Path.Combine(SaveFolder, $"Seed_{mapData.seed}.json");
        string json = JsonUtility.ToJson(mapData, true);
        File.WriteAllText(path, json);
        Debug.Log($"Map saved at: {path}");
    }

    public static MapSaveData LoadMapBySeed(int seed)
    {
        string path = Path.Combine(SaveFolder, $"Seed_{seed}.json");
        if (!File.Exists(path))
        {
            Debug.LogWarning($"Không tìm thấy file map Seed_{seed}.json");
            return null;
        }

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<MapSaveData>(json);
    }
}
