using System.Collections.Generic;
using UnityEngine;

public class PlayerArmorInventory : MonoBehaviour
{
    [Header("References")]
    public PlayerArmor playerArmor;

    [Header("Inventory")]
    [Tooltip("Legacy value only. The real limit is the number of empty hotbar slots.")]
    public int maxArmorItems = 999;

    public List<ArmorItemType> armorItems = new List<ArmorItemType>();

    [Header("Selected")]
    public int selectedIndex = 0;

    [Header("Debug")]
    public bool debugMessages = true;

    private void Awake()
    {
        FindPlayerArmor();
        FixSelectedIndex();
    }

    private void FindPlayerArmor()
    {
        if (playerArmor != null)
            return;

        playerArmor = GetComponent<PlayerArmor>();

        if (playerArmor == null)
            playerArmor = GetComponentInParent<PlayerArmor>();

        if (playerArmor == null)
            playerArmor = GetComponentInChildren<PlayerArmor>(true);

        if (playerArmor == null)
            playerArmor = FindFirstObjectByType<PlayerArmor>();
    }

    public bool HasArmorItem()
    {
        return armorItems != null && armorItems.Count > 0;
    }

    public int GetItemCount()
    {
        return armorItems == null ? 0 : armorItems.Count;
    }

    public bool CanAddArmorItem()
    {
        // WeaponSwitcher checks whether an empty hotbar slot exists.
        return true;
    }

    public bool AddArmorItem(ArmorItemType armorType)
    {
        if (armorItems == null)
            armorItems = new List<ArmorItemType>();

        armorItems.Add(armorType);
        selectedIndex = armorItems.Count - 1;

        if (debugMessages)
        {
            Debug.Log(
                "ARMOR ADDED: " + armorType +
                " | Stored armor items: " + armorItems.Count
            );
        }

        return true;
    }

    public ArmorItemType GetSelectedArmor()
    {
        if (!HasArmorItem())
            return ArmorItemType.Strong100;

        FixSelectedIndex();
        return armorItems[selectedIndex];
    }

    public ArmorItemType GetArmorAtIndex(int index)
    {
        if (armorItems == null || index < 0 || index >= armorItems.Count)
            return ArmorItemType.Strong100;

        return armorItems[index];
    }

    public void SelectNextArmor()
    {
        if (!HasArmorItem())
            return;

        selectedIndex++;

        if (selectedIndex >= armorItems.Count)
            selectedIndex = 0;
    }

    public void SelectPreviousArmor()
    {
        if (!HasArmorItem())
            return;

        selectedIndex--;

        if (selectedIndex < 0)
            selectedIndex = armorItems.Count - 1;
    }

    public bool CanUseSelectedArmor()
    {
        if (!HasArmorItem())
            return false;

        FindPlayerArmor();

        if (playerArmor == null)
        {
            Debug.LogWarning("PlayerArmor is missing.");
            return false;
        }

        if (playerArmor.IsArmorActive)
        {
            if (debugMessages)
            {
                Debug.Log(
                    "Armor is already equipped. Wait until it breaks before using another armor."
                );
            }

            return false;
        }

        return true;
    }

    public bool UseSelectedArmor()
    {
        if (!CanUseSelectedArmor())
            return false;

        FixSelectedIndex();

        ArmorItemType armorType = armorItems[selectedIndex];

        // EquipArmor changes only armor health, never player HP.
        playerArmor.EquipArmor(armorType);

        armorItems.RemoveAt(selectedIndex);
        FixSelectedIndex();

        if (debugMessages)
            Debug.Log("USED ARMOR: " + armorType);

        return true;
    }

    public bool UseArmorAtIndex(int index)
    {
        if (armorItems == null || index < 0 || index >= armorItems.Count)
            return false;

        selectedIndex = index;
        return UseSelectedArmor();
    }

    private void FixSelectedIndex()
    {
        if (armorItems == null)
            armorItems = new List<ArmorItemType>();

        if (armorItems.Count <= 0)
        {
            selectedIndex = 0;
            return;
        }

        selectedIndex = Mathf.Clamp(selectedIndex, 0, armorItems.Count - 1);
    }
}