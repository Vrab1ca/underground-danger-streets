using UnityEngine;

public class GameSettingsApplier : MonoBehaviour
{
    private void Start()
    {
        ApplySettings();
    }

    public static void ApplySettings()
    {
        float volume = PlayerPrefs.GetFloat("Volume", 1f);
        bool fullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        int quality = PlayerPrefs.GetInt("Quality", QualitySettings.GetQualityLevel());

        int width = PlayerPrefs.GetInt("ResolutionWidth", Screen.currentResolution.width);
        int height = PlayerPrefs.GetInt("ResolutionHeight", Screen.currentResolution.height);

        AudioListener.volume = volume;
        QualitySettings.SetQualityLevel(quality);
        Screen.SetResolution(width, height, fullscreen);

        Debug.Log("Applied game settings: " + width + "x" + height);
    }
}