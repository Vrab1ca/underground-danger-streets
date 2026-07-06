using TMPro;
using UnityEngine;

public class HealthInventoryHUD : MonoBehaviour
{
    [Header("References")]
    public PlayerHealthInventory healthInventory;
    public WeaponSwitcher weaponSwitcher;
    public TMP_Text healthText;

    private void Update()
    {
        if (healthInventory == null || healthText == null)
            return;

        int count = healthInventory.GetItemCount();

        if (count <= 0)
        {
            healthText.text = "Health Items: 0 / " + healthInventory.maxHealthItems;
            return;
        }

        string selectedText = healthInventory.GetSelectedItem().ToString();

        bool selectedHealthSlot = false;

        if (weaponSwitcher != null)
            selectedHealthSlot = IsHealthSlot(weaponSwitcher.selectedSlot);

        if (selectedHealthSlot)
        {
            healthText.text =
                "Health Items: " + count + " / " + healthInventory.maxHealthItems +
                "\nSelected Slot: " + weaponSwitcher.selectedSlot +
                "\nSelected Item: " + selectedText +
                "\nLeft Mouse: Use";
        }
        else
        {
            healthText.text =
                "Health Items: " + count + " / " + healthInventory.maxHealthItems +
                "\nPress 6 or scroll to select";
        }
    }

    private bool IsHealthSlot(WeaponSwitcher.QuickSlot slot)
    {
        return slot == WeaponSwitcher.QuickSlot.HealthItem1 ||
               slot == WeaponSwitcher.QuickSlot.HealthItem2 ||
               slot == WeaponSwitcher.QuickSlot.HealthItem3 ||
               slot == WeaponSwitcher.QuickSlot.HealthItem4;
    }
}