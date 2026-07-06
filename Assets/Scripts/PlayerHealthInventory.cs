using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthInventory : MonoBehaviour
{
    [Header("References")]
    public PlayerHealth playerHealth;

    [Header("Inventory")]
    public int maxHealthItems = 4;
    public List<HealthItemType> healthItems = new List<HealthItemType>();

    [Header("Selected")]
    public int selectedIndex = 0;

    [Header("Debug")]
    public bool debugMessages = true;

    private void Awake()
    {
        if (playerHealth == null)
            playerHealth = GetComponent<PlayerHealth>();
    }

    public bool HasHealthItem()
    {
        return healthItems.Count > 0;
    }

    public int GetItemCount()
    {
        return healthItems.Count;
    }

    public bool CanAddHealthItem()
    {
        return healthItems.Count < maxHealthItems;
    }

    public bool AddHealthItem(HealthItemType itemType)
    {
        if (!CanAddHealthItem())
        {
            Debug.Log("Health inventory FULL: " + healthItems.Count + " / " + maxHealthItems);
            return false;
        }

        healthItems.Add(itemType);

        if (selectedIndex < 0)
            selectedIndex = 0;

        if (selectedIndex >= healthItems.Count)
            selectedIndex = healthItems.Count - 1;

        Debug.Log("HEALTH ADDED TO INVENTORY: " + itemType + " | Count: " + healthItems.Count + " / " + maxHealthItems);

        return true;
    }

    public HealthItemType GetSelectedItem()
    {
        if (healthItems.Count <= 0)
            return HealthItemType.Small20;

        selectedIndex = Mathf.Clamp(selectedIndex, 0, healthItems.Count - 1);

        return healthItems[selectedIndex];
    }

    public void SelectNextHealthItem()
    {
        if (healthItems.Count <= 0)
            return;

        selectedIndex++;

        if (selectedIndex >= healthItems.Count)
            selectedIndex = 0;

        Debug.Log("Selected health item: " + GetSelectedItem());
    }

    public void SelectPreviousHealthItem()
    {
        if (healthItems.Count <= 0)
            return;

        selectedIndex--;

        if (selectedIndex < 0)
            selectedIndex = healthItems.Count - 1;

        Debug.Log("Selected health item: " + GetSelectedItem());
    }

    public bool UseSelectedHealthItem()
    {
        if (playerHealth == null)
        {
            Debug.LogWarning("PlayerHealth missing.");
            return false;
        }

        if (healthItems.Count <= 0)
        {
            Debug.Log("No health items.");
            return false;
        }

        selectedIndex = Mathf.Clamp(selectedIndex, 0, healthItems.Count - 1);

        HealthItemType itemType = healthItems[selectedIndex];

        if (itemType == HealthItemType.Small20)
            playerHealth.Heal(20f);

        if (itemType == HealthItemType.Medium50)
            playerHealth.Heal(50f);

        if (itemType == HealthItemType.Full100)
            playerHealth.Heal(100f);

        if (itemType == HealthItemType.Regen50)
            playerHealth.HealOverTime(50f, 10f, 1f);

        healthItems.RemoveAt(selectedIndex);

        if (selectedIndex >= healthItems.Count)
            selectedIndex = healthItems.Count - 1;

        if (selectedIndex < 0)
            selectedIndex = 0;

        Debug.Log("USED HEALTH: " + itemType + " | Left: " + healthItems.Count);

        return true;
    }
}