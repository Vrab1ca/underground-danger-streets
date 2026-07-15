using System;
using UnityEngine;

public class MeleeWeaponDropInput : MonoBehaviour
{
    [Serializable]
    public class MeleeDropDefinition
    {
        [Tooltip("Must match Weapon -> Weapon Name, for example: Baseball Bat")]
        public string weaponName;

        [Tooltip("Ground pickup prefab for this weapon.")]
        public GameObject pickupPrefab;
    }

    [Header("References")]
    public WeaponSwitcher weaponSwitcher;

    [Header("Control")]
    public KeyCode dropKey = KeyCode.G;

    [Header("Melee Pickup Prefabs")]
    public MeleeDropDefinition[] meleeWeapons;

    [Header("Debug")]
    public bool debugMessages = true;

    private void Awake()
    {
        FindWeaponSwitcher();
    }

    private void Update()
    {
        if (!Input.GetKeyDown(dropKey))
            return;

        FindWeaponSwitcher();

        if (weaponSwitcher == null)
        {
            Debug.LogError("MeleeWeaponDropInput cannot find WeaponSwitcher.");
            return;
        }

        Weapon activeWeapon = weaponSwitcher.GetActiveWeapon();

        if (activeWeapon == null)
        {
            if (debugMessages)
                Debug.Log("DROP FAILED: Select a weapon hotbar slot first.");

            return;
        }

        GameObject pickupToDrop = activeWeapon.pickupPrefab;

        if (pickupToDrop == null)
        {
            pickupToDrop = FindPickupPrefab(activeWeapon.weaponName);

            if (pickupToDrop != null)
                activeWeapon.pickupPrefab = pickupToDrop;
        }

        if (pickupToDrop == null)
        {
            Debug.LogError(
                "DROP FAILED: No pickup prefab assigned for weapon: " +
                activeWeapon.weaponName
            );
            return;
        }

        if (debugMessages)
            Debug.Log("DROPPING WEAPON: " + activeWeapon.weaponName);

        weaponSwitcher.DropCurrentWeapon();
    }

    private GameObject FindPickupPrefab(string activeWeaponName)
    {
        if (meleeWeapons == null)
            return null;

        string safeActiveName =
            string.IsNullOrWhiteSpace(activeWeaponName)
                ? ""
                : activeWeaponName.Trim();

        for (int i = 0; i < meleeWeapons.Length; i++)
        {
            MeleeDropDefinition definition = meleeWeapons[i];

            if (definition == null || definition.pickupPrefab == null)
                continue;

            string safeDefinitionName =
                string.IsNullOrWhiteSpace(definition.weaponName)
                    ? ""
                    : definition.weaponName.Trim();

            if (string.Equals(
                    safeActiveName,
                    safeDefinitionName,
                    StringComparison.OrdinalIgnoreCase
                ))
            {
                return definition.pickupPrefab;
            }
        }

        return null;
    }

    private void FindWeaponSwitcher()
    {
        if (weaponSwitcher != null)
            return;

        weaponSwitcher = GetComponent<WeaponSwitcher>();

        if (weaponSwitcher == null)
            weaponSwitcher = GetComponentInChildren<WeaponSwitcher>(true);

        if (weaponSwitcher == null)
            weaponSwitcher = GetComponentInParent<WeaponSwitcher>();

        if (weaponSwitcher == null)
            weaponSwitcher = FindFirstObjectByType<WeaponSwitcher>();
    }
}