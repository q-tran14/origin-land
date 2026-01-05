using UnityEngine;
using System.IO;

public static class GamePaths
{
    public static string GameDataFolder
    {
        get
        {
            return Path.Combine(
                Application.persistentDataPath,
                "GameData"
            );
        }
    }

    public static string CharacterDataPath
    {
        get
        {
            return Path.Combine(
                GameDataFolder,
                "data.json"
            );
        }
    }
}
