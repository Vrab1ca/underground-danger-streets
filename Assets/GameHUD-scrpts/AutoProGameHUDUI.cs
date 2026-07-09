using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AutoProGameHUDUI : MonoBehaviour
{
    private class SlotRuntimeUI
    {
        public Image background;
        public TMP_Text keyText;
        public TMP_Text titleText;
        public TMP_Text countText;
    }

    [Header("Player References")]
    public PlayerHealth playerHealth;
    public PlayerArmor playerArmor;
    public WeaponSwitcher weaponSwitcher;
    public PlayerGrenadeInventory grenadeInventory;
    public JumpPlatformInventory jumpPlatformInventory;
    public PlayerHealthInventory healthInventory;
    public PlayerArmorInventory armorInventory;

    [Header("Canvas")]
    public Canvas targetCanvas;
    public Vector2 referenceResolution = new Vector2(1920, 1080);

    [Header("Colors")]
    public Color panelColor = new Color(0.02f, 0.025f, 0.035f, 0.80f);
    public Color panelShadowColor = new Color(0f, 0f, 0f, 0.55f);

    public Color healthColor = new Color(0.95f, 0.12f, 0.14f, 1f);
    public Color lowHealthColor = new Color(1f, 0.03f, 0.03f, 1f);
    public Color armorColor = new Color(0.15f, 0.55f, 1f, 1f);

    public Color barBackgroundColor = new Color(0f, 0f, 0f, 0.55f);

    public Color normalSlotColor = new Color(0.08f, 0.10f, 0.14f, 0.88f);
    public Color selectedSlotColor = new Color(1f, 0.72f, 0.18f, 0.98f);
    public Color emptySlotColor = new Color(0.03f, 0.03f, 0.04f, 0.60f);

    public Color textColor = Color.white;
    public Color selectedTextColor = Color.black;
    public Color emptyTextColor = new Color(0.65f, 0.65f, 0.65f, 1f);

    [Header("Animation")]
    public float barSmoothSpeed = 8f;

    private RectTransform hudRoot;

    private Image healthFill;
    private Image armorFill;

    private TMP_Text healthValueText;
    private TMP_Text armorValueText;
    private TMP_Text armorTypeText;

    private TMP_Text weaponNameText;
    private TMP_Text ammoText;

 
    private TMP_Text interactionText;

    private SlotRuntimeUI[] slots = new SlotRuntimeUI[7];

    private float currentHealthFill = 1f;
    private float currentArmorFill = 0f;

    private void Start()
    {
        SetupCanvas();
        AutoFindReferences();
        BuildHUD();
    }

    private void Update()
    {
        AutoFindReferences();

        UpdateHealthUI();
        UpdateArmorUI();
        UpdateWeaponUI();
        UpdateHotbarUI();
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

    private void BuildHUD()
    {
        Transform oldRoot = targetCanvas.transform.Find("AutoHUDRoot");

        if (oldRoot != null)
            Destroy(oldRoot.gameObject);

        hudRoot = CreateRect("AutoHUDRoot", targetCanvas.transform);
        SetRectFullScreen(hudRoot);

        BuildStatusPanel();
        BuildWeaponPanel();
        BuildHotbarPanel();
        BuildCenterUI();
    }

    private void BuildStatusPanel()
    {
        RectTransform panel = CreatePanel(
            "StatusPanel",
            hudRoot,
            new Vector2(0f, 1f),
            new Vector2(0f, 1f),
            new Vector2(0f, 1f),
            new Vector2(30f, -30f),
            new Vector2(390f, 170f),
            panelColor
        );

        CreateText(
            "HealthLabel",
            panel,
            "HEALTH",
            new Vector2(20f, -18f),
            new Vector2(120f, 25f),
            18,
            TextAlignmentOptions.Left,
            textColor,
            FontStyles.Bold
        );

        healthValueText = CreateText(
            "HealthValueText",
            panel,
            "100 / 100",
            new Vector2(250f, -18f),
            new Vector2(120f, 25f),
            20,
            TextAlignmentOptions.Right,
            textColor,
            FontStyles.Bold
        );

        RectTransform healthBg = CreateImageRect(
            "HealthBarBackground",
            panel,
            new Vector2(20f, -50f),
            new Vector2(350f, 18f),
            barBackgroundColor
        );

        healthFill = CreateFillImage(
            "HealthBarFill",
            healthBg,
            healthColor
        );

        CreateText(
            "ArmorLabel",
            panel,
            "ARMOR",
            new Vector2(20f, -82f),
            new Vector2(120f, 25f),
            18,
            TextAlignmentOptions.Left,
            textColor,
            FontStyles.Bold
        );

        armorValueText = CreateText(
            "ArmorValueText",
            panel,
            "0 / 0",
            new Vector2(250f, -82f),
            new Vector2(120f, 25f),
            20,
            TextAlignmentOptions.Right,
            textColor,
            FontStyles.Bold
        );

        RectTransform armorBg = CreateImageRect(
            "ArmorBarBackground",
            panel,
            new Vector2(20f, -114f),
            new Vector2(350f, 18f),
            barBackgroundColor
        );

        armorFill = CreateFillImage(
            "ArmorBarFill",
            armorBg,
            armorColor
        );

        armorTypeText = CreateText(
            "ArmorTypeText",
            panel,
            "TYPE: None",
            new Vector2(20f, -140f),
            new Vector2(350f, 25f),
            17,
            TextAlignmentOptions.Left,
            textColor,
            FontStyles.Bold
        );
    }

    private void BuildWeaponPanel()
    {
        RectTransform panel = CreatePanel(
            "WeaponPanel",
            hudRoot,
            new Vector2(1f, 0f),
            new Vector2(1f, 0f),
            new Vector2(1f, 0f),
            new Vector2(-40f, 40f),
            new Vector2(310f, 125f),
            panelColor
        );

        weaponNameText = CreateText(
            "WeaponNameText",
            panel,
            "",
            new Vector2(-20f, -18f),
            new Vector2(260f, 35f),
            24,
            TextAlignmentOptions.Right,
            textColor,
            FontStyles.Bold
        );

        ammoText = CreateText(
            "AmmoText",
            panel,
            "",
            new Vector2(-20f, -58f),
            new Vector2(260f, 55f),
            42,
            TextAlignmentOptions.Right,
            textColor,
            FontStyles.Bold
        );
    }

    private void BuildHotbarPanel()
    {
        RectTransform panel = CreatePanel(
            "HotbarPanel",
            hudRoot,
            new Vector2(0.5f, 0f),
            new Vector2(0.5f, 0f),
            new Vector2(0.5f, 0f),
            new Vector2(0f, 28f),
            new Vector2(880f, 92f),
            panelColor
        );

        float slotWidth = 110f;
        float slotHeight = 72f;
        float gap = 10f;

        float totalWidth = slotWidth * 7f + gap * 6f;
        float startX = -totalWidth / 2f + slotWidth / 2f;

        for (int i = 0; i < 7; i++)
        {
            RectTransform slotRect = CreatePanel(
                "Slot" + (i + 1),
                panel,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(startX + i * (slotWidth + gap), 0f),
                new Vector2(slotWidth, slotHeight),
                normalSlotColor
            );

            SlotRuntimeUI slot = new SlotRuntimeUI();
            slot.background = slotRect.GetComponent<Image>();

            slot.keyText = CreateText(
                "KeyText",
                slotRect,
                "[" + (i + 1) + "]",
                new Vector2(0f, -7f),
                new Vector2(slotWidth, 22f),
                16,
                TextAlignmentOptions.Center,
                textColor,
                FontStyles.Bold
            );

            slot.titleText = CreateText(
                "TitleText",
                slotRect,
                "ITEM",
                new Vector2(0f, -30f),
                new Vector2(slotWidth, 24f),
                15,
                TextAlignmentOptions.Center,
                textColor,
                FontStyles.Bold
            );

            slot.countText = CreateText(
                "CountText",
                slotRect,
                "",
                new Vector2(0f, -52f),
                new Vector2(slotWidth, 20f),
                17,
                TextAlignmentOptions.Center,
                textColor,
                FontStyles.Bold
            );

            slots[i] = slot;
        }
    }

    private void BuildCenterUI()
    {
        interactionText = CreateText(
            "InteractionText",
            hudRoot,
            "Press E",
            new Vector2(0f, -190f),
            new Vector2(700f, 60f),
            25,
            TextAlignmentOptions.Center,
            textColor,
            FontStyles.Bold
        );

        SetRectCenter(interactionText.rectTransform, new Vector2(0f, -190f));
        interactionText.gameObject.SetActive(false);
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

    private void SetRectCenter(RectTransform rect, Vector2 anchoredPosition)
    {
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPosition;
    }

    private void UpdateHealthUI()
    {
        if (playerHealth == null)
            return;

        float targetFill = 0f;

        if (playerHealth.maxHealth > 0f)
            targetFill = playerHealth.currentHealth / playerHealth.maxHealth;

        targetFill = Mathf.Clamp01(targetFill);

        currentHealthFill = Mathf.Lerp(
            currentHealthFill,
            targetFill,
            Time.deltaTime * barSmoothSpeed
        );

        if (healthFill != null)
        {
            healthFill.fillAmount = currentHealthFill;

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

        currentArmorFill = Mathf.Lerp(
            currentArmorFill,
            targetFill,
            Time.deltaTime * barSmoothSpeed
        );

        if (armorFill != null)
        {
            armorFill.fillAmount = currentArmorFill;
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

        SlotRuntimeUI slot = slots[index];

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
}