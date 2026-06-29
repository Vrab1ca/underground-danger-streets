using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class CrosshairPreviewLive : MonoBehaviour
{
    [Header("UI References")]
    public Toggle crosshairToggle;
    public Slider crosshairSizeSlider;
    public TMP_Dropdown crosshairColorDropdown;
    public TextMeshProUGUI previewText;

    [Header("Preview Settings")]
    public float defaultSize = 36f;
    public float minSize = 20f;
    public float maxSize = 80f;

    private void Update()
    {
        UpdatePreview();
    }

    private void OnValidate()
    {
        UpdatePreview();
    }

    private void UpdatePreview()
    {
        if (previewText == null)
            return;

        bool show = true;

        if (crosshairToggle != null)
            show = crosshairToggle.isOn;

        previewText.gameObject.SetActive(show);

        previewText.text = "+";
        previewText.alignment = TextAlignmentOptions.Center;
        previewText.raycastTarget = false;

        float size = defaultSize;

        if (crosshairSizeSlider != null)
        {
            // If slider is wrongly 0-1, convert it to 20-80
            if (crosshairSizeSlider.maxValue <= 1.1f)
            {
                size = Mathf.Lerp(minSize, maxSize, crosshairSizeSlider.value);
            }
            else
            {
                size = crosshairSizeSlider.value;
            }
        }

        size = Mathf.Clamp(size, minSize, maxSize);
        previewText.fontSize = size;

        if (crosshairColorDropdown != null)
            previewText.color = GetColor(crosshairColorDropdown.value);
        else
            previewText.color = Color.white;

        previewText.transform.SetAsLastSibling();
    }

    private Color GetColor(int index)
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
                return Color.black;
            default:
                return Color.white;
        }
    }
}