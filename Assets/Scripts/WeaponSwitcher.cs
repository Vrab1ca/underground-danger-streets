using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    public enum QuickSlot
    {
        Weapon1,
        Weapon2,
        NormalGrenade,
        Molotov
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

    [Header("Drop")]
    public Transform dropPoint;
    public KeyCode dropKey = KeyCode.G;

    private int selectedWeaponIndex = 0;

    private void Start()
    {
        SelectSlot(QuickSlot.Weapon1);
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
    }

    private void HandleNumberKeys()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SelectSlot(QuickSlot.Weapon1);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            SelectSlot(QuickSlot.Weapon2);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            SelectSlot(QuickSlot.NormalGrenade);

        if (Input.GetKeyDown(KeyCode.Alpha4))
            SelectSlot(QuickSlot.Molotov);
    }

    private void HandleScroll()
    {
        Weapon activeWeapon = GetActiveWeapon();

        if (activeWeapon != null && activeWeapon.IsSniperZooming())
            return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f)
            NextSlot();

        if (scroll < 0f)
            PreviousSlot();
    }

    private void NextSlot()
    {
        int slot = (int)selectedSlot;
        slot++;

        if (slot > 3)
            slot = 0;

        SelectSlot((QuickSlot)slot);
    }

    private void PreviousSlot()
    {
        int slot = (int)selectedSlot;
        slot--;

        if (slot < 0)
            slot = 3;

        SelectSlot((QuickSlot)slot);
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
        UpdateGrenadeVisuals();

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
                i == selectedWeaponIndex;

            weapons[i].gameObject.SetActive(shouldShow);
        }
    }

    private void UpdateGrenadeVisuals()
    {
        if (normalGrenadeVisual != null)
            normalGrenadeVisual.SetActive(selectedSlot == QuickSlot.NormalGrenade);

        if (molotovVisual != null)
            molotovVisual.SetActive(selectedSlot == QuickSlot.Molotov);
    }

    private void HandleUseSelectedItem()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        if (selectedSlot == QuickSlot.NormalGrenade)
        {
            if (grenadeInventory != null)
            {
                grenadeInventory.SelectNormalGrenade();
                grenadeInventory.ThrowSelectedGrenade();
            }

            return;
        }

        if (selectedSlot == QuickSlot.Molotov)
        {
            if (grenadeInventory != null)
            {
                grenadeInventory.SelectMolotov();
                grenadeInventory.ThrowSelectedGrenade();
            }

            return;
        }
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

        SimpleADS ads = weaponObject.GetComponent<SimpleADS>();

        if (ads != null)
        {
            ads.fpsCamera = fpsCamera;
            ads.carCamera = carCamera;
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
        SelectSlot(QuickSlot.Weapon1);
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
        System.Collections.Generic.List<Weapon> weapons = new System.Collections.Generic.List<Weapon>();

        for (int i = 0; i < transform.childCount; i++)
        {
            Weapon weapon = transform.GetChild(i).GetComponent<Weapon>();

            if (weapon != null)
                weapons.Add(weapon);
        }

        return weapons.ToArray();
    }
}