using UnityEngine;
using System.Collections.Generic;

public class WeaponSwitcher : MonoBehaviour
{
    public enum QuickSlot
    {
        Weapon1,
        Weapon2,
        NormalGrenade,
        Molotov,
        JumpPlatform
    }

    [Header("Selected Slot")]
    public QuickSlot selectedSlot = QuickSlot.Weapon1;

    [Header("Inventory")]
    public int maxWeapons = 2;

    [Header("Camera References")]
    public Camera fpsCamera;
    public Camera carCamera;

    [Header("Sniper Scope UI")]
    public GameObject scopeOverlay;
    public GameObject normalCrosshairUI;

    [Header("Grenade System")]
    public PlayerGrenadeInventory grenadeInventory;
    public GameObject normalGrenadeVisual;
    public GameObject molotovVisual;

    [Header("Jump Platform")]
    public JumpPlatformInventory jumpPlatformInventory;
    public GameObject jumpPlatformVisual;

    [Header("Drop")]
    public Transform dropPoint;
    public KeyCode dropKey = KeyCode.G;

    private int selectedWeaponIndex = 0;

    private void Start()
    {
        SelectFirstAvailableSlot();
    }

    private void Update()
    {
        HandleNumberKeys();
        HandleScroll();

        if (Input.GetKeyDown(dropKey))
        {
            if (IsWeaponSlot())
                DropCurrentWeapon();
        }

        HandleUseSelectedItem();

        if (!IsSlotAvailable(selectedSlot))
            SelectFirstAvailableSlot();
    }

    private void HandleNumberKeys()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            TrySelectSlot(QuickSlot.Weapon1);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            TrySelectSlot(QuickSlot.Weapon2);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            TrySelectSlot(QuickSlot.NormalGrenade);

        if (Input.GetKeyDown(KeyCode.Alpha4))
            TrySelectSlot(QuickSlot.Molotov);

