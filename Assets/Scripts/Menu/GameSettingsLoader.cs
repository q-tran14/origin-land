using UnityEngine;
using System.IO;

public class GameSettingsLoader : MonoBehaviour
{
    void Awake()
    {
        if (!File.Exists(SavePaths.SettingsPath))
            return;

        string json = File.ReadAllText(SavePaths.SettingsPath);
        GameSettings settings =
            JsonUtility.FromJson<GameSettings>(json);

        AudioListener.volume = settings.volume;
        QualitySettings.SetQualityLevel(settings.graphics);
    }
}
