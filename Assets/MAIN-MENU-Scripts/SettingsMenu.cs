using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("Volume")]
    public Slider volumeSlider;
    public TMP_Text volumeValueText;

    [Header("Mouse")]
    public Slider sensitivitySlider;
    public TMP_Text sensitivityValueText;

    [Header("Display")]
    public Toggle fullscreenToggle;
    public TMP_Dropdown qualityDropdown;
    public TMP_Dropdown aspectRatioDropdown;
    public TMP_Dropdown resolutionDropdown;

    private List<Vector2Int> currentResolutions = new List<Vector2Int>();

    private void Start()
    {
        SetupQualityDropdown();
        SetupAspectRatioDropdown();

        if (aspectRatioDropdown != null)
        {
            aspectRatioDropdown.onValueChanged.AddListener(delegate
            {
                OnAspectRatioChanged();
            });
        }

        LoadSettings();
        RefreshResolutionDropdown();
        ApplySettings();
    }

    private void Update()
    {
        UpdateTexts();
    }

    private void SetupQualityDropdown()
    {
        if (qualityDropdown == null)
            return;

        qualityDropdown.ClearOptions();

        List<string> options = new List<string>();

        foreach (string qualityName in QualitySettings.names)
        {
            options.Add(qualityName);
        }

        qualityDropdown.AddOptions(options);
    }

    private void SetupAspectRatioDropdown()
    {
        if (aspectRatioDropdown == null)
            return;

        aspectRatioDropdown.ClearOptions();

        List<string> options = new List<string>
        {
            "16:9",
            "4:3",
            "16:10"
        };

        aspectRatioDropdown.AddOptions(options);
    }

    public void OnAspectRatioChanged()
    {
        RefreshResolutionDropdown();
    }

    private void RefreshResolutionDropdown()
    {
        if (resolutionDropdown == null)
            return;

        resolutionDropdown.ClearOptions();
        currentResolutions.Clear();

        int aspectIndex = 0;

        if (aspectRatioDropdown != null)
            aspectIndex = aspectRatioDropdown.value;

        if (aspectIndex == 0)
        {
            // 16:9
            currentResolutions.Add(new Vector2Int(1280, 720));
            currentResolutions.Add(new Vector2Int(1366, 768));
            currentResolutions.Add(new Vector2Int(1600, 900));
            currentResolutions.Add(new Vector2Int(1920, 1080));
            currentResolutions.Add(new Vector2Int(2560, 1440));
            currentResolutions.Add(new Vector2Int(3840, 2160));
        }
        else if (aspectIndex == 1)
        {
            // 4:3
            currentResolutions.Add(new Vector2Int(800, 600));
            currentResolutions.Add(new Vector2Int(1024, 768));
            currentResolutions.Add(new Vector2Int(1280, 960));
            currentResolutions.Add(new Vector2Int(1440, 1080));
            currentResolutions.Add(new Vector2Int(1600, 1200));
        }
        else if (aspectIndex == 2)
        {
            // 16:10
            currentResolutions.Add(new Vector2Int(1280, 800));
            currentResolutions.Add(new Vector2Int(1440, 900));
            currentResolutions.Add(new Vector2Int(1680, 1050));
            currentResolutions.Add(new Vector2Int(1920, 1200));
            currentResolutions.Add(new Vector2Int(2560, 1600));
        }

        List<string> options = new List<string>();

        foreach (Vector2Int resolution in currentResolutions)
        {
            options.Add(resolution.x + " x " + resolution.y);
        }

        resolutionDropdown.AddOptions(options);

        int savedAspect = PlayerPrefs.GetInt("AspectRatioIndex", 0);
        int savedResolution = PlayerPrefs.GetInt("ResolutionIndex", GetDefaultResolutionIndex());

        if (aspectIndex == savedAspect)
        {
            resolutionDropdown.value = Mathf.Clamp(savedResolution, 0, currentResolutions.Count - 1);
        }
        else
        {
            resolutionDropdown.value = GetDefaultResolutionIndex();
        }

        resolutionDropdown.RefreshShownValue();
    }

    private int GetDefaultResolutionIndex()
    {
        int aspectIndex = 0;

        if (aspectRatioDropdown != null)
            aspectIndex = aspectRatioDropdown.value;

        if (aspectIndex == 0)
            return 3; // 1920x1080

        if (aspectIndex == 1)
            return 3; // 1440x1080

        if (aspectIndex == 2)
            return 3; // 1920x1200

        return 0;
    }

    private void LoadSettings()
    {
        if (volumeSlider != null)
            volumeSlider.value = PlayerPrefs.GetFloat("Volume", 1f);

        if (sensitivitySlider != null)
            sensitivitySlider.value = PlayerPrefs.GetFloat("MouseSensitivity", 2f);

        if (fullscreenToggle != null)
            fullscreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen", 1) == 1;

        if (qualityDropdown != null)
            qualityDropdown.value = PlayerPrefs.GetInt("Quality", QualitySettings.GetQualityLevel());

        if (aspectRatioDropdown != null)
            aspectRatioDropdown.value = PlayerPrefs.GetInt("AspectRatioIndex", 0);
    }

    public void ApplySettings()
    {
        float volume = 1f;
        float sensitivity = 2f;
        bool fullscreen = true;
        int quality = QualitySettings.GetQualityLevel();
        int aspectIndex = 0;
        int resolutionIndex = 0;

        if (volumeSlider != null)
            volume = volumeSlider.value;

        if (sensitivitySlider != null)
            sensitivity = sensitivitySlider.value;

        if (fullscreenToggle != null)
            fullscreen = fullscreenToggle.isOn;

        if (qualityDropdown != null)
            quality = qualityDropdown.value;

        if (aspectRatioDropdown != null)
            aspectIndex = aspectRatioDropdown.value;

        if (resolutionDropdown != null)
            resolutionIndex = resolutionDropdown.value;

        resolutionIndex = Mathf.Clamp(resolutionIndex, 0, currentResolutions.Count - 1);

        Vector2Int selectedResolution = currentResolutions[resolutionIndex];

        AudioListener.volume = volume;
        QualitySettings.SetQualityLevel(quality);
        Screen.SetResolution(selectedResolution.x, selectedResolution.y, fullscreen);

        PlayerPrefs.SetFloat("Volume", volume);
        PlayerPrefs.SetFloat("MouseSensitivity", sensitivity);
        PlayerPrefs.SetInt("Fullscreen", fullscreen ? 1 : 0);
        PlayerPrefs.SetInt("Quality", quality);

        PlayerPrefs.SetInt("AspectRatioIndex", aspectIndex);
        PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
        PlayerPrefs.SetInt("ResolutionWidth", selectedResolution.x);
        PlayerPrefs.SetInt("ResolutionHeight", selectedResolution.y);

        PlayerPrefs.Save();

        Debug.Log("Settings applied: " + selectedResolution.x + "x" + selectedResolution.y);
    }

    public void ResetSettings()
    {
        PlayerPrefs.DeleteKey("Volume");
        PlayerPrefs.DeleteKey("MouseSensitivity");
        PlayerPrefs.DeleteKey("Fullscreen");
        PlayerPrefs.DeleteKey("Quality");
        PlayerPrefs.DeleteKey("AspectRatioIndex");
        PlayerPrefs.DeleteKey("ResolutionIndex");
        PlayerPrefs.DeleteKey("ResolutionWidth");
        PlayerPrefs.DeleteKey("ResolutionHeight");

        PlayerPrefs.Save();

        LoadSettings();
        RefreshResolutionDropdown();
        ApplySettings();

        Debug.Log("Settings reset.");
    }

    private void UpdateTexts()
    {
        if (volumeValueText != null && volumeSlider != null)
            volumeValueText.text = Mathf.RoundToInt(volumeSlider.value * 100f) + "%";

        if (sensitivityValueText != null && sensitivitySlider != null)
            sensitivityValueText.text = sensitivitySlider.value.ToString("0.0");
    }

    public static Color GetCrosshairColor(int index)
    {
        switch (index)
        {
            case 1:
                return Color.red;
            case 2:
                return Color.green;
            case 3:
                return Color.blue;
            case 4:
                return Color.yellow;
            case 5:
                return Color.cyan;
            case 6:
                return Color.magenta;
            case 7:
                return Color.black;
            default:
                return Color.white;
        }
    }
}