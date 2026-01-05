using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject settingsPanel;
    public GameObject mainMenuPanel;

    [Header("Audio")]
    public Slider volumeSlider;

    [Header("Graphics")]
    public TMP_Dropdown graphicsDropdown;

    void Start()
    {
        // Load saved settings
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", 1f);
        AudioListener.volume = volumeSlider.value;

        graphicsDropdown.value = PlayerPrefs.GetInt("Graphics", 2);
        QualitySettings.SetQualityLevel(graphicsDropdown.value);
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

    public void ChangeVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("Volume", value);
    }

    public void ChangeGraphics(int index)
    {
        QualitySettings.SetQualityLevel(index);
        PlayerPrefs.SetInt("Graphics", index);
    }
}
