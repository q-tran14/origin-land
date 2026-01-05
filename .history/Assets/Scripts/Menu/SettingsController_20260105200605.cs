using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Collections;

public class SettingsController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject settingsPanel;
    public GameObject mainMenuPanel;
    public GameObject GameTitle;

    [Header("Audio")]
    public Slider volumeSlider;
    public AudioSource backgroundMusic;

    [Header("Graphics")]
    public TMP_Dropdown graphicsDropdown;

    [Header("Brightness")]
    public Slider brightnessSlider;
    public Image brightnessOverlay;

    [Header("Save Notification")]
    public GameObject saveNotificationPanel;
    public float notificationTime = 3f;

    Coroutine hideCoroutine;


    string savePath;

    GameSettings settings;

    void Awake()
    {
        savePath = SavePaths.SettingsPath;

        Directory.CreateDirectory(
            Path.GetDirectoryName(savePath)
        );
        

        LoadSettings();
    }

    void ApplySettings()
    {
        // Volume
        backgroundMusic.volume = settings.volume;
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
        backgroundMusic.volume = value;
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
        Debug.Log("Brightness: " + value);

    }

    public void SaveSettings()
    {
        string json = JsonUtility.ToJson(settings, true);
        File.WriteAllText(savePath, json);

        ShowSaveNotification();
    }

    void ShowSaveNotification()
    {
        saveNotificationPanel.SetActive(true);

        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        hideCoroutine = StartCoroutine(AutoHideNotification());
    }

    IEnumerator AutoHideNotification()
    {
        yield return new WaitForSeconds(notificationTime);
        saveNotificationPanel.SetActive(false);
    }

    public void CloseSaveNotification()
    {
        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        saveNotificationPanel.SetActive(false);
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
        AudioListener.pause = false;
        Time.timeScale = 1f;

        settingsPanel.SetActive(true);
        mainMenuPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        AudioListener.pause = false;
        Time.timeScale = 1f;

        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}
