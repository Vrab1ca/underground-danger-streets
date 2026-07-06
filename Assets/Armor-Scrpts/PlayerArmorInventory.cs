using System.Collections.Generic;
using UnityEngine;

public class PlayerArmorInventory : MonoBehaviour
{
    [Header("References")]
    public PlayerArmor playerArmor;

    [Header("Inventory")]
    public int maxArmorItems = 1;
    public List<ArmorItemType> armorItems = new List<ArmorItemType>();

    [Header("Selected")]
    public int selectedIndex = 0;

    [Header("Debug")]
    public bool debugMessages = true;

    private void Awake()
    {
        if (playerArmor == null)
            playerArmor = GetComponent<PlayerArmor>();

        if (maxArmorItems <= 0)
            maxArmorItems = 1;
    }

    private void OnValidate()
    {
        if (maxArmorItems <= 0)
            maxArmorItems = 1;
    }

    public bool HasArmorItem()
    {
        return armorItems.Count > 0;
    }

    public int GetItemCount()
    {
        return armorItems.Count;
    }

    public bool AddArmorItem(ArmorItemType armorType)
    {
        if (armorItems.Count >= maxArmorItems)
        {
            Debug.Log("Armor inventory full. You can carry only 1 armor item in slot 7.");
            return false;
        }

        armorItems.Add(armorType);
        selectedIndex = 0;

        Debug.Log("ARMOR ADDED TO SLOT 7: " + armorType);

        return true;
    }

    public ArmorItemType GetSelectedArmor()
    {
        if (armorItems.Count <= 0)
            return ArmorItemType.Strong100;

        selectedIndex = Mathf.Clamp(selectedIndex, 0, armorItems.Count - 1);

        return armorItems[selectedIndex];
    }

    public void SelectNextArmor()
    {
        if (armorItems.Count <= 0)
            return;

        selectedIndex = 0;

        Debug.Log("Selected armor: " + GetSelectedArmor());
    }

    public bool CanUseSelectedArmor()
    {
        if (armorItems.Count <= 0)
        {
            Debug.Log("No armor item in slot 7.");
            return false;
        }

        if (playerArmor == null)
        {
            Debug.LogWarning("PlayerArmor missing.");
            return false;
        }

        if (playerArmor.hasArmor && playerArmor.currentArmor > 0f)
        {
            Debug.Log(
                "You already have armor equipped: " +
                playerArmor.equippedArmorType +
                " | Armor left: " +
                playerArmor.currentArmor +
                " / " +
                playerArmor.maxArmor +
                ". Wait until it breaks before using another armor."
            );

            return false;
        }

        return true;
    }

    public bool UseSelectedArmor()
    {
        if (!CanUseSelectedArmor())
            return false;

        selectedIndex = Mathf.Clamp(selectedIndex, 0, armorItems.Count - 1);

        ArmorItemType armorType = armorItems[selectedIndex];

        playerArmor.EquipArmor(armorType);

        armorItems.RemoveAt(selectedIndex);
        selectedIndex = 0;

        Debug.Log("USED ARMOR: " + armorType);

        return true;
    }
}