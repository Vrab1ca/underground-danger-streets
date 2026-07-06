using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponSwitcher : MonoBehaviour
{
    public enum QuickSlot
    {
        Weapon1,
        Weapon2,
        NormalGrenade,
        Molotov,
        JumpPlatform,
        HealthItem1,
        HealthItem2,
        HealthItem3,
        HealthItem4,
        ArmorItem
    }

    [Header("Selected Slot")]
    public QuickSlot selectedSlot = QuickSlot.Weapon1;

    [Header("Weapons")]
    public int maxWeapons = 2;

    [Header("Camera References")]
    public Camera fpsCamera;
    public Camera carCamera;

    [Header("Sniper Scope UI")]
    public GameObject scopeOverlay;
    public GameObject normalCrosshairUI;

    [Header("Grenades")]
    public PlayerGrenadeInventory grenadeInventory;
    public GameObject normalGrenadeVisual;
    public GameObject molotovVisual;

    [Header("Jump Platform")]
    public JumpPlatformInventory jumpPlatformInventory;
    public GameObject jumpPlatformVisual;

    [Header("Health")]
    public PlayerHealthInventory healthInventory;
    public GameObject healthItemVisual;
    public HealthItemHandVisual healthHandVisual;

    [Header("Armor")]
    public PlayerArmorInventory armorInventory;
    public GameObject armorVisual;
    public ArmorHandVisual armorHandVisual;

    [Header("Drop Weapon")]
    public Transform dropPoint;
    public KeyCode dropKey = KeyCode.G;

    [Header("Scroll")]
    public float scrollCooldown = 0.08f;

    [Header("Debug")]
    public bool debugHotbar = true;

    private int selectedWeaponIndex;
    private float nextScrollTime;
    private bool usingHealthItem;
    private bool usingArmorItem;

    private QuickSlot[] slotOrder =
    {
        QuickSlot.Weapon1,
        QuickSlot.Weapon2,
        QuickSlot.NormalGrenade,
        QuickSlot.Molotov,
        QuickSlot.JumpPlatform,
        QuickSlot.HealthItem1,
        QuickSlot.HealthItem2,
        QuickSlot.HealthItem3,
        QuickSlot.HealthItem4,
        QuickSlot.ArmorItem
    };

    private void Start()
    {
        AutoFindReferences();
        SelectFirstAvailableSlot();
    }

    private void Update()
    {
        AutoFindReferences();

        HandleNumberKeys();
        HandleScroll();
        HandleDropWeapon();
        HandleUseSelectedSlot();

        if (!IsSlotAvailable(selectedSlot))
            SelectFirstAvailableSlot();
    }

    private void AutoFindReferences()
    {
        if (grenadeInventory == null)
            grenadeInventory = FindFirstObjectByType<PlayerGrenadeInventory>();

        if (jumpPlatformInventory == null)
            jumpPlatformInventory = FindFirstObjectByType<JumpPlatformInventory>();

        if (healthInventory == null)
            healthInventory = FindFirstObjectByType<PlayerHealthInventory>();

        if (armorInventory == null)
            armorInventory = FindFirstObjectByType<PlayerArmorInventory>();
    }

    private void HandleNumberKeys()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SelectSlotByButton(QuickSlot.Weapon1);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            SelectSlotByButton(QuickSlot.Weapon2);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            SelectSlotByButton(QuickSlot.NormalGrenade);

        if (Input.GetKeyDown(KeyCode.Alpha4))
            SelectSlotByButton(QuickSlot.Molotov);

        if (Input.GetKeyDown(KeyCode.Alpha5))
            SelectSlotByButton(QuickSlot.JumpPlatform);

        if (Input.GetKeyDown(KeyCode.Alpha6))
            SelectOrCycleHealthSlot();

        if (Input.GetKeyDown(KeyCode.Alpha7))
            SelectOrCycleArmorSlot();
    }

    private void SelectOrCycleHealthSlot()
    {
        if (healthInventory == null || healthInventory.GetItemCount() <= 0)
        {
            Debug.Log("No health items.");
            return;
        }

        if (!IsHealthSlot(selectedSlot))
        {
            SelectSlot(QuickSlot.HealthItem1);
            return;
        }

        if (selectedSlot == QuickSlot.HealthItem1 && IsSlotAvailable(QuickSlot.HealthItem2))
        {
            SelectSlot(QuickSlot.HealthItem2);
            return;
        }

        if (selectedSlot == QuickSlot.HealthItem2 && IsSlotAvailable(QuickSlot.HealthItem3))
        {
            SelectSlot(QuickSlot.HealthItem3);
            return;
        }

        if (selectedSlot == QuickSlot.HealthItem3 && IsSlotAvailable(QuickSlot.HealthItem4))
        {
            SelectSlot(QuickSlot.HealthItem4);
            return;
        }

        SelectSlot(QuickSlot.HealthItem1);
    }

    private void SelectOrCycleArmorSlot()
    {
        if (armorInventory == null || !armorInventory.HasArmorItem())
        {
            Debug.Log("No armor items.");
            return;
        }

        if (selectedSlot == QuickSlot.ArmorItem)
        {
            armorInventory.SelectNextArmor();
            RefreshVisuals();
            return;
        }

        SelectSlot(QuickSlot.ArmorItem);
    }

    private void SelectSlotByButton(QuickSlot slot)
    {
        if (!IsSlotAvailable(slot))
        {
            Debug.Log("This slot is empty: " + slot);
            return;
        }

        SelectSlot(slot);
    }

    private void HandleScroll()
    {
        if (usingHealthItem || usingArmorItem)
            return;

        if (Time.time < nextScrollTime)
            return;

        Weapon activeWeapon = GetActiveWeapon();

        if (activeWeapon != null && activeWeapon.IsSniperZooming())
            return;

        float scroll = Input.mouseScrollDelta.y;

        if (Mathf.Abs(scroll) < 0.01f)
            scroll = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(scroll) < 0.01f)
            return;

        nextScrollTime = Time.time + scrollCooldown;

        if (scroll > 0f)
            ScrollHotbar(1);
        else
            ScrollHotbar(-1);
    }

    private void ScrollHotbar(int direction)
    {
        int currentIndex = System.Array.IndexOf(slotOrder, selectedSlot);

        if (currentIndex < 0)
            currentIndex = 0;

        for (int i = 1; i <= slotOrder.Length; i++)
        {
            int nextIndex = currentIndex + direction * i;

            while (nextIndex < 0)
                nextIndex += slotOrder.Length;

            while (nextIndex >= slotOrder.Length)
                nextIndex -= slotOrder.Length;

            QuickSlot candidateSlot = slotOrder[nextIndex];

            bool available = IsSlotAvailable(candidateSlot);

            if (debugHotbar)
                Debug.Log("Scroll check: " + candidateSlot + " | Available = " + available);

            if (available)
            {
                SelectSlot(candidateSlot);
                return;
            }
        }

        Debug.Log("No available hotbar slot.");
    }

    private void SelectFirstAvailableSlot()
    {
        for (int i = 0; i < slotOrder.Length; i++)
        {
            if (IsSlotAvailable(slotOrder[i]))
            {
                SelectSlot(slotOrder[i]);
                return;
            }
        }

        HideEverything();
    }

    public void SelectSlot(QuickSlot slot)
    {
        if (!IsSlotAvailable(slot))
        {
            Debug.Log("Cannot select empty slot: " + slot);
            return;
        }

        selectedSlot = slot;

        if (selectedSlot == QuickSlot.Weapon1)
            selectedWeaponIndex = 0;

        if (selectedSlot == QuickSlot.Weapon2)
            selectedWeaponIndex = 1;

        if (selectedSlot == QuickSlot.NormalGrenade && grenadeInventory != null)
            grenadeInventory.SelectNormalGrenade();

        if (selectedSlot == QuickSlot.Molotov && grenadeInventory != null)
            grenadeInventory.SelectMolotov();

        if (IsHealthSlot(selectedSlot) && healthInventory != null)
            healthInventory.selectedIndex = GetHealthIndexFromSlot(selectedSlot);

        RefreshVisuals();

        Debug.Log("SELECTED HOTBAR SLOT: " + selectedSlot);
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
            if (jumpPlatformInventory == null)
                return false;

            return jumpPlatformInventory.GetPlatformCount() > 0;
        }

        if (IsHealthSlot(slot))
        {
            if (healthInventory == null)
                return false;

            int healthIndex = GetHealthIndexFromSlot(slot);

            return healthInventory.GetItemCount() > healthIndex;
        }

        if (slot == QuickSlot.ArmorItem)
        {
            if (armorInventory == null)
                return false;

            return armorInventory.HasArmorItem();
        }

        return false;
    }

    private bool IsHealthSlot(QuickSlot slot)
    {
        return slot == QuickSlot.HealthItem1 ||
               slot == QuickSlot.HealthItem2 ||
               slot == QuickSlot.HealthItem3 ||
               slot == QuickSlot.HealthItem4;
    }

    private int GetHealthIndexFromSlot(QuickSlot slot)
    {
        if (slot == QuickSlot.HealthItem1)
            return 0;

        if (slot == QuickSlot.HealthItem2)
            return 1;

        if (slot == QuickSlot.HealthItem3)
            return 2;

        if (slot == QuickSlot.HealthItem4)
            return 3;

        return 0;
    }

    private void RefreshVisuals()
    {
        HideEverything();

        if (selectedSlot == QuickSlot.Weapon1 || selectedSlot == QuickSlot.Weapon2)
            ShowSelectedWeapon();

        if (selectedSlot == QuickSlot.NormalGrenade)
            ShowNormalGrenade();

        if (selectedSlot == QuickSlot.Molotov)
            ShowMolotov();

        if (selectedSlot == QuickSlot.JumpPlatform)
            ShowJumpPlatform();

        if (IsHealthSlot(selectedSlot))
            ShowHealthItem();

        if (selectedSlot == QuickSlot.ArmorItem)
            ShowArmorItem();
    }

    private void HideEverything()
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

        if (healthHandVisual != null)
            healthHandVisual.HideItem();
        else if (healthItemVisual != null)
            healthItemVisual.SetActive(false);

        if (armorHandVisual != null)
            armorHandVisual.HideArmor();
        else if (armorVisual != null)
            armorVisual.SetActive(false);
    }

    private void ShowSelectedWeapon()
    {
        Weapon[] weapons = GetWeapons();

        if (weapons.Length <= 0)
            return;

        selectedWeaponIndex = Mathf.Clamp(selectedWeaponIndex, 0, weapons.Length - 1);

        if (weapons[selectedWeaponIndex] != null)
            weapons[selectedWeaponIndex].gameObject.SetActive(true);
    }

    private void ShowNormalGrenade()
    {
        if (normalGrenadeVisual != null)
            normalGrenadeVisual.SetActive(true);
    }

    private void ShowMolotov()
    {
        if (molotovVisual != null)
            molotovVisual.SetActive(true);
    }

    private void ShowJumpPlatform()
    {
        if (jumpPlatformVisual != null)
            jumpPlatformVisual.SetActive(true);
    }

    private void ShowHealthItem()
    {
        if (healthInventory == null)
            return;

        if (healthInventory.GetItemCount() <= 0)
            return;

        if (healthHandVisual != null)
        {
            healthHandVisual.ShowItem(healthInventory.GetSelectedItem());
            return;
        }

        if (healthItemVisual != null)
            healthItemVisual.SetActive(true);
    }

    private void ShowArmorItem()
    {
        if (armorInventory == null)
            return;

        if (!armorInventory.HasArmorItem())
            return;

        if (armorHandVisual != null)
        {
            armorHandVisual.ShowArmor(armorInventory.GetSelectedArmor());
            return;
        }

        if (armorVisual != null)
            armorVisual.SetActive(true);
    }

    private void HandleDropWeapon()
    {
        if (!Input.GetKeyDown(dropKey))
            return;

        if (selectedSlot == QuickSlot.Weapon1 || selectedSlot == QuickSlot.Weapon2)
            DropCurrentWeapon();
    }

    private void HandleUseSelectedSlot()
    {
        if (usingHealthItem || usingArmorItem)
            return;

        if (!Input.GetMouseButtonDown(0))
            return;

        if (selectedSlot == QuickSlot.NormalGrenade)
        {
            UseNormalGrenade();
            return;
        }

        if (selectedSlot == QuickSlot.Molotov)
        {
            UseMolotov();
            return;
        }

        if (selectedSlot == QuickSlot.JumpPlatform)
        {
            UseJumpPlatform();
            return;
        }

        if (IsHealthSlot(selectedSlot))
        {
            UseHealthItem();
            return;
        }

        if (selectedSlot == QuickSlot.ArmorItem)
        {
            UseArmorItem();
            return;
        }
    }

    private void UseNormalGrenade()
    {
        if (grenadeInventory == null)
            return;

        if (grenadeInventory.GetGrenadeCount(GrenadeType.Normal) <= 0)
            return;

        grenadeInventory.SelectNormalGrenade();
        grenadeInventory.ThrowNormalGrenade();

        AfterUsingItem();
    }

    private void UseMolotov()
    {
        if (grenadeInventory == null)
            return;

        if (grenadeInventory.GetGrenadeCount(GrenadeType.Molotov) <= 0)
            return;

        grenadeInventory.SelectMolotov();
        grenadeInventory.ThrowMolotov();

        AfterUsingItem();
    }

    private void UseJumpPlatform()
    {
        if (jumpPlatformInventory == null)
            return;

        if (jumpPlatformInventory.GetPlatformCount() <= 0)
            return;

        jumpPlatformInventory.PlacePlatform();

        AfterUsingItem();
    }

    private void UseHealthItem()
    {
        if (healthInventory == null)
            return;

        if (!healthInventory.HasHealthItem())
            return;

        StartCoroutine(UseHealthItemRoutine());
    }

    private IEnumerator UseHealthItemRoutine()
    {
        usingHealthItem = true;

        if (healthHandVisual != null)
            yield return StartCoroutine(healthHandVisual.DrinkAnimation());

        healthInventory.UseSelectedHealthItem();

        usingHealthItem = false;

        AfterUsingItem();
    }

    private void UseArmorItem()
    {
        if (usingArmorItem)
            return;

        if (armorInventory == null)
            return;

        if (!armorInventory.HasArmorItem())
            return;

        if (!armorInventory.CanUseSelectedArmor())
            return;

        StartCoroutine(UseArmorItemRoutine());
    }

    private IEnumerator UseArmorItemRoutine()
    {
        usingArmorItem = true;

        if (armorHandVisual != null)
            yield return StartCoroutine(armorHandVisual.PutArmorAnimation());

        bool used = armorInventory.UseSelectedArmor();

        usingArmorItem = false;

        if (used)
            AfterUsingItem();
        else
            RefreshVisuals();
    }

    private void AfterUsingItem()
    {
        if (IsSlotAvailable(selectedSlot))
            RefreshVisuals();
        else
            SelectFirstAvailableSlot();
    }

    public bool AddWeapon(GameObject weaponPrefab)
    {
        if (weaponPrefab == null)
            return false;

        if (GetWeaponCount() >= maxWeapons)
        {
            Debug.LogWarning("Inventory full. You can carry only 2 weapons.");
            return false;
        }

        GameObject newWeapon = Instantiate(weaponPrefab, transform);

        newWeapon.transform.localPosition = Vector3.zero;
        newWeapon.transform.localRotation = Quaternion.identity;
        newWeapon.transform.localScale = weaponPrefab.transform.localScale;

        PrepareWeapon(newWeapon);

        if (GetWeaponCount() <= 1)
            SelectSlot(QuickSlot.Weapon1);
        else
            SelectSlot(QuickSlot.Weapon2);

        return true;
    }

    private void PrepareWeapon(GameObject weaponObject)
    {
        Weapon weapon = weaponObject.GetComponent<Weapon>();

        if (weapon == null)
            return;

        weapon.fpsCamera = fpsCamera;
        weapon.carCamera = carCamera;
        weapon.scopeOverlay = scopeOverlay;
        weapon.normalCrosshairUI = normalCrosshairUI;
    }

    public void DropCurrentWeapon()
    {
        Weapon activeWeapon = GetActiveWeapon();

        if (activeWeapon == null)
            return;

        if (activeWeapon.pickupPrefab == null)
            return;

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

        Destroy(activeWeapon.gameObject);

        selectedWeaponIndex = 0;

        Invoke(nameof(SelectFirstAvailableSlot), 0.05f);
    }

    public Weapon GetActiveWeapon()
    {
        if (selectedSlot != QuickSlot.Weapon1 && selectedSlot != QuickSlot.Weapon2)
            return null;

        Weapon[] weapons = GetWeapons();

        if (weapons.Length <= 0)
            return null;

        selectedWeaponIndex = Mathf.Clamp(selectedWeaponIndex, 0, weapons.Length - 1);

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