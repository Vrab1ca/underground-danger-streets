using TMPro;
using UnityEngine;

public class GameHUDUI : MonoBehaviour
{
    [Header("Player References")]
    public PlayerHealth playerHealth;
    public PlayerArmor playerArmor;
    public WeaponSwitcher weaponSwitcher;
    public PlayerGrenadeInventory grenadeInventory;
    public JumpPlatformInventory jumpPlatformInventory;
    public PlayerHealthInventory healthInventory;
    public PlayerArmorInventory armorInventory;

    [Header("Main Texts")]
    public TMP_Text healthText;
    public TMP_Text armorText;
    public TMP_Text ammoText;
    public TMP_Text interactionText;

    [Header("Slot Texts")]
    public TMP_Text slot1Text;
    public TMP_Text slot2Text;
    public TMP_Text slot3Text;
    public TMP_Text slot4Text;
    public TMP_Text slot5Text;
    public TMP_Text slot6Text;
    public TMP_Text slot7Text;

    private void Start()
    {
        AutoFindReferences();

        if (interactionText != null)
            interactionText.gameObject.SetActive(false);
    }

    private void Update()
    {
        AutoFindReferences();

        UpdateHealthUI();
        UpdateArmorUI();
        UpdateAmmoUI();
        UpdateSlotsUI();
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
        if (healthText == null)
            return;

        if (playerHealth == null)
        {
            healthText.text = "Health: ?";
            return;
        }

        healthText.text =
            "Health: " +
            Mathf.CeilToInt(playerHealth.currentHealth) +
            " / " +
            Mathf.CeilToInt(playerHealth.maxHealth);
    }

    private void UpdateArmorUI()
    {
        if (armorText == null)
            return;

        if (playerArmor == null || !playerArmor.hasArmor)
        {
            armorText.text =
                "Armor: 0 / 0" +
                "\nType: None";

            return;
        }

        armorText.text =
            "Armor: " +
            Mathf.CeilToInt(playerArmor.currentArmor) +
            " / " +
            Mathf.CeilToInt(playerArmor.maxArmor) +
            "\nType: " +
            playerArmor.equippedArmorType;
    }

    private void UpdateAmmoUI()
    {
        if (ammoText == null)
            return;

        if (weaponSwitcher == null)
        {
            ammoText.text = "";
            return;
        }

        Weapon activeWeapon = weaponSwitcher.GetActiveWeapon();

        if (activeWeapon == null)
        {
            ammoText.text = "";
            return;
        }

        ammoText.text =
            activeWeapon.weaponName +
            "\n" +
            activeWeapon.AmmoInMagazine +
            " / " +
            activeWeapon.ReserveAmmo;
    }

    private void UpdateSlotsUI()
    {
        SetSlotText(slot1Text, "[1]\nWeapon 1", WeaponSwitcher.QuickSlot.Weapon1);
        SetSlotText(slot2Text, "[2]\nWeapon 2", WeaponSwitcher.QuickSlot.Weapon2);
        SetSlotText(slot3Text, "[3]\nGrenade\n" + GetNormalGrenadeCount(), WeaponSwitcher.QuickSlot.NormalGrenade);
        SetSlotText(slot4Text, "[4]\nMolotov\n" + GetMolotovCount(), WeaponSwitcher.QuickSlot.Molotov);
        SetSlotText(slot5Text, "[5]\nPlatform\n" + GetPlatformCount(), WeaponSwitcher.QuickSlot.JumpPlatform);
        SetSlotText(slot6Text, "[6]\nHealth\n" + GetHealthCount(), GetSelectedHealthSlot());
        SetSlotText(slot7Text, "[7]\nArmor\n" + GetArmorCount(), WeaponSwitcher.QuickSlot.ArmorItem);
    }

    private void SetSlotText(TMP_Text text, string normalText, WeaponSwitcher.QuickSlot slot)
    {
        if (text == null)
            return;

        bool selected = false;

        if (weaponSwitcher != null)
            selected = weaponSwitcher.selectedSlot == slot;

        if (selected)
            text.text = "> " + normalText + " <";
        else
            text.text = normalText;
    }

    private WeaponSwitcher.QuickSlot GetSelectedHealthSlot()
    {
        if (weaponSwitcher == null)
            return WeaponSwitcher.QuickSlot.HealthItem1;

        if (weaponSwitcher.selectedSlot == WeaponSwitcher.QuickSlot.HealthItem1)
            return WeaponSwitcher.QuickSlot.HealthItem1;

        if (weaponSwitcher.selectedSlot == WeaponSwitcher.QuickSlot.HealthItem2)
            return WeaponSwitcher.QuickSlot.HealthItem2;

        if (weaponSwitcher.selectedSlot == WeaponSwitcher.QuickSlot.HealthItem3)
            return WeaponSwitcher.QuickSlot.HealthItem3;

        if (weaponSwitcher.selectedSlot == WeaponSwitcher.QuickSlot.HealthItem4)
            return WeaponSwitcher.QuickSlot.HealthItem4;

        return WeaponSwitcher.QuickSlot.HealthItem1;
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

    public void ShowInteractionText(string message)
    {
        if (interactionText == null)
            return;

        interactionText.gameObject.SetActive(true);
        interactionText.text = message;
    }

    public void HideInteractionText()
    {
        if (interactionText == null)
            return;

        interactionText.gameObject.SetActive(false);
    }
}