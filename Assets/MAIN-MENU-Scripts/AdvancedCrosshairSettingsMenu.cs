using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdvancedCrosshairSettingsMenu : MonoBehaviour
{
    [Header("Preview")]
    public AdvancedCrosshair previewCrosshair;

    [Header("Toggles")]
    public Toggle crosshairToggle;
    public Toggle centerDotToggle;
    public Toggle outlineToggle;

    [Header("Dropdowns")]
    public TMP_Dropdown styleDropdown;
    public TMP_Dropdown colorDropdown;

    [Header("Sliders")]
    public Slider lengthSlider;
    public Slider thicknessSlider;
    public Slider gapSlider;
    public Slider dotSizeSlider;
    public Slider opacitySlider;
    public Slider outlineThicknessSlider;

    [Header("Value Texts")]
    public TMP_Text lengthValueText;
    public TMP_Text thicknessValueText;
    public TMP_Text gapValueText;
    public TMP_Text dotSizeValueText;
    public TMP_Text opacityValueText;
    public TMP_Text outlineThicknessValueText;

    private void Start()
    {
        SetupDropdowns();
        SetupSliders();
        LoadSettingsToUI();
        UpdatePreview();
    }

    private void Update()
    {
        UpdatePreview();
        UpdateTexts();
    }

    private void SetupDropdowns()
    {
        if (styleDropdown != null)
        {
            styleDropdown.ClearOptions();

            styleDropdown.AddOptions(new List<string>
            {
                "Classic",
                "Dot Only",
                "T Shape",
                "Cross"
            });
        }

        if (colorDropdown != null)
        {
            colorDropdown.ClearOptions();

            colorDropdown.AddOptions(new List<string>
            {
                "White",
                "Red",
                "Green",
                "Blue",
                "Yellow",
                "Cyan",
                "Magenta",
                "Black"
            });
        }
    }

    private void SetupSliders()
    {
        SetSlider(lengthSlider, 4f, 40f, 18f);
        SetSlider(thicknessSlider, 1f, 12f, 4f);
        SetSlider(gapSlider, 0f, 30f, 8f);
        SetSlider(dotSizeSlider, 2f, 20f, 5f);
        SetSlider(opacitySlider, 0.1f, 1f, 1f);
        SetSlider(outlineThicknessSlider, 1f, 6f, 1f);
    }

    private void SetSlider(Slider slider, float min, float max, float value)
    {
        if (slider == null)
            return;

        slider.minValue = min;
        slider.maxValue = max;
        slider.value = value;
        slider.wholeNumbers = false;
    }

    private void LoadSettingsToUI()
    {
        if (crosshairToggle != null)
            crosshairToggle.isOn = PlayerPrefs.GetInt("Crosshair_Enabled", 1) == 1;

        if (styleDropdown != null)
            styleDropdown.value = PlayerPrefs.GetInt("Crosshair_Style", 0);

        if (colorDropdown != null)
            colorDropdown.value = PlayerPrefs.GetInt("Crosshair_Color", 0);

        if (lengthSlider != null)
            lengthSlider.value = PlayerPrefs.GetFloat("Crosshair_Length", 18f);

        if (thicknessSlider != null)
            thicknessSlider.value = PlayerPrefs.GetFloat("Crosshair_Thickness", 4f);

        if (gapSlider != null)
            gapSlider.value = PlayerPrefs.GetFloat("Crosshair_Gap", 8f);

        if (centerDotToggle != null)
            centerDotToggle.isOn = PlayerPrefs.GetInt("Crosshair_Dot", 0) == 1;

        if (dotSizeSlider != null)
            dotSizeSlider.value = PlayerPrefs.GetFloat("Crosshair_DotSize", 5f);

        if (opacitySlider != null)
            opacitySlider.value = PlayerPrefs.GetFloat("Crosshair_Opacity", 1f);

        if (outlineToggle != null)
            outlineToggle.isOn = PlayerPrefs.GetInt("Crosshair_Outline", 0) == 1;

        if (outlineThicknessSlider != null)
            outlineThicknessSlider.value = PlayerPrefs.GetFloat("Crosshair_OutlineThickness", 1f);
    }

    public void ApplySettings()
    {
        PlayerPrefs.SetInt("Crosshair_Enabled", GetToggle(crosshairToggle, true) ? 1 : 0);
        PlayerPrefs.SetInt("Crosshair_Style", GetDropdown(styleDropdown, 0));
        PlayerPrefs.SetInt("Crosshair_Color", GetDropdown(colorDropdown, 0));

        PlayerPrefs.SetFloat("Crosshair_Length", GetSlider(lengthSlider, 18f));
        PlayerPrefs.SetFloat("Crosshair_Thickness", GetSlider(thicknessSlider, 4f));
        PlayerPrefs.SetFloat("Crosshair_Gap", GetSlider(gapSlider, 8f));

        PlayerPrefs.SetInt("Crosshair_Dot", GetToggle(centerDotToggle, false) ? 1 : 0);
        PlayerPrefs.SetFloat("Crosshair_DotSize", GetSlider(dotSizeSlider, 5f));

        PlayerPrefs.SetFloat("Crosshair_Opacity", GetSlider(opacitySlider, 1f));

        PlayerPrefs.SetInt("Crosshair_Outline", GetToggle(outlineToggle, false) ? 1 : 0);
        PlayerPrefs.SetFloat("Crosshair_OutlineThickness", GetSlider(outlineThicknessSlider, 1f));

        PlayerPrefs.Save();

        UpdatePreview();

        Debug.Log("Advanced crosshair settings saved.");
    }

    public void ResetCrosshair()
    {
        PlayerPrefs.DeleteKey("Crosshair_Enabled");
        PlayerPrefs.DeleteKey("Crosshair_Style");
        PlayerPrefs.DeleteKey("Crosshair_Color");
        PlayerPrefs.DeleteKey("Crosshair_Length");
        PlayerPrefs.DeleteKey("Crosshair_Thickness");
        PlayerPrefs.DeleteKey("Crosshair_Gap");
        PlayerPrefs.DeleteKey("Crosshair_Dot");
        PlayerPrefs.DeleteKey("Crosshair_DotSize");
        PlayerPrefs.DeleteKey("Crosshair_Opacity");
        PlayerPrefs.DeleteKey("Crosshair_Outline");
        PlayerPrefs.DeleteKey("Crosshair_OutlineThickness");

        PlayerPrefs.Save();

        LoadSettingsToUI();
        UpdatePreview();

        Debug.Log("Advanced crosshair reset.");
    }

    private void UpdatePreview()
    {
        if (previewCrosshair == null)
            return;

        previewCrosshair.SetCrosshair(
            GetToggle(crosshairToggle, true),
            GetDropdown(styleDropdown, 0),
            GetDropdown(colorDropdown, 0),
            GetSlider(lengthSlider, 18f),
            GetSlider(thicknessSlider, 4f),
            GetSlider(gapSlider, 8f),
            GetToggle(centerDotToggle, false),
            GetSlider(dotSizeSlider, 5f),
            GetSlider(opacitySlider, 1f),
            GetToggle(outlineToggle, false),
            GetSlider(outlineThicknessSlider, 1f)
        );
    }

    private void UpdateTexts()
    {
        SetText(lengthValueText, lengthSlider, "0");
        SetText(thicknessValueText, thicknessSlider, "0.0");
        SetText(gapValueText, gapSlider, "0");
        SetText(dotSizeValueText, dotSizeSlider, "0");
        SetText(opacityValueText, opacitySlider, "0.0");
        SetText(outlineThicknessValueText, outlineThicknessSlider, "0.0");
    }

    private void SetText(TMP_Text text, Slider slider, string format)
    {
        if (text == null || slider == null)
            return;

        text.text = slider.value.ToString(format);
    }

    private bool GetToggle(Toggle toggle, bool defaultValue)
    {
        if (toggle == null)
            return defaultValue;

        return toggle.isOn;
    }

    private int GetDropdown(TMP_Dropdown dropdown, int defaultValue)
    {
        if (dropdown == null)
            return defaultValue;

        return dropdown.value;
    }

    private float GetSlider(Slider slider, float defaultValue)
    {
        if (slider == null)
            return defaultValue;

        return slider.value;
    }
}