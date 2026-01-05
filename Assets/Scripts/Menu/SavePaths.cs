using UnityEngine;
using System.IO;

public static class SavePaths
{
    public static string SettingsPath
    {
        get
        {
            return Path.Combine(
                Application.persistentDataPath,
                "GameData",
                "settings.json"
            );
        }
    }
}
