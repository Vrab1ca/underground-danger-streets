using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProGameHUDUI : MonoBehaviour
{
    [System.Serializable]
    public class SlotUI
    {
        public Image background;
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

    [Header("Status UI")]
    public Image healthFill;
    public TMP_Text healthValueText;

    public Image armorFill;
    public TMP_Text armorValueText;
    public TMP_Text armorTypeText;

    [Header("Weapon UI")]
    public TMP_Text weaponNameText;
    public TMP_Text ammoText;

    [Header("Hotbar Slots")]
    public SlotUI slot1;
    public SlotUI slot2;
    public SlotUI slot3;
    public SlotUI slot4;
    public SlotUI slot5;
    public SlotUI slot6;
    public SlotUI slot7;

    [Header("Center UI")]
    public TMP_Text crosshairText;
    public TMP_Text interactionText;

    [Header("Colors")]
    public Color normalSlotColor = new Color(0.08f, 0.10f, 0.14f, 0.85f);
    public Color selectedSlotColor = new Color(1f, 0.72f, 0.18f, 0.95f);
    public Color emptySlotColor = new Color(0.03f, 0.03f, 0.04f, 0.55f);

    public Color healthColor = new Color(0.95f, 0.15f, 0.18f, 1f);
    public Color lowHealthColor = new Color(1f, 0.05f, 0.05f, 1f);
    public Color armorColor = new Color(0.20f, 0.60f, 1f, 1f);

    [Header("Smooth Animation")]
    public float barSmoothSpeed = 8f;

    private float currentHealthFill;
    private float currentArmorFill;

    private void Start()
    {
        AutoFindReferences();

        currentHealthFill = 1f;
        currentArmorFill = 0f;

        if (interactionText != null)
            interactionText.gameObject.SetActive(false);

        if (crosshairText != null)
            crosshairText.text = "+";
    }

    private void Update()
    {
        AutoFindReferences();

        UpdateHealthUI();
        UpdateArmorUI();
        UpdateWeaponUI();
        UpdateHotbarUI();
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

    private void UpdateHealthUI()
    {
        if (playerHealth == null)
            return;

        float targetFill = playerHealth.currentHealth / playerHealth.maxHealth;
        targetFill = Mathf.Clamp01(targetFill);

        currentHealthFill = Mathf.Lerp(currentHealthFill, targetFill, Time.deltaTime * barSmoothSpeed);

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

        currentArmorFill = Mathf.Lerp(currentArmorFill, targetFill, Time.deltaTime * barSmoothSpeed);

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
            weaponNameText.text = "";
            ammoText.text = "";
            return;
        }

        weaponNameText.text = activeWeapon.weaponName;
        ammoText.text = activeWeapon.AmmoInMagazine + " / " + activeWeapon.ReserveAmmo;
    }

    private void UpdateHotbarUI()
    {
        SetSlot(
            slot1,
            "[1]",
            "WEAPON",
            "",
            WeaponSwitcher.QuickSlot.Weapon1,
            HasWeapon(0)
        );

        SetSlot(
            slot2,
            "[2]",
            "WEAPON",
            "",
            WeaponSwitcher.QuickSlot.Weapon2,
            HasWeapon(1)
        );

        SetSlot(
            slot3,
            "[3]",
            "GRENADE",
            GetNormalGrenadeCount().ToString(),
            WeaponSwitcher.QuickSlot.NormalGrenade,
            GetNormalGrenadeCount() > 0
        );

        SetSlot(
            slot4,
            "[4]",
            "MOLOTOV",
            GetMolotovCount().ToString(),
            WeaponSwitcher.QuickSlot.Molotov,
            GetMolotovCount() > 0
        );

        SetSlot(
            slot5,
            "[5]",
            "PLATFORM",
            GetPlatformCount().ToString(),
            WeaponSwitcher.QuickSlot.JumpPlatform,
            GetPlatformCount() > 0
        );

        SetSlot(
            slot6,
            "[6]",
            "HEALTH",
            GetHealthCount().ToString(),
            WeaponSwitcher.QuickSlot.HealthItem1,
            GetHealthCount() > 0
        );

        SetSlot(
            slot7,
            "[7]",
            "ARMOR",
            GetArmorCount().ToString(),
            WeaponSwitcher.QuickSlot.ArmorItem,
            GetArmorCount() > 0
        );
    }

    private void SetSlot(
        SlotUI slot,
        string keyText,
        string title,
        string count,
        WeaponSwitcher.QuickSlot slotType,
        bool hasItem
    )
    {
        if (slot == null)
            return;

        bool selected = false;

        if (weaponSwitcher != null)
        {
            selected = weaponSwitcher.selectedSlot == slotType;

            if (slotType == WeaponSwitcher.QuickSlot.HealthItem1)
            {
                selected =
                    weaponSwitcher.selectedSlot == WeaponSwitcher.QuickSlot.HealthItem1 ||
                    weaponSwitcher.selectedSlot == WeaponSwitcher.QuickSlot.HealthItem2 ||
                    weaponSwitcher.selectedSlot == WeaponSwitcher.QuickSlot.HealthItem3 ||
                    weaponSwitcher.selectedSlot == WeaponSwitcher.QuickSlot.HealthItem4;
            }
        }

        if (slot.background != null)
        {
            if (!hasItem)
                slot.background.color = emptySlotColor;
            else if (selected)
                slot.background.color = selectedSlotColor;
            else
                slot.background.color = normalSlotColor;
        }

        if (slot.titleText != null)
        {
            if (selected)
                slot.titleText.text = keyText + "\n" + title;
            else
                slot.titleText.text = keyText + "\n" + title;
        }

        if (slot.countText != null)
        {
            if (string.IsNullOrEmpty(count))
                slot.countText.text = "";
            else
                slot.countText.text = "x" + count;
        }
    }

    private bool HasWeapon(int index)
    {
        if (weaponSwitcher == null)
            return false;

        Weapon[] weapons = weaponSwitcher.GetComponentsInChildren<Weapon>(true);

        return weapons.Length > index;
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