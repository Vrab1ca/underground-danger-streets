using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthInventory : MonoBehaviour
{
    [Header("References")]
    public PlayerHealth playerHealth;

    [Header("Inventory")]
    [Tooltip("Legacy value only. The real limit is the number of empty hotbar slots.")]
    public int maxHealthItems = 999;

    public List<HealthItemType> healthItems = new List<HealthItemType>();

    [Header("Selected")]
    public int selectedIndex = 0;

    [Header("Debug")]
    public bool debugMessages = true;

    private void Awake()
    {
        FindPlayerHealth();
        FixSelectedIndex();
    }

    private void FindPlayerHealth()
    {
        if (playerHealth != null)
            return;

        playerHealth = GetComponent<PlayerHealth>();

        if (playerHealth == null)
            playerHealth = GetComponentInParent<PlayerHealth>();

        if (playerHealth == null)
            playerHealth = GetComponentInChildren<PlayerHealth>(true);

        if (playerHealth == null)
            playerHealth = FindFirstObjectByType<PlayerHealth>();
    }

    public bool HasHealthItem()
    {
        return healthItems != null && healthItems.Count > 0;
    }

    public int GetItemCount()
    {
        return healthItems == null ? 0 : healthItems.Count;
    }

    public bool CanAddHealthItem()
    {
        // WeaponSwitcher checks whether an empty hotbar slot exists.
        return true;
    }

    public bool AddHealthItem(HealthItemType itemType)
    {
        if (healthItems == null)
            healthItems = new List<HealthItemType>();

        healthItems.Add(itemType);
        selectedIndex = healthItems.Count - 1;

        if (debugMessages)
        {
            Debug.Log(
                "HEALTH ITEM ADDED: " + itemType +
                " | Stored health items: " + healthItems.Count
            );
        }

        return true;
    }

    public HealthItemType GetSelectedItem()
    {
        if (!HasHealthItem())
            return HealthItemType.Small20;

        FixSelectedIndex();
        return healthItems[selectedIndex];
    }

    public HealthItemType GetItemAtIndex(int index)
    {
        if (healthItems == null || index < 0 || index >= healthItems.Count)
            return HealthItemType.Small20;

        return healthItems[index];
    }

    public void SelectNextHealthItem()
    {
        if (!HasHealthItem())
            return;

        selectedIndex++;

        if (selectedIndex >= healthItems.Count)
            selectedIndex = 0;
    }

    public void SelectPreviousHealthItem()
    {
        if (!HasHealthItem())
            return;

        selectedIndex--;

        if (selectedIndex < 0)
            selectedIndex = healthItems.Count - 1;
    }

    public bool UseSelectedHealthItem()
    {
        FindPlayerHealth();

        if (playerHealth == null)
        {
            Debug.LogWarning("PlayerHealth is missing.");
            return false;
        }

        if (!HasHealthItem())
            return false;

        FixSelectedIndex();

        HealthItemType itemType = healthItems[selectedIndex];

        switch (itemType)
        {
            case HealthItemType.Small20:
                playerHealth.Heal(20f);
                break;

            case HealthItemType.Medium50:
                playerHealth.Heal(50f);
                break;

            case HealthItemType.Full100:
                playerHealth.Heal(100f);
                break;

            case HealthItemType.Regen50:
                playerHealth.HealOverTime(50f, 10f, 1f);
                break;
        }

        healthItems.RemoveAt(selectedIndex);
        FixSelectedIndex();

        if (debugMessages)
            Debug.Log("USED HEALTH: " + itemType);

        return true;
    }

    public bool UseHealthItemAtIndex(int index)
    {
        if (healthItems == null || index < 0 || index >= healthItems.Count)
            return false;

        selectedIndex = index;
        return UseSelectedHealthItem();
    }

    public int FindItemIndex(HealthItemType itemType)
    {
        if (healthItems == null)
            return -1;

        for (int i = 0; i < healthItems.Count; i++)
        {
            if (healthItems[i] == itemType)
                return i;
        }

        return -1;
    }

    private void FixSelectedIndex()
    {
        if (healthItems == null)
            healthItems = new List<HealthItemType>();

        if (healthItems.Count <= 0)
        {
            selectedIndex = 0;
            return;
        }

        selectedIndex = Mathf.Clamp(selectedIndex, 0, healthItems.Count - 1);
    }
}