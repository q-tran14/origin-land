using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class SettingsController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject settingsPanel;
    public GameObject mainMenuPanel;

    [Header("Audio")]
    public Slider volumeSlider;

    [Header("Graphics")]
    public TMP_Dropdown graphicsDropdown;

    [Header("Brightness")]
    public Slider brightnessSlider;
    public Image brightnessOverlay;

    string savePath;

    GameSettings settings;

    void Awake()
    {
        savePath = Application.persistentDataPath + "/settings.json";
        LoadSettings();
    }

    void ApplySettings()
    {
        // Volume
        AudioListener.volume = settings.volume;
        volumeSlider.value = settings.volume;

        // Graphics
        QualitySettings.SetQualityLevel(settings.graphics);
        graphicsDropdown.value = settings.graphics;

        // Brightness
        Color c = brightnessOverlay.color;
        c.a = settings.brightness;
        brightnessOverlay.color = c;
        brightnessSlider.value = settings.brightness;
    }

    public void ChangeVolume(float value)
    {
        settings.volume = value;
        AudioListener.volume = value;
    }

    public void ChangeGraphics(int index)
    {
        settings.graphics = index;
        QualitySettings.SetQualityLevel(index);
    }

    public void ChangeBrightness(float value)
    {
        settings.brightness = value;
        Color c = brightnessOverlay.color;
        c.a = value;
        brightnessOverlay.color = c;
    }

    public void SaveSettings()
    {
        string json = JsonUtility.ToJson(settings, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Settings saved: " + savePath);
    }

    void LoadSettings()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            settings = JsonUtility.FromJson<GameSettings>(json);
        }
        else
        {
            settings = new GameSettings()
            {
                volume = 1f,
                graphics = 2,
                brightness = 0f
            };
        }

        ApplySettings();
    }

    public void OpenSettings()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}