        if (Input.GetKeyDown(KeyCode.Alpha5))
            TrySelectSlot(QuickSlot.JumpPlatform);
    }

    private void HandleScroll()
    {
        Weapon activeWeapon = GetActiveWeapon();

        if (activeWeapon != null && activeWeapon.IsSniperZooming())
            return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f)
            SelectNextAvailableSlot();

        if (scroll < 0f)
            SelectPreviousAvailableSlot();
    }

    private void TrySelectSlot(QuickSlot slot)
    {
        if (!IsSlotAvailable(slot))
        {
            Debug.Log("Slot not available: " + slot);
            return;
        }

        SelectSlot(slot);
    }

    private void SelectNextAvailableSlot()
    {
        int current = (int)selectedSlot;

        for (int i = 1; i <= 5; i++)
        {
            int next = current + i;

            if (next > 4)
                next -= 5;

            QuickSlot nextSlot = (QuickSlot)next;

            if (IsSlotAvailable(nextSlot))
            {
                SelectSlot(nextSlot);
                return;
            }
        }
    }

    private void SelectPreviousAvailableSlot()
    {
        int current = (int)selectedSlot;

        for (int i = 1; i <= 5; i++)
        {
            int previous = current - i;

            if (previous < 0)
                previous += 5;

            QuickSlot previousSlot = (QuickSlot)previous;

            if (IsSlotAvailable(previousSlot))
            {
                SelectSlot(previousSlot);
                return;
            }
        }
    }

    private void SelectFirstAvailableSlot()
    {
        if (IsSlotAvailable(QuickSlot.Weapon1))
        {
            SelectSlot(QuickSlot.Weapon1);
            return;
        }

        if (IsSlotAvailable(QuickSlot.Weapon2))
        {
            SelectSlot(QuickSlot.Weapon2);
            return;
        }

        if (IsSlotAvailable(QuickSlot.NormalGrenade))
        {
            SelectSlot(QuickSlot.NormalGrenade);
            return;
        }

        if (IsSlotAvailable(QuickSlot.Molotov))
        {
            SelectSlot(QuickSlot.Molotov);
            return;
        }

        if (IsSlotAvailable(QuickSlot.JumpPlatform))
        {
            SelectSlot(QuickSlot.JumpPlatform);
            return;
        }

        HideAllWeaponsAndItems();
    }

    private bool IsSlotAvailable(QuickSlot slot)
    {
        Weapon[] weapons = GetWeapons();

        if (slot == QuickSlot.Weapon1)
            return weapons.Length >= 1;

        if (slot == QuickSlot.Weapon2)
            return weapons.Length >= 2;

        if (slot == QuickSlot.NormalGrenade)
        {
            if (grenadeInventory == null)
                return false;

            return grenadeInventory.GetGrenadeCount(GrenadeType.Normal) > 0;
        }

        if (slot == QuickSlot.Molotov)
        {
            if (grenadeInventory == null)
                return false;

            return grenadeInventory.GetGrenadeCount(GrenadeType.Molotov) > 0;
        }

        if (slot == QuickSlot.JumpPlatform)
        {
            return GetJumpPlatformCount() > 0;
        }

        return false;
    }

    public void SelectSlot(QuickSlot slot)
    {
        selectedSlot = slot;

        if (slot == QuickSlot.Weapon1)
            selectedWeaponIndex = 0;

        if (slot == QuickSlot.Weapon2)
            selectedWeaponIndex = 1;

        if (slot == QuickSlot.NormalGrenade && grenadeInventory != null)
            grenadeInventory.SelectNormalGrenade();

        if (slot == QuickSlot.Molotov && grenadeInventory != null)
            grenadeInventory.SelectMolotov();

        UpdateWeaponVisibility();
        UpdateItemVisuals();

        Debug.Log("Selected slot: " + selectedSlot);
    }

    private void UpdateWeaponVisibility()
    {
        Weapon[] weapons = GetWeapons();

        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] == null)
                continue;

            bool shouldShow =
                IsWeaponSlot() &&
                i == selectedWeaponIndex &&
                IsSlotAvailable(selectedSlot);

            weapons[i].gameObject.SetActive(shouldShow);
        }
    }

    private void UpdateItemVisuals()
    {
        bool showNormalGrenade =
            selectedSlot == QuickSlot.NormalGrenade &&
            grenadeInventory != null &&
            grenadeInventory.GetGrenadeCount(GrenadeType.Normal) > 0;

        bool showMolotov =
            selectedSlot == QuickSlot.Molotov &&
            grenadeInventory != null &&
            grenadeInventory.GetGrenadeCount(GrenadeType.Molotov) > 0;

        bool showJumpPlatform =
            selectedSlot == QuickSlot.JumpPlatform &&
            GetJumpPlatformCount() > 0;

        if (normalGrenadeVisual != null)
            normalGrenadeVisual.SetActive(showNormalGrenade);

        if (molotovVisual != null)
            molotovVisual.SetActive(showMolotov);

        if (jumpPlatformVisual != null)
            jumpPlatformVisual.SetActive(showJumpPlatform);
    }

    private void HideAllWeaponsAndItems()
    {
        Weapon[] weapons = GetWeapons();

        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] != null)
                weapons[i].gameObject.SetActive(false);
        }

        if (normalGrenadeVisual != null)
            normalGrenadeVisual.SetActive(false);

        if (molotovVisual != null)
            molotovVisual.SetActive(false);

        if (jumpPlatformVisual != null)
            jumpPlatformVisual.SetActive(false);
    }

    private void HandleUseSelectedItem()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        if (selectedSlot == QuickSlot.NormalGrenade)
        {
            if (grenadeInventory != null && grenadeInventory.GetGrenadeCount(GrenadeType.Normal) > 0)
            {
                grenadeInventory.SelectNormalGrenade();
                grenadeInventory.ThrowNormalGrenade();

                if (grenadeInventory.GetGrenadeCount(GrenadeType.Normal) <= 0)
                    SelectFirstAvailableSlot();
                else
                    UpdateItemVisuals();
            }

            return;
        }

        if (selectedSlot == QuickSlot.Molotov)
        {
            if (grenadeInventory != null && grenadeInventory.GetGrenadeCount(GrenadeType.Molotov) > 0)
            {
                grenadeInventory.SelectMolotov();
                grenadeInventory.ThrowMolotov();

                if (grenadeInventory.GetGrenadeCount(GrenadeType.Molotov) <= 0)
                    SelectFirstAvailableSlot();
                else
                    UpdateItemVisuals();
            }

            return;
        }

        if (selectedSlot == QuickSlot.JumpPlatform)
        {
            UseJumpPlatform();

            if (GetJumpPlatformCount() <= 0)
                SelectFirstAvailableSlot();
            else
                UpdateItemVisuals();

            return;
        }
    }

    private void UseJumpPlatform()
    {
        if (jumpPlatformInventory == null)
        {
            Debug.LogWarning("Jump Platform Inventory is missing.");
            return;
        }

        jumpPlatformInventory.PlacePlatform();

        Debug.Log("Placed jump platform.");
    }

    private int GetJumpPlatformCount()
    {
        if (jumpPlatformInventory == null)
            return 0;

        return jumpPlatformInventory.GetPlatformCount();
    }

    private bool IsWeaponSlot()
    {
        return selectedSlot == QuickSlot.Weapon1 || selectedSlot == QuickSlot.Weapon2;
    }

    public bool AddWeapon(GameObject weaponPrefab)
    {
        if (weaponPrefab == null)
        {
            Debug.LogWarning("Weapon prefab is missing.");
            return false;
        }

        int weaponCount = GetWeaponCount();

        if (weaponCount >= maxWeapons)
        {
            Debug.LogWarning("Inventory full. You can carry only 2 weapons. Press G to drop one.");
            return false;
        }

        GameObject newWeapon = Instantiate(weaponPrefab, transform);

        newWeapon.transform.localPosition = Vector3.zero;
        newWeapon.transform.localRotation = Quaternion.identity;
        newWeapon.transform.localScale = weaponPrefab.transform.localScale;

        PrepareWeapon(newWeapon);

        int newWeaponIndex = GetWeaponCount() - 1;

        if (newWeaponIndex <= 0)
            SelectSlot(QuickSlot.Weapon1);
        else
            SelectSlot(QuickSlot.Weapon2);

        Debug.Log("Picked weapon: " + newWeapon.name);

        return true;
    }

    private void PrepareWeapon(GameObject weaponObject)
    {
        Weapon weapon = weaponObject.GetComponent<Weapon>();

        if (weapon != null)
        {
            weapon.fpsCamera = fpsCamera;
            weapon.carCamera = carCamera;

            weapon.scopeOverlay = scopeOverlay;
            weapon.normalCrosshairUI = normalCrosshairUI;
        }
    }

    public void DropCurrentWeapon()
    {
        Weapon activeWeapon = GetActiveWeapon();

        if (activeWeapon == null)
        {
            Debug.Log("No weapon to drop.");
            return;
        }

        if (activeWeapon.pickupPrefab == null)
        {
            Debug.LogWarning("Pickup Prefab missing on weapon: " + activeWeapon.weaponName);
            return;
        }

        Vector3 spawnPosition;

        if (dropPoint != null)
            spawnPosition = dropPoint.position;
        else
            spawnPosition = transform.position + transform.forward * 2f;

        GameObject droppedPickup = Instantiate(
            activeWeapon.pickupPrefab,
            spawnPosition,
            Quaternion.identity
        );

        droppedPickup.SetActive(true);

        WeaponPickupBoxRadius pickupScript = droppedPickup.GetComponent<WeaponPickupBoxRadius>();

        if (pickupScript != null)
        {
            pickupScript.pickupDistance = 3f;
            pickupScript.pickupKey = KeyCode.F;
        }

        Debug.Log("Dropped weapon: " + activeWeapon.weaponName);

        Destroy(activeWeapon.gameObject);

        selectedWeaponIndex = 0;

        Invoke(nameof(SelectFirstAvailableSlot), 0.05f);
    }

    public Weapon GetActiveWeapon()
    {
        if (!IsWeaponSlot())
            return null;

        Weapon[] weapons = GetWeapons();

        if (weapons.Length == 0)
            return null;

        if (selectedWeaponIndex < 0)
            selectedWeaponIndex = 0;

        if (selectedWeaponIndex >= weapons.Length)
            selectedWeaponIndex = weapons.Length - 1;

        return weapons[selectedWeaponIndex];
    }

    private int GetWeaponCount()
    {
        return GetWeapons().Length;
    }

    private Weapon[] GetWeapons()
    {
        List<Weapon> weapons = new List<Weapon>();

        for (int i = 0; i < transform.childCount; i++)
        {
            Weapon weapon = transform.GetChild(i).GetComponent<Weapon>();

            if (weapon != null)
                weapons.Add(weapon);
        }

        return weapons.ToArray();
    }
}