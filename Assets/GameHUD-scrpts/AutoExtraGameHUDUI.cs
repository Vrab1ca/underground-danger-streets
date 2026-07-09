using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AutoExtraGameHUDUI : MonoBehaviour
{
    public static AutoExtraGameHUDUI Instance { get; private set; }

    public enum VehicleHUDMode
    {
        None,
        Car,
        Helicopter
    }

    [Header("Canvas")]
    public Canvas targetCanvas;
    public Vector2 referenceResolution = new Vector2(1920, 1080);

    [Header("Stamina")]
    public bool showStamina = true;
    public float currentStamina = 100f;
    public float maxStamina = 100f;

    [Header("Vehicle")]
    public VehicleHUDMode vehicleMode = VehicleHUDMode.None;

    public float currentCarFuel = 100f;
    public float maxCarFuel = 100f;

    public float currentHelicopterFuel = 100f;
    public float maxHelicopterFuel = 100f;

    public int currentHelicopterBombs = 0;
    public int maxHelicopterBombs = 0;

    [Header("Colors")]
    public Color panelColor = new Color(0.02f, 0.025f, 0.035f, 0.82f);
    public Color panelShadowColor = new Color(0f, 0f, 0f, 0.55f);

    public Color staminaColor = new Color(1f, 0.85f, 0.20f, 1f);
    public Color lowStaminaColor = new Color(1f, 0.35f, 0.05f, 1f);

    public Color fuelColor = new Color(0.15f, 0.95f, 0.55f, 1f);
    public Color lowFuelColor = new Color(1f, 0.18f, 0.10f, 1f);

    public Color barBackgroundColor = new Color(0f, 0f, 0f, 0.60f);
    public Color textColor = Color.white;

    [Header("Animation")]
    public float barSmoothSpeed = 8f;

    private RectTransform hudRoot;

    private RectTransform staminaPanel;
    private Image staminaFill;
    private TMP_Text staminaValueText;

    private RectTransform vehiclePanel;
    private Image vehicleFuelFill;
    private TMP_Text vehicleTitleText;
    private TMP_Text vehicleFuelText;
    private TMP_Text helicopterBombsText;
    private TMP_Text vehicleHintText;

    private float smoothStaminaFill = 1f;
    private float smoothVehicleFuelFill = 1f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetupCanvas();
        BuildHUD();
    }

    private void Update()
    {
        UpdateStaminaUI();
        UpdateVehicleUI();
    }

    private void SetupCanvas()
    {
        if (targetCanvas == null)
            targetCanvas = GetComponentInParent<Canvas>();

        if (targetCanvas == null)
            targetCanvas = FindFirstObjectByType<Canvas>();

        if (targetCanvas == null)
        {
            GameObject canvasObject = new GameObject("Canvas");
            targetCanvas = canvasObject.AddComponent<Canvas>();
            targetCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }

        targetCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = targetCanvas.GetComponent<CanvasScaler>();

        if (scaler == null)
            scaler = targetCanvas.gameObject.AddComponent<CanvasScaler>();

        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = referenceResolution;
        scaler.matchWidthOrHeight = 0.5f;

        GraphicRaycaster raycaster = targetCanvas.GetComponent<GraphicRaycaster>();

        if (raycaster == null)
            targetCanvas.gameObject.AddComponent<GraphicRaycaster>();
    }

    private void BuildHUD()
    {
        Transform oldRoot = targetCanvas.transform.Find("AutoExtraHUDRoot");

        if (oldRoot != null)
            Destroy(oldRoot.gameObject);

        hudRoot = CreateRect("AutoExtraHUDRoot", targetCanvas.transform);
        SetRectFullScreen(hudRoot);

        BuildStaminaPanel();
        BuildVehiclePanel();
    }

    private void BuildStaminaPanel()
    {
        staminaPanel = CreatePanel(
            "StaminaPanel",
            hudRoot,
            new Vector2(0f, 1f),
            new Vector2(0f, 1f),
            new Vector2(0f, 1f),
            new Vector2(30f, -210f),
            new Vector2(390f, 78f),
            panelColor
        );

        CreateText(
            "StaminaLabel",
            staminaPanel,
            "STAMINA",
            new Vector2(20f, -15f),
            new Vector2(160f, 25f),
            18,
            TextAlignmentOptions.Left,
            textColor,
            FontStyles.Bold
        );

        staminaValueText = CreateText(
            "StaminaValueText",
            staminaPanel,
            "100 / 100",
            new Vector2(230f, -15f),
            new Vector2(140f, 25f),
            20,
            TextAlignmentOptions.Right,
            textColor,
            FontStyles.Bold
        );

        RectTransform staminaBg = CreateImageRect(
            "StaminaBarBackground",
            staminaPanel,
            new Vector2(20f, -48f),
            new Vector2(350f, 18f),
            barBackgroundColor
        );

        staminaFill = CreateFillImage(
            "StaminaBarFill",
            staminaBg,
            staminaColor
        );
    }

    private void BuildVehiclePanel()
    {
        vehiclePanel = CreatePanel(
            "VehiclePanel",
            hudRoot,
            new Vector2(1f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 1f),
            new Vector2(-40f, -30f),
            new Vector2(370f, 165f),
            panelColor
        );

        vehicleTitleText = CreateText(
            "VehicleTitleText",
            vehiclePanel,
            "VEHICLE",
            new Vector2(-350f, -18f),
            new Vector2(330f, 30f),
            24,
            TextAlignmentOptions.Right,
            textColor,
            FontStyles.Bold
        );

        vehicleFuelText = CreateText(
            "VehicleFuelText",
            vehiclePanel,
            "FUEL: 100 / 100",
            new Vector2(-350f, -55f),
            new Vector2(330f, 28f),
            20,
            TextAlignmentOptions.Right,
            textColor,
            FontStyles.Bold
        );

        RectTransform fuelBg = CreateImageRectRight(
            "VehicleFuelBarBackground",
            vehiclePanel,
            new Vector2(-350f, -90f),
            new Vector2(330f, 18f),
            barBackgroundColor
        );

        vehicleFuelFill = CreateFillImage(
            "VehicleFuelBarFill",
            fuelBg,
            fuelColor
        );

        helicopterBombsText = CreateText(
            "HelicopterBombsText",
            vehiclePanel,
            "BOMBS: 0 / 0",
            new Vector2(-350f, -115f),
            new Vector2(330f, 26f),
            20,
            TextAlignmentOptions.Right,
            textColor,
            FontStyles.Bold
        );

        vehicleHintText = CreateText(
            "VehicleHintText",
            vehiclePanel,
            "",
            new Vector2(-350f, -140f),
            new Vector2(330f, 22f),
            16,
            TextAlignmentOptions.Right,
            new Color(0.85f, 0.85f, 0.85f, 1f),
            FontStyles.Bold
        );

        vehiclePanel.gameObject.SetActive(false);
    }

    public void SetStamina(float current, float max)
    {
        currentStamina = Mathf.Max(0f, current);
        maxStamina = Mathf.Max(1f, max);
    }

    public void ShowCarFuel(float currentFuel, float maxFuel)
    {
        vehicleMode = VehicleHUDMode.Car;

        currentCarFuel = Mathf.Max(0f, currentFuel);
        maxCarFuel = Mathf.Max(1f, maxFuel);
    }

    public void ShowHelicopterHUD(float currentFuel, float maxFuel, int bombs, int maxBombs)
    {
        vehicleMode = VehicleHUDMode.Helicopter;

        currentHelicopterFuel = Mathf.Max(0f, currentFuel);
        maxHelicopterFuel = Mathf.Max(1f, maxFuel);

        currentHelicopterBombs = Mathf.Max(0, bombs);
        maxHelicopterBombs = Mathf.Max(0, maxBombs);
    }

    public void HideVehicleHUD()
    {
        vehicleMode = VehicleHUDMode.None;
    }

    private void UpdateStaminaUI()
    {
        if (staminaPanel == null)
            return;

        staminaPanel.gameObject.SetActive(showStamina);

        if (!showStamina)
            return;

        float targetFill = currentStamina / maxStamina;
        targetFill = Mathf.Clamp01(targetFill);

        smoothStaminaFill = Mathf.Lerp(
            smoothStaminaFill,
            targetFill,
            Time.deltaTime * barSmoothSpeed
        );

        if (staminaFill != null)
        {
            staminaFill.fillAmount = smoothStaminaFill;

            if (currentStamina <= maxStamina * 0.25f)
                staminaFill.color = lowStaminaColor;
            else
                staminaFill.color = staminaColor;
        }

        if (staminaValueText != null)
        {
            staminaValueText.text =
                Mathf.CeilToInt(currentStamina) +
                " / " +
                Mathf.CeilToInt(maxStamina);
        }
    }

    private void UpdateVehicleUI()
    {
        if (vehiclePanel == null)
            return;

        bool showVehicle = vehicleMode != VehicleHUDMode.None;

        vehiclePanel.gameObject.SetActive(showVehicle);

        if (!showVehicle)
            return;

        if (vehicleMode == VehicleHUDMode.Car)
            UpdateCarUI();

        if (vehicleMode == VehicleHUDMode.Helicopter)
            UpdateHelicopterUI();
    }

    private void UpdateCarUI()
    {
        float targetFill = currentCarFuel / maxCarFuel;
        targetFill = Mathf.Clamp01(targetFill);

        smoothVehicleFuelFill = Mathf.Lerp(
            smoothVehicleFuelFill,
            targetFill,
            Time.deltaTime * barSmoothSpeed
        );

        if (vehicleTitleText != null)
            vehicleTitleText.text = "CAR";

        if (vehicleFuelText != null)
        {
            vehicleFuelText.text =
                "FUEL: " +
                Mathf.CeilToInt(currentCarFuel) +
                " / " +
                Mathf.CeilToInt(maxCarFuel);
        }

        if (vehicleFuelFill != null)
        {
            vehicleFuelFill.fillAmount = smoothVehicleFuelFill;

            if (currentCarFuel <= maxCarFuel * 0.20f)
                vehicleFuelFill.color = lowFuelColor;
            else
                vehicleFuelFill.color = fuelColor;
        }

        if (helicopterBombsText != null)
            helicopterBombsText.gameObject.SetActive(false);

        if (vehicleHintText != null)
            vehicleHintText.text = "Vehicle active";
    }

    private void UpdateHelicopterUI()
    {
        float targetFill = currentHelicopterFuel / maxHelicopterFuel;
        targetFill = Mathf.Clamp01(targetFill);

        smoothVehicleFuelFill = Mathf.Lerp(
            smoothVehicleFuelFill,
            targetFill,
            Time.deltaTime * barSmoothSpeed
        );

        if (vehicleTitleText != null)
            vehicleTitleText.text = "HELICOPTER";

        if (vehicleFuelText != null)
        {
            vehicleFuelText.text =
                "FUEL: " +
                Mathf.CeilToInt(currentHelicopterFuel) +
                " / " +
                Mathf.CeilToInt(maxHelicopterFuel);
        }

        if (vehicleFuelFill != null)
        {
            vehicleFuelFill.fillAmount = smoothVehicleFuelFill;

            if (currentHelicopterFuel <= maxHelicopterFuel * 0.20f)
                vehicleFuelFill.color = lowFuelColor;
            else
                vehicleFuelFill.color = fuelColor;
        }

        if (helicopterBombsText != null)
        {
            helicopterBombsText.gameObject.SetActive(true);

            helicopterBombsText.text =
                "BOMBS: " +
                currentHelicopterBombs +
                " / " +
                maxHelicopterBombs;
        }

        if (vehicleHintText != null)
            vehicleHintText.text = "Air support ready";
    }

    private RectTransform CreateRect(string name, Transform parent)
    {
        GameObject obj = new GameObject(name);
        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.SetParent(parent, false);
        return rect;
    }

    private RectTransform CreatePanel(
        string name,
        Transform parent,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 pivot,
        Vector2 anchoredPosition,
        Vector2 size,
        Color color
    )
    {
        RectTransform rect = CreateRect(name, parent);

        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = pivot;
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = size;

        Image image = rect.gameObject.AddComponent<Image>();
        image.color = color;
        image.raycastTarget = false;

        Shadow shadow = rect.gameObject.AddComponent<Shadow>();
        shadow.effectColor = panelShadowColor;
        shadow.effectDistance = new Vector2(4f, -4f);

        return rect;
    }

    private RectTransform CreateImageRect(
        string name,
        Transform parent,
        Vector2 anchoredPosition,
        Vector2 size,
        Color color
    )
    {
        RectTransform rect = CreateRect(name, parent);

        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = size;

        Image image = rect.gameObject.AddComponent<Image>();
        image.color = color;
        image.raycastTarget = false;

        return rect;
    }

    private RectTransform CreateImageRectRight(
        string name,
        Transform parent,
        Vector2 anchoredPosition,
        Vector2 size,
        Color color
    )
    {
        RectTransform rect = CreateRect(name, parent);

        rect.anchorMin = new Vector2(1f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(1f, 1f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = size;

        Image image = rect.gameObject.AddComponent<Image>();
        image.color = color;
        image.raycastTarget = false;

        return rect;
    }

    private Image CreateFillImage(string name, RectTransform parent, Color color)
    {
        RectTransform rect = CreateRect(name, parent);

        rect.anchorMin = new Vector2(0f, 0f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = Vector2.zero;

        Image image = rect.gameObject.AddComponent<Image>();
        image.color = color;
        image.type = Image.Type.Filled;
        image.fillMethod = Image.FillMethod.Horizontal;
        image.fillOrigin = 0;
        image.fillAmount = 1f;
        image.raycastTarget = false;

        return image;
    }

    private TMP_Text CreateText(
        string name,
        Transform parent,
        string text,
        Vector2 anchoredPosition,
        Vector2 size,
        int fontSize,
        TextAlignmentOptions alignment,
        Color color,
        FontStyles fontStyle
    )
    {
        RectTransform rect = CreateRect(name, parent);

        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = size;

        TextMeshProUGUI tmp = rect.gameObject.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = alignment;
        tmp.color = color;
        tmp.fontStyle = fontStyle;
        tmp.enableWordWrapping = false;
        tmp.raycastTarget = false;

        Outline outline = rect.gameObject.AddComponent<Outline>();
        outline.effectColor = new Color(0f, 0f, 0f, 0.65f);
        outline.effectDistance = new Vector2(1.2f, -1.2f);

        return tmp;
    }

    private void SetRectFullScreen(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = Vector2.zero;
    }
}