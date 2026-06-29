using TMPro;
using UnityEngine;

public class CrosshairSettingsApplier : MonoBehaviour
{
    public TextMeshProUGUI crosshairText;

    private void Update()
    {
        ApplyCrosshair();
    }

    private void ApplyCrosshair()
    {
        if (crosshairText == null)
            return;

        bool enabled = PlayerPrefs.GetInt("CrosshairEnabled", 1) == 1;
        float size = PlayerPrefs.GetFloat("CrosshairSize", 36f);
        int colorIndex = PlayerPrefs.GetInt("CrosshairColor", 0);

        crosshairText.gameObject.SetActive(enabled);
        crosshairText.fontSize = size;
        crosshairText.color = SettingsMenu.GetCrosshairColor(colorIndex);
        crosshairText.text = "+";
    }
}