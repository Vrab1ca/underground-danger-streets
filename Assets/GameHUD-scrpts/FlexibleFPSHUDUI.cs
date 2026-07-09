using TMPro;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class FlexibleFPSHUDUI : MonoBehaviour
{
    public static FlexibleFPSHUDUI Instance { get; private set; }

    public enum VehicleMode
    {
        None,
        Car,
        Helicopter
    }

    private class SlotUI
    {
        public RectTransform root;
        public Image background;
        public TMP_Text keyText;
        public TMP_Text titleText;
        public TMP_Text countText;
    }

    [Header("Layout Mode")]
    public bool buildMissingUIOnStart = true;

    [Tooltip("OFF = you can move panels manually in Canvas and script will not move them on Play.")]
    public bool applyInspectorLayoutOnStart = false;

    [Header("Canvas")]
    public Canvas targetCanvas;
    public Vector2 referenceResolution = new Vector2(1920, 1080);

    [Header("Default Panel Positions")]
    public Vector2 statusPanelPosition = new Vector2(30f, -30f);
    public Vector2 staminaPanelPosition = new Vector2(30f, -210f);
    public Vector2 vehiclePanelPosition = new Vector2(-40f, -30f);
    public Vector2 weaponPanelPosition = new Vector2(-40f, 40f);
    public Vector2 hotbarPanelPosition = new Vector2(0f, 28f);
    public Vector2 interactionTextPosition = new Vector2(0f, -190f);

    [Header("Default Panel Sizes")]
    public Vector2 statusPanelSize = new Vector2(390f, 170f);
    public Vector2 staminaPanelSize = new Vector2(390f, 78f);
    public Vector2 vehiclePanelSize = new Vector2(370f, 165f);
    public Vector2 weaponPanelSize = new Vector2(310f, 125f);
    public Vector2 hotbarPanelSize = new Vector2(880f, 92f);

    [Header("Player References")]
    public PlayerHealth playerHealth;
    public PlayerArmor playerArmor;
    public WeaponSwitcher weaponSwitcher;
    public PlayerGrenadeInventory grenadeInventory;
    public JumpPlatformInventory jumpPlatformInventory;
    public PlayerHealthInventory healthInventory;
    public PlayerArmorInventory armorInventory;

    [Header("Stamina")]
    public bool showStamina = true;
    public float currentStamina = 100f;
    public float maxStamina = 100f;

    [Header("Vehicle")]
    public bool showVehiclePanel;
    public VehicleMode currentVehicleMode = VehicleMode.None;

    public string vehicleName = "CAR";
    public float currentFuel = 100f;
    public float maxFuel = 100f;

    public bool showBombs;
    public int currentBombs;
    public int maxBombs;

    [Header("Vehicle Anti Glitch")]
    public float vehicleAutoHideDelay = 0.35f;
    private float lastVehicleHUDUpdateTime;

    [Header("Colors")]
    public Color panelColor = new Color(0.02f, 0.025f, 0.035f, 0.82f);
    public Color barBackgroundColor = new Color(0f, 0f, 0f, 0.60f);

    public Color healthColor = new Color(0.95f, 0.12f, 0.14f, 1f);
    public Color lowHealthColor = new Color(1f, 0.03f, 0.03f, 1f);
    public Color armorColor = new Color(0.15f, 0.55f, 1f, 1f);
    public Color staminaColor = new Color(1f, 0.85f, 0.20f, 1f);
    public Color lowStaminaColor = new Color(1f, 0.35f, 0.05f, 1f);
    public Color fuelColor = new Color(0.15f, 0.95f, 0.55f, 1f);
    public Color lowFuelColor = new Color(1f, 0.18f, 0.10f, 1f);

    public Color normalSlotColor = new Color(0.08f, 0.10f, 0.14f, 0.88f);
    public Color selectedSlotColor = new Color(1f, 0.72f, 0.18f, 0.98f);
    public Color emptySlotColor = new Color(0.03f, 0.03f, 0.04f, 0.60f);

    public Color textColor = Color.white;
    public Color selectedTextColor = Color.black;
    public Color emptyTextColor = new Color(0.65f, 0.65f, 0.65f, 1f);

    [Header("Animation")]
    public float barSmoothSpeed = 8f;

    private RectTransform hudRoot;

    private RectTransform statusPanel;
    private RectTransform staminaPanel;
    private RectTransform vehiclePanel;
    private RectTransform weaponPanel;
    private RectTransform hotbarPanel;

    private Image healthFill;
    private Image armorFill;
    private Image staminaFill;
    private Image fuelFill;

    private TMP_Text healthValueText;
    private TMP_Text armorValueText;
    private TMP_Text armorTypeText;
    private TMP_Text staminaValueText;

    private TMP_Text vehicleTitleText;
    private TMP_Text vehicleFuelText;
    private TMP_Text vehicleBombsText;

    private TMP_Text weaponNameText;
    private TMP_Text ammoText;
    private TMP_Text interactionText;

    private SlotUI[] slots = new SlotUI[7];

    private float smoothHealthFill = 1f;
    private float smoothArmorFill = 0f;
    private float smoothStaminaFill = 1f;
    private float smoothFuelFill = 1f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetupCanvas();
        AutoFindReferences();

        if (buildMissingUIOnStart)
            CreateOrRefreshHUDLayout(applyInspectorLayoutOnStart);

        CacheUIReferences();
    }

    private void Update()
    {
        AutoFindReferences();

        UpdateHealthUI();
        UpdateArmorUI();
        UpdateStaminaUI();
        UpdateVehicleUI();
        UpdateWeaponUI();
        UpdateHotbarUI();
    }

    [ContextMenu("Create Missing HUD Layout")]
    public void ContextCreateMissingHUDLayout()
    {
        SetupCanvas();
        CreateOrRefreshHUDLayout(false);
        CacheUIReferences();
        MarkSceneDirty();
    }

    [ContextMenu("Apply Inspector Layout Positions")]
    public void ContextApplyInspectorLayout()
    {
        SetupCanvas();
        CreateOrRefreshHUDLayout(true);
        CacheUIReferences();
        MarkSceneDirty();
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
        }

        targetCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = targetCanvas.GetComponent<CanvasScaler>();

        if (scaler == null)
            scaler = targetCanvas.gameObject.AddComponent<CanvasScaler>();

        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = referenceResolution;
        scaler.matchWidthOrHeight = 0.5f;

        if (targetCanvas.GetComponent<GraphicRaycaster>() == null)
            targetCanvas.gameObject.AddComponent<GraphicRaycaster>();
    }

    private void AutoFindReferences()
    {
        if (playerHealth == null)
            playerHealth = FindFirstObjectByType<PlayerHealth>();

        if (playerArmor == null)
            playerArmor = FindFirstObjectByType<PlayerArmor>();

        if (weaponSwitcher == null)
            weaponSwitcher = FindFirstObjectByType<WeaponSwitcher>();

        if (grenadeInventory == null)
            grenadeInventory = FindFirstObjectByType<PlayerGrenadeInventory>();

        if (jumpPlatformInventory == null)
            jumpPlatformInventory = FindFirstObjectByType<JumpPlatformInventory>();

        if (healthInventory == null)
            healthInventory = FindFirstObjectByType<PlayerHealthInventory>();

        if (armorInventory == null)
            armorInventory = FindFirstObjectByType<PlayerArmorInventory>();
    }

    private void CreateOrRefreshHUDLayout(bool applyLayout)
    {
        hudRoot = GetOrCreateRect("HUD_ROOT_FLEXIBLE", targetCanvas.transform);
        SetFullScreen(hudRoot);

        BuildStatusPanel(applyLayout);
        BuildStaminaPanel(applyLayout);
        BuildVehiclePanel(applyLayout);
        BuildWeaponPanel(applyLayout);
        BuildHotbarPanel(applyLayout);
        BuildInteractionText(applyLayout);
    }

    private void BuildStatusPanel(bool applyLayout)
    {
        statusPanel = GetOrCreatePanel("StatusPanel", hudRoot, panelColor);

        ApplyRect(
            statusPanel,
            new Vector2(0f, 1f),
            new Vector2(0f, 1f),
            new Vector2(0f, 1f),
            statusPanelPosition,
            statusPanelSize,
            applyLayout
        );

        CreateText("HealthLabel", statusPanel, "HEALTH", new Vector2(20f, -18f), new Vector2(120f, 25f), 18, TextAlignmentOptions.Left);
        healthValueText = CreateText("HealthValueText", statusPanel, "100 / 100", new Vector2(250f, -18f), new Vector2(120f, 25f), 20, TextAlignmentOptions.Right);

        RectTransform healthBg = GetOrCreateImage("HealthBarBackground", statusPanel, barBackgroundColor);
        ApplyTopLeftRect(healthBg, new Vector2(20f, -50f), new Vector2(350f, 18f), applyLayout);
        healthFill = GetOrCreateFill("HealthBarFill", healthBg, healthColor);

        CreateText("ArmorLabel", statusPanel, "ARMOR", new Vector2(20f, -82f), new Vector2(120f, 25f), 18, TextAlignmentOptions.Left);
        armorValueText = CreateText("ArmorValueText", statusPanel, "0 / 0", new Vector2(250f, -82f), new Vector2(120f, 25f), 20, TextAlignmentOptions.Right);

        RectTransform armorBg = GetOrCreateImage("ArmorBarBackground", statusPanel, barBackgroundColor);
        ApplyTopLeftRect(armorBg, new Vector2(20f, -114f), new Vector2(350f, 18f), applyLayout);
        armorFill = GetOrCreateFill("ArmorBarFill", armorBg, armorColor);

        armorTypeText = CreateText("ArmorTypeText", statusPanel, "TYPE: None", new Vector2(20f, -140f), new Vector2(350f, 25f), 17, TextAlignmentOptions.Left);
    }

    private void BuildStaminaPanel(bool applyLayout)
    {
        staminaPanel = GetOrCreatePanel("StaminaPanel", hudRoot, panelColor);

        ApplyRect(
            staminaPanel,
            new Vector2(0f, 1f),
            new Vector2(0f, 1f),
            new Vector2(0f, 1f),
            staminaPanelPosition,
            staminaPanelSize,
            applyLayout
        );

        CreateText("StaminaLabel", staminaPanel, "STAMINA", new Vector2(20f, -15f), new Vector2(160f, 25f), 18, TextAlignmentOptions.Left);
        staminaValueText = CreateText("StaminaValueText", staminaPanel, "100 / 100", new Vector2(230f, -15f), new Vector2(140f, 25f), 20, TextAlignmentOptions.Right);

        RectTransform staminaBg = GetOrCreateImage("StaminaBarBackground", staminaPanel, barBackgroundColor);
        ApplyTopLeftRect(staminaBg, new Vector2(20f, -48f), new Vector2(350f, 18f), applyLayout);
        staminaFill = GetOrCreateFill("StaminaBarFill", staminaBg, staminaColor);
    }

    private void BuildVehiclePanel(bool applyLayout)
    {
        vehiclePanel = GetOrCreatePanel("VehiclePanel", hudRoot, panelColor);

        ApplyRect(
            vehiclePanel,
            new Vector2(1f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 1f),
            vehiclePanelPosition,
            vehiclePanelSize,
            applyLayout
        );

        vehicleTitleText = CreateText("VehicleTitleText", vehiclePanel, "VEHICLE", new Vector2(20f, -18f), new Vector2(330f, 30f), 24, TextAlignmentOptions.Right);
        vehicleFuelText = CreateText("VehicleFuelText", vehiclePanel, "FUEL: 100 / 100", new Vector2(20f, -55f), new Vector2(330f, 28f), 20, TextAlignmentOptions.Right);

        RectTransform fuelBg = GetOrCreateImage("VehicleFuelBarBackground", vehiclePanel, barBackgroundColor);
        ApplyTopLeftRect(fuelBg, new Vector2(20f, -90f), new Vector2(330f, 18f), applyLayout);
        fuelFill = GetOrCreateFill("VehicleFuelBarFill", fuelBg, fuelColor);

        vehicleBombsText = CreateText("VehicleBombsText", vehiclePanel, "BOMBS: 0 / 0", new Vector2(20f, -115f), new Vector2(330f, 26f), 20, TextAlignmentOptions.Right);
    }

    private void BuildWeaponPanel(bool applyLayout)
    {
        weaponPanel = GetOrCreatePanel("WeaponPanel", hudRoot, panelColor);

        ApplyRect(
            weaponPanel,
            new Vector2(1f, 0f),
            new Vector2(1f, 0f),
            new Vector2(1f, 0f),
            weaponPanelPosition,
            weaponPanelSize,
            applyLayout
        );

        weaponNameText = CreateText("WeaponNameText", weaponPanel, "", new Vector2(20f, -18f), new Vector2(260f, 35f), 24, TextAlignmentOptions.Right);
        ammoText = CreateText("AmmoText", weaponPanel, "", new Vector2(20f, -58f), new Vector2(260f, 55f), 42, TextAlignmentOptions.Right);
    }

    private void BuildHotbarPanel(bool applyLayout)
    {
        hotbarPanel = GetOrCreatePanel("HotbarPanel", hudRoot, panelColor);

        ApplyRect(
            hotbarPanel,
            new Vector2(0.5f, 0f),
            new Vector2(0.5f, 0f),
            new Vector2(0.5f, 0f),
            hotbarPanelPosition,
            hotbarPanelSize,
            applyLayout
        );

        float slotWidth = 110f;
        float slotHeight = 72f;
        float gap = 10f;
        float totalWidth = slotWidth * 7f + gap * 6f;
        float startX = -totalWidth / 2f + slotWidth / 2f;

        for (int i = 0; i < 7; i++)
        {
            RectTransform slotRoot = GetOrCreatePanel("Slot" + (i + 1), hotbarPanel, normalSlotColor);

            ApplyRect(
                slotRoot,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(startX + i * (slotWidth + gap), 0f),
                new Vector2(slotWidth, slotHeight),
                applyLayout
            );

            SlotUI slot = new SlotUI();

            slot.root = slotRoot;
            slot.background = slotRoot.GetComponent<Image>();

            slot.keyText = CreateText(
                "KeyText",
                slotRoot,
                "[" + (i + 1) + "]",
                new Vector2(0f, -7f),
                new Vector2(slotWidth, 22f),
                16,
                TextAlignmentOptions.Center
            );

            slot.titleText = CreateText(
                "TitleText",
                slotRoot,
                "ITEM",
                new Vector2(0f, -30f),
                new Vector2(slotWidth, 24f),
                15,
                TextAlignmentOptions.Center
            );

            slot.countText = CreateText(
                "CountText",
                slotRoot,
                "",
                new Vector2(0f, -52f),
                new Vector2(slotWidth, 20f),
                17,
                TextAlignmentOptions.Center
            );

            slots[i] = slot;
        }
    }

    private void BuildInteractionText(bool applyLayout)
    {
        interactionText = CreateText(
            "InteractionText",
            hudRoot,
            "Press E",
            interactionTextPosition,
            new Vector2(700f, 60f),
            25,
            TextAlignmentOptions.Center
        );

        RectTransform rect = interactionText.rectTransform;

        if (applyLayout || rect.anchorMin != new Vector2(0.5f, 0.5f))
        {
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = interactionTextPosition;
            rect.sizeDelta = new Vector2(700f, 60f);
        }

        interactionText.gameObject.SetActive(false);
    }

    private void CacheUIReferences()
    {
        if (targetCanvas == null)
            return;

        Transform root = targetCanvas.transform.Find("HUD_ROOT_FLEXIBLE");

        if (root == null)
            return;

        hudRoot = root.GetComponent<RectTransform>();

        statusPanel = FindRect("StatusPanel");
        staminaPanel = FindRect("StaminaPanel");
        vehiclePanel = FindRect("VehiclePanel");
        weaponPanel = FindRect("WeaponPanel");
        hotbarPanel = FindRect("HotbarPanel");

        healthFill = FindImage("StatusPanel/HealthBarBackground/HealthBarFill");
        armorFill = FindImage("StatusPanel/ArmorBarBackground/ArmorBarFill");
        staminaFill = FindImage("StaminaPanel/StaminaBarBackground/StaminaBarFill");
        fuelFill = FindImage("VehiclePanel/VehicleFuelBarBackground/VehicleFuelBarFill");

        healthValueText = FindText("StatusPanel/HealthValueText");
        armorValueText = FindText("StatusPanel/ArmorValueText");
        armorTypeText = FindText("StatusPanel/ArmorTypeText");

        staminaValueText = FindText("StaminaPanel/StaminaValueText");

        vehicleTitleText = FindText("VehiclePanel/VehicleTitleText");
        vehicleFuelText = FindText("VehiclePanel/VehicleFuelText");
        vehicleBombsText = FindText("VehiclePanel/VehicleBombsText");

        weaponNameText = FindText("WeaponPanel/WeaponNameText");
        ammoText = FindText("WeaponPanel/AmmoText");
        interactionText = FindText("InteractionText");
    }

    private RectTransform FindRect(string path)
    {
        if (hudRoot == null)
            return null;

        Transform found = hudRoot.Find(path);

        if (found == null)
            return null;

        return found.GetComponent<RectTransform>();
    }

    private TMP_Text FindText(string path)
    {
        if (hudRoot == null)
            return null;

        Transform found = hudRoot.Find(path);

        if (found == null)
            return null;

        return found.GetComponent<TMP_Text>();
    }

    private Image FindImage(string path)
    {
        if (hudRoot == null)
            return null;

        Transform found = hudRoot.Find(path);

        if (found == null)
            return null;

        return found.GetComponent<Image>();
    }

    private RectTransform GetOrCreateRect(string name, Transform parent)
    {
        Transform found = parent.Find(name);

        if (found != null)
            return found.GetComponent<RectTransform>();

        GameObject obj = new GameObject(name);
        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.SetParent(parent, false);

        return rect;
    }

    private RectTransform GetOrCreatePanel(string name, Transform parent, Color color)
    {
        RectTransform rect = GetOrCreateRect(name, parent);

        Image image = rect.GetComponent<Image>();

        if (image == null)
            image = rect.gameObject.AddComponent<Image>();

        image.color = color;
        image.raycastTarget = false;

        Shadow shadow = rect.GetComponent<Shadow>();

        if (shadow == null)
            shadow = rect.gameObject.AddComponent<Shadow>();

        shadow.effectColor = new Color(0f, 0f, 0f, 0.55f);
        shadow.effectDistance = new Vector2(4f, -4f);

        return rect;
    }

    private RectTransform GetOrCreateImage(string name, Transform parent, Color color)
    {
        RectTransform rect = GetOrCreateRect(name, parent);

        Image image = rect.GetComponent<Image>();

        if (image == null)
            image = rect.gameObject.AddComponent<Image>();

        image.color = color;
        image.raycastTarget = false;

        return rect;
    }

    private Image GetOrCreateFill(string name, Transform parent, Color color)
    {
        RectTransform rect = GetOrCreateRect(name, parent);

        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = Vector2.zero;

        Image image = rect.GetComponent<Image>();

        if (image == null)
            image = rect.gameObject.AddComponent<Image>();

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
        Vector2 position,
        Vector2 size,
        int fontSize,
        TextAlignmentOptions alignment
    )
    {
        RectTransform rect = GetOrCreateRect(name, parent);

        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        TextMeshProUGUI tmp = rect.GetComponent<TextMeshProUGUI>();

        if (tmp == null)
            tmp = rect.gameObject.AddComponent<TextMeshProUGUI>();

        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = alignment;
        tmp.color = textColor;
        tmp.fontStyle = FontStyles.Bold;
        tmp.enableWordWrapping = false;
        tmp.raycastTarget = false;

        Outline outline = rect.GetComponent<Outline>();

        if (outline == null)
            outline = rect.gameObject.AddComponent<Outline>();

        outline.effectColor = new Color(0f, 0f, 0f, 0.65f);
        outline.effectDistance = new Vector2(1.2f, -1.2f);

        return tmp;
    }

    private void ApplyRect(
        RectTransform rect,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 pivot,
        Vector2 position,
        Vector2 size,
        bool applyLayout
    )
    {
        if (!applyLayout && rect.sizeDelta != Vector2.zero)
            return;

        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = pivot;
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
    }

    private void ApplyTopLeftRect(
        RectTransform rect,
        Vector2 position,
        Vector2 size,
        bool applyLayout
    )
    {
        if (!applyLayout && rect.sizeDelta != Vector2.zero)
            return;

        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
    }

    private void SetFullScreen(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = Vector2.zero;
    }

    private void UpdateHealthUI()
    {
        if (playerHealth == null)
            return;

        float targetFill = 0f;

        if (playerHealth.maxHealth > 0f)
            targetFill = playerHealth.currentHealth / playerHealth.maxHealth;

        targetFill = Mathf.Clamp01(targetFill);

        smoothHealthFill = Mathf.Lerp(
            smoothHealthFill,
            targetFill,
            Time.deltaTime * barSmoothSpeed
        );

        if (healthFill != null)
        {
            healthFill.fillAmount = smoothHealthFill;

            if (playerHealth.currentHealth <= 25f)
                healthFill.color = lowHealthColor;
            else
                healthFill.color = healthColor;
        }

        if (healthValueText != null)
        {
            healthValueText.text =
                Mathf.CeilToInt(playerHealth.currentHealth) +
                " / " +
                Mathf.CeilToInt(playerHealth.maxHealth);
        }
    }

    private void UpdateArmorUI()
    {
        if (playerArmor == null)
            return;

        float targetFill = 0f;

        if (playerArmor.hasArmor && playerArmor.maxArmor > 0f)
            targetFill = playerArmor.currentArmor / playerArmor.maxArmor;

        targetFill = Mathf.Clamp01(targetFill);

        smoothArmorFill = Mathf.Lerp(
            smoothArmorFill,
            targetFill,
            Time.deltaTime * barSmoothSpeed
        );

        if (armorFill != null)
        {
            armorFill.fillAmount = smoothArmorFill;
            armorFill.color = armorColor;
        }

        if (armorValueText != null)
        {
            if (playerArmor.hasArmor)
            {
                armorValueText.text =
                    Mathf.CeilToInt(playerArmor.currentArmor) +
                    " / " +
                    Mathf.CeilToInt(playerArmor.maxArmor);
            }
            else
            {
                armorValueText.text = "0 / 0";
            }
        }

        if (armorTypeText != null)
        {
            if (playerArmor.hasArmor)
                armorTypeText.text = "TYPE: " + playerArmor.equippedArmorType;
            else
                armorTypeText.text = "TYPE: None";
        }
    }

    private void UpdateStaminaUI()
    {
        if (staminaPanel != null)
            staminaPanel.gameObject.SetActive(showStamina);

        if (!showStamina)
            return;

        float targetFill = 0f;

        if (maxStamina > 0f)
            targetFill = currentStamina / maxStamina;

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
        if (showVehiclePanel)
        {
            if (Time.time - lastVehicleHUDUpdateTime > vehicleAutoHideDelay)
            {
                showVehiclePanel = false;
                currentVehicleMode = VehicleMode.None;
            }
        }

        if (vehiclePanel != null)
            vehiclePanel.gameObject.SetActive(showVehiclePanel);

        if (!showVehiclePanel)
            return;

        float targetFill = 0f;

        if (maxFuel > 0f)
            targetFill = currentFuel / maxFuel;

        targetFill = Mathf.Clamp01(targetFill);

        smoothFuelFill = Mathf.Lerp(
            smoothFuelFill,
            targetFill,
            Time.deltaTime * barSmoothSpeed
        );

        if (fuelFill != null)
        {
            fuelFill.fillAmount = smoothFuelFill;

            if (currentFuel <= maxFuel * 0.20f)
                fuelFill.color = lowFuelColor;
            else
                fuelFill.color = fuelColor;
        }

        if (vehicleTitleText != null)
            vehicleTitleText.text = vehicleName.ToUpper();

        if (vehicleFuelText != null)
        {
            vehicleFuelText.text =
                "FUEL: " +
                Mathf.CeilToInt(currentFuel) +
                " / " +
                Mathf.CeilToInt(maxFuel);
        }

        if (vehicleBombsText != null)
        {
            vehicleBombsText.gameObject.SetActive(showBombs);

            if (showBombs)
            {
                vehicleBombsText.text =
                    "BOMBS: " +
                    currentBombs +
                    " / " +
                    maxBombs;
            }
        }
    }

    private void UpdateWeaponUI()
    {
        if (weaponNameText == null || ammoText == null)
            return;

        if (weaponSwitcher == null)
        {
            weaponNameText.text = "";
            ammoText.text = "";
            return;
        }

        Weapon activeWeapon = weaponSwitcher.GetActiveWeapon();

        if (activeWeapon == null)
        {
            weaponNameText.text = "ITEM";
            ammoText.text = weaponSwitcher.selectedSlot.ToString();
            return;
        }

        weaponNameText.text = activeWeapon.weaponName;
        ammoText.text = activeWeapon.AmmoInMagazine + " / " + activeWeapon.ReserveAmmo;
    }

    private void UpdateHotbarUI()
    {
        SetSlot(
            0,
            "1",
            GetWeaponName(0, "WEAPON 1"),
            "",
            HasWeapon(0),
            IsSelected(WeaponSwitcher.QuickSlot.Weapon1)
        );

        SetSlot(
            1,
            "2",
            GetWeaponName(1, "WEAPON 2"),
            "",
            HasWeapon(1),
            IsSelected(WeaponSwitcher.QuickSlot.Weapon2)
        );

        SetSlot(
            2,
            "3",
            "GRENADE",
            "x" + GetNormalGrenadeCount(),
            GetNormalGrenadeCount() > 0,
            IsSelected(WeaponSwitcher.QuickSlot.NormalGrenade)
        );

        SetSlot(
            3,
            "4",
            "MOLOTOV",
            "x" + GetMolotovCount(),
            GetMolotovCount() > 0,
            IsSelected(WeaponSwitcher.QuickSlot.Molotov)
        );

        SetSlot(
            4,
            "5",
            "PLATFORM",
            "x" + GetPlatformCount(),
            GetPlatformCount() > 0,
            IsSelected(WeaponSwitcher.QuickSlot.JumpPlatform)
        );

        SetSlot(
            5,
            "6",
            "HEALTH",
            "x" + GetHealthCount(),
            GetHealthCount() > 0,
            IsHealthSelected()
        );

        SetSlot(
            6,
            "7",
            "ARMOR",
            "x" + GetArmorCount(),
            GetArmorCount() > 0,
            IsSelected(WeaponSwitcher.QuickSlot.ArmorItem)
        );
    }

    private void SetSlot(
        int index,
        string key,
        string title,
        string count,
        bool hasItem,
        bool selected
    )
    {
        if (index < 0 || index >= slots.Length)
            return;

        SlotUI slot = slots[index];

        if (slot == null)
            return;

        Color backgroundColor = normalSlotColor;
        Color finalTextColor = textColor;

        if (!hasItem)
        {
            backgroundColor = emptySlotColor;
            finalTextColor = emptyTextColor;
        }

        if (selected)
        {
            backgroundColor = selectedSlotColor;
            finalTextColor = selectedTextColor;
        }

        if (slot.background != null)
            slot.background.color = backgroundColor;

        if (slot.keyText != null)
        {
            slot.keyText.text = "[" + key + "]";
            slot.keyText.color = finalTextColor;
        }

        if (slot.titleText != null)
        {
            slot.titleText.text = title;
            slot.titleText.color = finalTextColor;
        }

        if (slot.countText != null)
        {
            if (hasItem)
                slot.countText.text = count;
            else
                slot.countText.text = "";

            slot.countText.color = finalTextColor;
        }
    }

    private bool IsSelected(WeaponSwitcher.QuickSlot slot)
    {
        if (weaponSwitcher == null)
            return false;

        return weaponSwitcher.selectedSlot == slot;
    }

    private bool IsHealthSelected()
    {
        if (weaponSwitcher == null)
            return false;

        return weaponSwitcher.selectedSlot == WeaponSwitcher.QuickSlot.HealthItem1 ||
               weaponSwitcher.selectedSlot == WeaponSwitcher.QuickSlot.HealthItem2 ||
               weaponSwitcher.selectedSlot == WeaponSwitcher.QuickSlot.HealthItem3 ||
               weaponSwitcher.selectedSlot == WeaponSwitcher.QuickSlot.HealthItem4;
    }

    private bool HasWeapon(int index)
    {
        if (weaponSwitcher == null)
            return false;

        Weapon[] weapons = weaponSwitcher.GetComponentsInChildren<Weapon>(true);

        return weapons.Length > index;
    }

    private string GetWeaponName(int index, string defaultName)
    {
        if (weaponSwitcher == null)
            return defaultName;

        Weapon[] weapons = weaponSwitcher.GetComponentsInChildren<Weapon>(true);

        if (weapons.Length <= index)
            return defaultName;

        if (weapons[index] == null)
            return defaultName;

        if (string.IsNullOrEmpty(weapons[index].weaponName))
            return defaultName;

        return weapons[index].weaponName.ToUpper();
    }

    private int GetNormalGrenadeCount()
    {
        if (grenadeInventory == null)
            return 0;

        return grenadeInventory.GetGrenadeCount(GrenadeType.Normal);
    }

    private int GetMolotovCount()
    {
        if (grenadeInventory == null)
            return 0;

        return grenadeInventory.GetGrenadeCount(GrenadeType.Molotov);
    }

    private int GetPlatformCount()
    {
        if (jumpPlatformInventory == null)
            return 0;

        return jumpPlatformInventory.GetPlatformCount();
    }

    private int GetHealthCount()
    {
        if (healthInventory == null)
            return 0;

        return healthInventory.GetItemCount();
    }

    private int GetArmorCount()
    {
        if (armorInventory == null)
            return 0;

        return armorInventory.GetItemCount();
    }

    public void SetStamina(float current, float max)
    {
        showStamina = true;

        maxStamina = Mathf.Max(1f, max);
        currentStamina = Mathf.Clamp(current, 0f, maxStamina);
    }

    public void ShowCarFuel(float current, float max)
    {
        showVehiclePanel = true;
        currentVehicleMode = VehicleMode.Car;

        vehicleName = "CAR";

        maxFuel = Mathf.Max(1f, max);
        currentFuel = Mathf.Clamp(current, 0f, maxFuel);

        showBombs = false;
        currentBombs = 0;
        maxBombs = 0;

        lastVehicleHUDUpdateTime = Time.time;
    }

    public void ShowHelicopterHUD(float current, float max, int bombs, int maxBombsValue)
    {
        showVehiclePanel = true;
        currentVehicleMode = VehicleMode.Helicopter;

        vehicleName = "HELICOPTER";

        maxFuel = Mathf.Max(1f, max);
        currentFuel = Mathf.Clamp(current, 0f, maxFuel);

        showBombs = true;
        currentBombs = Mathf.Max(0, bombs);
        maxBombs = Mathf.Max(0, maxBombsValue);

        lastVehicleHUDUpdateTime = Time.time;
    }

    public void HideVehicleHUD()
    {
        showVehiclePanel = false;
        currentVehicleMode = VehicleMode.None;
    }

    public void HideCarHUD()
    {
        if (currentVehicleMode != VehicleMode.Car)
            return;

        showVehiclePanel = false;
        currentVehicleMode = VehicleMode.None;
    }

    public void HideHelicopterHUD()
    {
        if (currentVehicleMode != VehicleMode.Helicopter)
            return;

        showVehiclePanel = false;
        currentVehicleMode = VehicleMode.None;
    }

    public void ShowInteraction(string message)
    {
        if (interactionText == null)
            return;

        interactionText.gameObject.SetActive(true);
        interactionText.text = message;
    }

    public void HideInteraction()
    {
        if (interactionText == null)
            return;

        interactionText.gameObject.SetActive(false);
    }

    private void MarkSceneDirty()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying && targetCanvas != null)
        {
            EditorUtility.SetDirty(targetCanvas.gameObject);
            EditorSceneManager.MarkSceneDirty(targetCanvas.gameObject.scene);
        }
#endif
    }
}