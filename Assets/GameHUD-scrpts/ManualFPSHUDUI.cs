using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ManualFPSHUDUI : MonoBehaviour
{
    public static ManualFPSHUDUI Instance { get; private set; }

    public enum VehicleMode
    {
        None,
        Car,
        Helicopter
    }

    [System.Serializable]
    public class HotbarSlotUI
    {
        public GameObject slotPanel;
        public Image backgroundImage;
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

    [Header("Panels")]
    public GameObject statusPanel;
    public GameObject staminaPanel;
    public GameObject weaponPanel;
    public GameObject vehiclePanel;
    public GameObject hotbarPanel;
    public GameObject interactionPanel;

    [Header("Health UI")]
    public Slider healthSlider;
    public TMP_Text healthText;

    [Header("Armor UI")]
    public Slider armorSlider;
    public TMP_Text armorText;
    public TMP_Text armorTypeText;

    [Header("Stamina UI")]
    public Slider staminaSlider;
    public TMP_Text staminaText;

    [Header("Weapon UI")]
    public TMP_Text weaponNameText;
    public TMP_Text ammoText;

    [Header("Vehicle UI")]
    public TMP_Text vehicleTitleText;
    public Slider vehicleFuelSlider;
    public TMP_Text vehicleFuelText;
    public TMP_Text vehicleBombsText;

    [Header("Interaction UI")]
    public TMP_Text interactionText;

    [Header("Dynamic Hotbar UI - Maximum 8 Slots")]
    public HotbarSlotUI slot1;
    public HotbarSlotUI slot2;
    public HotbarSlotUI slot3;
    public HotbarSlotUI slot4;
    public HotbarSlotUI slot5;
    public HotbarSlotUI slot6;
    public HotbarSlotUI slot7;
    public HotbarSlotUI slot8;

    [Header("Stamina Values")]
    public bool showStamina = true;
    public float currentStamina = 100f;
    public float maxStamina = 100f;

    [Header("Vehicle Values")]
    public bool showVehiclePanel;
    public VehicleMode currentVehicleMode = VehicleMode.None;

    public string vehicleName = "CAR";
    public float currentFuel = 100f;
    public float maxFuel = 100f;

    public int currentBombs = 0;
    public int maxBombs = 0;

    [Header("Colors")]
    public Color normalSlotColor =
        new Color(0.08f, 0.10f, 0.14f, 0.90f);

    public Color selectedSlotColor =
        new Color(1f, 0.72f, 0.18f, 1f);

    public Color emptySlotColor =
        new Color(0.03f, 0.03f, 0.04f, 0.65f);

    public Color normalTextColor = Color.white;
    public Color selectedTextColor = Color.black;

    public Color emptyTextColor =
        new Color(0.65f, 0.65f, 0.65f, 1f);

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void Start()
    {
        AutoFindReferences();

        if (vehiclePanel != null)
            vehiclePanel.SetActive(false);

        if (interactionPanel != null)
            interactionPanel.SetActive(false);
    }

    private void Update()
    {
        AutoFindReferences();

        UpdateHealthUI();
        UpdateArmorUI();
        UpdateStaminaUI();
        UpdateWeaponUI();
        UpdateVehicleUI();
        UpdateHotbarUI();
    }

    private void AutoFindReferences()
    {
        if (playerHealth == null)
        {
            playerHealth =
                FindFirstObjectByType<PlayerHealth>();
        }

        if (playerArmor == null)
        {
            playerArmor =
                FindFirstObjectByType<PlayerArmor>();
        }

        if (weaponSwitcher == null)
        {
            weaponSwitcher =
                FindFirstObjectByType<WeaponSwitcher>();
        }

        if (grenadeInventory == null)
        {
            grenadeInventory =
                FindFirstObjectByType<PlayerGrenadeInventory>();
        }

        if (jumpPlatformInventory == null)
        {
            jumpPlatformInventory =
                FindFirstObjectByType<JumpPlatformInventory>();
        }

        if (healthInventory == null)
        {
            healthInventory =
                FindFirstObjectByType<PlayerHealthInventory>();
        }

        if (armorInventory == null)
        {
            armorInventory =
                FindFirstObjectByType<PlayerArmorInventory>();
        }
    }

    // =========================================================
    // HEALTH
    // =========================================================

    private void UpdateHealthUI()
    {
        if (playerHealth == null)
            return;

        if (healthSlider != null)
        {
            healthSlider.maxValue =
                playerHealth.maxHealth;

            healthSlider.value =
                playerHealth.currentHealth;
        }

        if (healthText != null)
        {
            healthText.text =
                Mathf.CeilToInt(
                    playerHealth.currentHealth
                ) +
                " / " +
                Mathf.CeilToInt(
                    playerHealth.maxHealth
                );
        }
    }

    // =========================================================
    // ARMOR
    // =========================================================

    private void UpdateArmorUI()
    {
        if (playerArmor == null)
            return;

        if (armorSlider != null)
        {
            if (playerArmor.hasArmor)
            {
                armorSlider.maxValue =
                    playerArmor.maxArmor;

                armorSlider.value =
                    playerArmor.currentArmor;
            }
            else
            {
                armorSlider.maxValue = 1f;
                armorSlider.value = 0f;
            }
        }

        if (armorText != null)
        {
            if (playerArmor.hasArmor)
            {
                armorText.text =
                    Mathf.CeilToInt(
                        playerArmor.currentArmor
                    ) +
                    " / " +
                    Mathf.CeilToInt(
                        playerArmor.maxArmor
                    );
            }
            else
            {
                armorText.text = "0 / 0";
            }
        }

        if (armorTypeText != null)
        {
            if (playerArmor.hasArmor)
            {
                armorTypeText.text =
                    "TYPE: " +
                    playerArmor.equippedArmorType;
            }
            else
            {
                armorTypeText.text = "TYPE: None";
            }
        }
    }

    // =========================================================
    // STAMINA
    // =========================================================

    private void UpdateStaminaUI()
    {
        if (staminaPanel != null)
        {
            staminaPanel.SetActive(showStamina);
        }

        if (!showStamina)
            return;

        if (staminaSlider != null)
        {
            staminaSlider.maxValue =
                maxStamina;

            staminaSlider.value =
                currentStamina;
        }

        if (staminaText != null)
        {
            staminaText.text =
                Mathf.CeilToInt(currentStamina) +
                " / " +
                Mathf.CeilToInt(maxStamina);
        }
    }

    // =========================================================
    // SELECTED ITEM / WEAPON
    // =========================================================

    private void UpdateWeaponUI()
    {
        if (weaponSwitcher == null)
            return;

        Weapon activeWeapon =
            weaponSwitcher.GetActiveWeapon();

        if (activeWeapon != null)
        {
            if (weaponNameText != null)
            {
                weaponNameText.text =
                    activeWeapon.weaponName.ToUpper();
            }

            if (ammoText != null)
            {
                if (activeWeapon.weaponMode ==
                    Weapon.WeaponMode.Melee)
                {
                    // Knife, bat and other melee weapons
                    // do not show ammunition.
                    ammoText.text = "";
                }
                else
                {
                    ammoText.text =
                        activeWeapon.AmmoInMagazine +
                        " / " +
                        activeWeapon.ReserveAmmo;
                }
            }

            return;
        }

        // Shows HANDS, HEALTH, GRENADE, ARMOR, etc.
        if (weaponNameText != null)
        {
            weaponNameText.text =
                weaponSwitcher.GetSelectedItemTitle();
        }

        if (ammoText != null)
            ammoText.text = "";
    }

    // =========================================================
    // VEHICLE
    // =========================================================

    private void UpdateVehicleUI()
    {
        if (vehiclePanel != null)
        {
            vehiclePanel.SetActive(
                showVehiclePanel
            );
        }

        if (!showVehiclePanel)
            return;

        if (vehicleTitleText != null)
        {
            vehicleTitleText.text =
                vehicleName;
        }

        if (vehicleFuelSlider != null)
        {
            vehicleFuelSlider.maxValue =
                maxFuel;

            vehicleFuelSlider.value =
                currentFuel;
        }

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
            if (currentVehicleMode ==
                VehicleMode.Helicopter)
            {
                vehicleBombsText.gameObject
                    .SetActive(true);

                vehicleBombsText.text =
                    "BOMBS: " +
                    currentBombs +
                    " / " +
                    maxBombs;
            }
            else
            {
                vehicleBombsText.gameObject
                    .SetActive(false);
            }
        }
    }

    // =========================================================
    // DYNAMIC HOTBAR
    // =========================================================

    private void UpdateHotbarUI()
    {
        HotbarSlotUI[] uiSlots =
        {
            slot1,
            slot2,
            slot3,
            slot4,
            slot5,
            slot6,
            slot7,
            slot8
        };

        int visibleCount = 0;

        if (weaponSwitcher != null)
        {
            visibleCount =
                weaponSwitcher.SlotCount;
        }

        for (int i = 0;
             i < uiSlots.Length;
             i++)
        {
            HotbarSlotUI uiSlot =
                uiSlots[i];

            if (uiSlot == null)
                continue;

            bool shouldShow =
                weaponSwitcher != null &&
                i < visibleCount;

            if (uiSlot.slotPanel != null)
            {
                uiSlot.slotPanel.SetActive(
                    shouldShow
                );
            }

            if (!shouldShow)
                continue;

            SetSlot(
                uiSlot,
                "[" + (i + 1) + "]",
                weaponSwitcher.GetSlotTitle(i),
                weaponSwitcher.GetSlotCountText(i),
                weaponSwitcher.IsSlotFilled(i),
                weaponSwitcher.IsSlotSelected(i)
            );
        }
    }

    private void SetSlot(
        HotbarSlotUI slot,
        string key,
        string title,
        string count,
        bool hasItem,
        bool selected
    )
    {
        if (slot == null)
            return;

        Color bgColor =
            normalSlotColor;

        Color finalTextColor =
            normalTextColor;

        if (!hasItem)
        {
            bgColor =
                emptySlotColor;

            finalTextColor =
                emptyTextColor;
        }

        if (selected)
        {
            bgColor =
                selectedSlotColor;

            finalTextColor =
                selectedTextColor;
        }

        if (slot.backgroundImage != null)
        {
            slot.backgroundImage.color =
                bgColor;
        }

        if (slot.keyText != null)
        {
            slot.keyText.text = key;

            slot.keyText.color =
                finalTextColor;
        }

        if (slot.titleText != null)
        {
            slot.titleText.text =
                title;

            slot.titleText.color =
                finalTextColor;
        }

        if (slot.countText != null)
        {
            if (hasItem)
            {
                slot.countText.text =
                    count;
            }
            else
            {
                slot.countText.text = "";
            }

            slot.countText.color =
                finalTextColor;
        }
    }

    // =========================================================
    // PUBLIC STAMINA METHODS
    // =========================================================

    public void SetStamina(
        float current,
        float max
    )
    {
        showStamina = true;

        maxStamina =
            Mathf.Max(1f, max);

        currentStamina =
            Mathf.Clamp(
                current,
                0f,
                maxStamina
            );
    }

    public void HideStamina()
    {
        showStamina = false;
    }

    // =========================================================
    // PUBLIC VEHICLE METHODS
    // =========================================================

    public void ShowCarFuel(
        float current,
        float max
    )
    {
        currentVehicleMode =
            VehicleMode.Car;

        showVehiclePanel = true;

        vehicleName = "CAR";

        maxFuel =
            Mathf.Max(1f, max);

        currentFuel =
            Mathf.Clamp(
                current,
                0f,
                maxFuel
            );

        currentBombs = 0;
        maxBombs = 0;
    }

    public void ShowHelicopterHUD(
        float current,
        float max,
        int bombs,
        int maxBombsValue
    )
    {
        currentVehicleMode =
            VehicleMode.Helicopter;

        showVehiclePanel = true;

        vehicleName =
            "HELICOPTER";

        maxFuel =
            Mathf.Max(1f, max);

        currentFuel =
            Mathf.Clamp(
                current,
                0f,
                maxFuel
            );

        currentBombs =
            Mathf.Max(0, bombs);

        maxBombs =
            Mathf.Max(
                0,
                maxBombsValue
            );
    }

    public void HideVehicleHUD()
    {
        currentVehicleMode =
            VehicleMode.None;

        showVehiclePanel = false;
    }

    public void HideCarHUD()
    {
        if (currentVehicleMode !=
            VehicleMode.Car)
        {
            return;
        }

        currentVehicleMode =
            VehicleMode.None;

        showVehiclePanel = false;
    }

    public void HideHelicopterHUD()
    {
        if (currentVehicleMode !=
            VehicleMode.Helicopter)
        {
            return;
        }

        currentVehicleMode =
            VehicleMode.None;

        showVehiclePanel = false;
    }

    // =========================================================
    // INTERACTION MESSAGE
    // =========================================================

    public void ShowInteraction(
        string message
    )
    {
        if (interactionPanel != null)
        {
            interactionPanel.SetActive(true);
        }

        if (interactionText != null)
        {
            interactionText.text =
                message;
        }
    }

    public void HideInteraction()
    {
        if (interactionPanel != null)
        {
            interactionPanel.SetActive(false);
        }
    }
}