using TMPro;
using UnityEngine;

public class QuickItemsHUD : MonoBehaviour
{
    [Header("References")]
    public PlayerGrenadeInventory grenadeInventory;
    public JumpPlatformInventory jumpPlatformInventory;
    public PlayerHealthInventory healthInventory;
    public WeaponSwitcher weaponSwitcher;

    [Header("UI")]
    public TMP_Text itemsText;

    private void Start()
    {
        AutoFindReferences();
    }

    private void Update()
    {
        AutoFindReferences();
        UpdateHUD();
    }

    private void AutoFindReferences()
    {
        if (grenadeInventory == null)
            grenadeInventory = FindFirstObjectByType<PlayerGrenadeInventory>();

        if (jumpPlatformInventory == null)
            jumpPlatformInventory = FindFirstObjectByType<JumpPlatformInventory>();

        if (healthInventory == null)
            healthInventory = FindFirstObjectByType<PlayerHealthInventory>();

        if (weaponSwitcher == null)
            weaponSwitcher = FindFirstObjectByType<WeaponSwitcher>();
    }

    private void UpdateHUD()
    {
        if (itemsText == null)
            return;

        int normalGrenades = 0;
        int molotovs = 0;
        int platforms = 0;
        int healthItems = 0;
        int maxHealthItems = 4;

        if (grenadeInventory != null)
        {
            normalGrenades = grenadeInventory.GetGrenadeCount(GrenadeType.Normal);
            molotovs = grenadeInventory.GetGrenadeCount(GrenadeType.Molotov);
        }

        if (jumpPlatformInventory != null)
            platforms = jumpPlatformInventory.GetPlatformCount();

        if (healthInventory != null)
        {
            healthItems = healthInventory.GetItemCount();
            maxHealthItems = healthInventory.maxHealthItems;
        }

        string selectedSlotText = "None";

        if (weaponSwitcher != null)
            selectedSlotText = weaponSwitcher.selectedSlot.ToString();

        string selectedHealthText = "";

        if (healthInventory != null && healthInventory.HasHealthItem())
            selectedHealthText = "\nHealth Selected: " + healthInventory.GetSelectedItem();

        itemsText.text =
            "Items\n" +
            "Grenades [3]: " + normalGrenades + "\n" +
            "Molotovs [4]: " + molotovs + "\n" +
            "Platforms [5]: " + platforms + "\n" +
            "Health [6]: " + healthItems + " / " + maxHealthItems + "\n" +
            "Selected: " + selectedSlotText +
            selectedHealthText;
    }
}