using UnityEngine;
using UnityEngine.UI;

public class AdvancedCrosshair : MonoBehaviour
{
    public enum CrosshairStyle
    {
        Classic,
        DotOnly,
        TShape,
        Cross
    }

    [Header("Crosshair Parts")]
    public Image topLine;
    public Image bottomLine;
    public Image leftLine;
    public Image rightLine;
    public Image centerDot;

    [Header("Settings")]
    public bool crosshairEnabled = true;
    public CrosshairStyle style = CrosshairStyle.Classic;

    public Color crosshairColor = Color.white;
    public float opacity = 1f;

    public float length = 18f;
    public float thickness = 4f;
    public float gap = 8f;

    public bool showCenterDot = false;
    public float dotSize = 5f;

    public bool showOutline = false;
    public float outlineThickness = 1f;
    public Color outlineColor = Color.black;

    private void Start()
    {
        ApplyFromPlayerPrefs();
    }

    public void ApplyFromPlayerPrefs()
    {
        crosshairEnabled = PlayerPrefs.GetInt("Crosshair_Enabled", 1) == 1;
        style = (CrosshairStyle)PlayerPrefs.GetInt("Crosshair_Style", 0);

        int colorIndex = PlayerPrefs.GetInt("Crosshair_Color", 0);
        crosshairColor = GetColor(colorIndex);

        length = PlayerPrefs.GetFloat("Crosshair_Length", 18f);
        thickness = PlayerPrefs.GetFloat("Crosshair_Thickness", 4f);
        gap = PlayerPrefs.GetFloat("Crosshair_Gap", 8f);

        showCenterDot = PlayerPrefs.GetInt("Crosshair_Dot", 0) == 1;
        dotSize = PlayerPrefs.GetFloat("Crosshair_DotSize", 5f);

        opacity = PlayerPrefs.GetFloat("Crosshair_Opacity", 1f);

        showOutline = PlayerPrefs.GetInt("Crosshair_Outline", 0) == 1;
        outlineThickness = PlayerPrefs.GetFloat("Crosshair_OutlineThickness", 1f);

        ApplyCrosshair();
    }

    public void SetCrosshair(
        bool enabled,
        int styleIndex,
        int colorIndex,
        float newLength,
        float newThickness,
        float newGap,
        bool dot,
        float newDotSize,
        float newOpacity,
        bool outline,
        float newOutlineThickness
    )
    {
        crosshairEnabled = enabled;
        style = (CrosshairStyle)styleIndex;
        crosshairColor = GetColor(colorIndex);

        length = newLength;
        thickness = newThickness;
        gap = newGap;

        showCenterDot = dot;
        dotSize = newDotSize;

        opacity = newOpacity;

        showOutline = outline;
        outlineThickness = newOutlineThickness;

        ApplyCrosshair();
    }

    public void ApplyCrosshair()
    {
        gameObject.SetActive(crosshairEnabled);

        Color finalColor = crosshairColor;
        finalColor.a = opacity;

        float realGap = gap;

        if (style == CrosshairStyle.Cross)
            realGap = 0f;

        bool useTop = style == CrosshairStyle.Classic || style == CrosshairStyle.Cross;
        bool useBottom = style == CrosshairStyle.Classic || style == CrosshairStyle.TShape || style == CrosshairStyle.Cross;
        bool useLeft = style == CrosshairStyle.Classic || style == CrosshairStyle.TShape || style == CrosshairStyle.Cross;
        bool useRight = style == CrosshairStyle.Classic || style == CrosshairStyle.TShape || style == CrosshairStyle.Cross;

        bool dotActive = showCenterDot || style == CrosshairStyle.DotOnly;

        SetLine(topLine, useTop, finalColor, new Vector2(thickness, length), new Vector2(0f, realGap + length / 2f));
        SetLine(bottomLine, useBottom, finalColor, new Vector2(thickness, length), new Vector2(0f, -realGap - length / 2f));
        SetLine(leftLine, useLeft, finalColor, new Vector2(length, thickness), new Vector2(-realGap - length / 2f, 0f));
        SetLine(rightLine, useRight, finalColor, new Vector2(length, thickness), new Vector2(realGap + length / 2f, 0f));

        SetLine(centerDot, dotActive, finalColor, new Vector2(dotSize, dotSize), Vector2.zero);
    }

    private void SetLine(Image image, bool active, Color color, Vector2 size, Vector2 position)
    {
        if (image == null)
            return;

        image.gameObject.SetActive(active);

        image.color = color;

        RectTransform rect = image.GetComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.anchoredPosition = position;

        Outline outline = image.GetComponent<Outline>();

        if (showOutline)
        {
            if (outline == null)
                outline = image.gameObject.AddComponent<Outline>();

            outline.effectColor = outlineColor;
            outline.effectDistance = new Vector2(outlineThickness, -outlineThickness);
            outline.enabled = true;
        }
        else
        {
            if (outline != null)
                outline.enabled = false;
        }
    }

    public static Color GetColor(int index)
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