using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    public enum HotbarItemType
    {
        Empty,
        Weapon,
        NormalGrenade,
        Molotov,
        JumpPlatform,
        Health,
        Armor,
        Flashlight,
        Battery
    }

    // Kept so older scripts that reference QuickSlot still compile.
    public enum QuickSlot
    {
        Hands,
        Weapon1,
        Weapon2,
        Weapon3,
        Weapon4,
        Weapon5,
        Weapon6,
        Weapon7,
        Weapon8,
        NormalGrenade,
        Molotov,
        JumpPlatform,
        HealthItem1,
        HealthItem2,
        HealthItem3,
        HealthItem4,
        ArmorItem,
        Flashlight,
        Battery
    }

    private class RuntimeHotbarSlot
    {
        public HotbarItemType itemType = HotbarItemType.Empty;
        public Weapon weapon;
        public HealthItemType healthType = HealthItemType.Small20;
        public ArmorItemType armorType = ArmorItemType.Strong100;
        public FlashlightBatteryType batteryType = FlashlightBatteryType.A;
    }

    [Header("Dynamic Hotbar")]
    [Range(1, 8)]
    public int slotCount = 5;

    [Tooltip("-1 means the separate fists/hands mode.")]
    public int selectedHotbarIndex = -1;

    [Header("Legacy Selected Value")]
    public QuickSlot selectedSlot = QuickSlot.Hands;

    [Header("Normal Hands - Not Inventory")]
    public GameObject handsVisual;

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

    [Header("Armor Pickup")]
    [Tooltip("When disabled, picking up armor adds it to a slot but keeps fists active.")]
    public bool autoSelectPickedArmor = false;

    [Header("Flashlight")]
    public PlayerFlashlightInventory flashlightInventory;

    [Header("Battery Hand Visuals - Optional")]
    [Tooltip("Optional model shown when an A battery slot is selected.")]
    public GameObject batteryAVisual;

    [Tooltip("Optional model shown when an AA battery slot is selected.")]
    public GameObject batteryAAVisual;

    [Tooltip("Optional model shown when an AAA battery slot is selected.")]
    public GameObject batteryAAAVisual;

    [Header("Battery Reload")]
    public KeyCode batteryUseKey = KeyCode.R;

    [Tooltip("When disabled, picking up a battery does not force-select it.")]
    public bool autoSelectPickedBattery = false;

    [Tooltip("After using a battery, automatically select the flashlight slot.")]
    public bool autoSelectFlashlightAfterReload = true;

    [Header("Controls")]
    public KeyCode handsKey = KeyCode.Q;
    public KeyCode dropKey = KeyCode.G;

    [Header("Drop Weapon")]
    public Transform dropPoint;

    [Header("Mouse Scroll")]
    public float scrollCooldown = 0.08f;

    [Header("Debug")]
    public bool debugHotbar = true;

    private readonly List<RuntimeHotbarSlot> slots =
        new List<RuntimeHotbarSlot>();

    private static readonly KeyCode[] NumberKeys =
    {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
        KeyCode.Alpha6,
        KeyCode.Alpha7,
        KeyCode.Alpha8
    };

    private float nextScrollTime;
    private bool usingHealthItem;
    private bool usingArmorItem;

    public int SlotCount
    {
        get
        {
            if (slots.Count > 0)
                return slots.Count;

            return Mathf.Clamp(slotCount, 1, 8);
        }
    }

    public bool HandsActive
    {
        get
        {
            if (selectedHotbarIndex < 0 ||
                selectedHotbarIndex >= slots.Count)
            {
                return true;
            }

            return slots[selectedHotbarIndex].itemType ==
                   HotbarItemType.Empty;
        }
    }

    public int InventoryWeaponCount
    {
        get { return GetWeaponCount(); }
    }

    private void OnValidate()
    {
        slotCount = Mathf.Clamp(slotCount, 1, 8);
    }

    private void Start()
    {
        AutoFindReferences();
        CreateEmptyRuntimeSlots();

        // Fists are separate from the inventory.
        SelectHands();
    }

    private void Update()
    {
        AutoFindReferences();

        if (usingHealthItem || usingArmorItem)
            return;

        HandleNumberKeys();
        HandleHandsKey();
        HandleScroll();
        HandleDropWeapon();
        HandleUseSelectedBattery();
        HandleUseSelectedItem();
    }

    private void CreateEmptyRuntimeSlots()
    {
        slots.Clear();

        int safeCount = Mathf.Clamp(slotCount, 1, 8);

        for (int i = 0; i < safeCount; i++)
            slots.Add(new RuntimeHotbarSlot());
    }

    private void AutoFindReferences()
    {
        if (fpsCamera == null)
            fpsCamera = Camera.main;

        if (grenadeInventory == null)
            grenadeInventory =
                FindFirstObjectByType<PlayerGrenadeInventory>();

        if (jumpPlatformInventory == null)
            jumpPlatformInventory =
                FindFirstObjectByType<JumpPlatformInventory>();

        if (healthInventory == null)
            healthInventory =
                FindFirstObjectByType<PlayerHealthInventory>();

        if (armorInventory == null)
            armorInventory =
                FindFirstObjectByType<PlayerArmorInventory>();

        if (flashlightInventory == null)
            flashlightInventory =
                FindFirstObjectByType<PlayerFlashlightInventory>();
    }

    // =========================================================
    // INPUT
    // =========================================================

    private void HandleNumberKeys()
    {
        int keyCount = Mathf.Min(slots.Count, NumberKeys.Length);

        for (int i = 0; i < keyCount; i++)
        {
            if (Input.GetKeyDown(NumberKeys[i]))
            {
                SelectHotbarIndex(i);
                return;
            }
        }
    }

    private void HandleHandsKey()
    {
        if (Input.GetKeyDown(handsKey))
            SelectHands();
    }

    private void HandleScroll()
    {
        if (Time.time < nextScrollTime)
            return;

        Weapon activeWeapon = GetActiveWeapon();

        if (activeWeapon != null &&
            activeWeapon.IsSniperZooming())
        {
            return;
        }

        float scroll = Input.mouseScrollDelta.y;

        if (Mathf.Abs(scroll) < 0.01f)
            scroll = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(scroll) < 0.01f)
            return;

        nextScrollTime = Time.time + scrollCooldown;

        int direction = scroll > 0f ? 1 : -1;
        ScrollHotbar(direction);
    }

    private void ScrollHotbar(int direction)
    {
        if (slots.Count <= 0)
        {
            SelectHands();
            return;
        }

        int nextIndex;

        if (selectedHotbarIndex < 0)
        {
            nextIndex = direction > 0
                ? 0
                : slots.Count - 1;
        }
        else
        {
            nextIndex = selectedHotbarIndex + direction;

            if (nextIndex < 0)
                nextIndex = slots.Count - 1;

            if (nextIndex >= slots.Count)
                nextIndex = 0;
        }

        SelectHotbarIndex(nextIndex);
    }

    // =========================================================
    // SELECTION
    // =========================================================

    public void SelectHands()
    {
        selectedHotbarIndex = -1;
        selectedSlot = QuickSlot.Hands;

        RefreshVisuals();

        if (debugHotbar)
            Debug.Log("Selected normal fists. Fists are not an inventory slot.");
    }

    public void SelectHotbarIndex(int index)
    {
        if (index < 0 || index >= slots.Count)
            return;

        bool selectedSameSlot =
            selectedHotbarIndex == index;

        selectedHotbarIndex = index;

        if (selectedSameSlot)
            CycleItemInsideSlot(slots[index].itemType);

        UpdateLegacySelectedSlot();
        RefreshVisuals();

        if (debugHotbar)
        {
            Debug.Log(
                "Selected hotbar Slot " +
                (index + 1) +
                ": " +
                GetSlotTitle(index)
            );
        }
    }

    private void CycleItemInsideSlot(HotbarItemType itemType)
    {
        // Health and armor do not cycle inside one slot.
        // Every collected health or armor item uses a separate slot.
    }

    private void UpdateLegacySelectedSlot()
    {
        if (HandsActive)
        {
            selectedSlot = QuickSlot.Hands;
            return;
        }

        RuntimeHotbarSlot slot = slots[selectedHotbarIndex];

        switch (slot.itemType)
        {
            case HotbarItemType.Weapon:
                selectedSlot = GetLegacyWeaponSlot(selectedHotbarIndex);
                break;

            case HotbarItemType.NormalGrenade:
                selectedSlot = QuickSlot.NormalGrenade;
                break;

            case HotbarItemType.Molotov:
                selectedSlot = QuickSlot.Molotov;
                break;

            case HotbarItemType.JumpPlatform:
                selectedSlot = QuickSlot.JumpPlatform;
                break;

            case HotbarItemType.Health:
                selectedSlot = GetLegacyHealthSlot();
                break;

            case HotbarItemType.Armor:
                selectedSlot = QuickSlot.ArmorItem;
                break;

            case HotbarItemType.Flashlight:
                selectedSlot = QuickSlot.Flashlight;
                break;

            case HotbarItemType.Battery:
                selectedSlot = QuickSlot.Battery;
                break;

            default:
                selectedSlot = QuickSlot.Hands;
                break;
        }
    }

    private QuickSlot GetLegacyWeaponSlot(int index)
    {
        switch (index)
        {
            case 0: return QuickSlot.Weapon1;
            case 1: return QuickSlot.Weapon2;
            case 2: return QuickSlot.Weapon3;
            case 3: return QuickSlot.Weapon4;
            case 4: return QuickSlot.Weapon5;
            case 5: return QuickSlot.Weapon6;
            case 6: return QuickSlot.Weapon7;
            case 7: return QuickSlot.Weapon8;
            default: return QuickSlot.Hands;
        }
    }

    private QuickSlot GetLegacyHealthSlot()
    {
        if (healthInventory == null)
            return QuickSlot.HealthItem1;

        switch (healthInventory.selectedIndex)
        {
            case 0: return QuickSlot.HealthItem1;
            case 1: return QuickSlot.HealthItem2;
            case 2: return QuickSlot.HealthItem3;
            case 3: return QuickSlot.HealthItem4;
            default: return QuickSlot.HealthItem1;
        }
    }

    // =========================================================
    // VISUALS
    // =========================================================

    private void RefreshVisuals()
    {
        HideEverything();

        if (HandsActive)
        {
            ShowHands();
            return;
        }

        RuntimeHotbarSlot slot = slots[selectedHotbarIndex];

        switch (slot.itemType)
        {
            case HotbarItemType.Weapon:
                if (slot.weapon != null)
                    slot.weapon.gameObject.SetActive(true);
                break;

            case HotbarItemType.NormalGrenade:
                if (normalGrenadeVisual != null)
                    normalGrenadeVisual.SetActive(true);

                if (grenadeInventory != null)
                    grenadeInventory.SelectNormalGrenade();
                break;

            case HotbarItemType.Molotov:
                if (molotovVisual != null)
                    molotovVisual.SetActive(true);

                if (grenadeInventory != null)
                    grenadeInventory.SelectMolotov();
                break;

            case HotbarItemType.JumpPlatform:
                if (jumpPlatformVisual != null)
                    jumpPlatformVisual.SetActive(true);
                break;

            case HotbarItemType.Health:
                ShowHealthItem();
                break;

            case HotbarItemType.Armor:
                ShowArmorItem();
                break;

            case HotbarItemType.Battery:
                ShowBatteryItem();
                break;
        }
    }

    private void HideEverything()
    {
        if (handsVisual != null)
            handsVisual.SetActive(false);

        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].weapon != null)
                slots[i].weapon.gameObject.SetActive(false);
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

        if (batteryAVisual != null)
            batteryAVisual.SetActive(false);

        if (batteryAAVisual != null)
            batteryAAVisual.SetActive(false);

        if (batteryAAAVisual != null)
            batteryAAAVisual.SetActive(false);
    }

    private void ShowHands()
    {
        if (handsVisual != null)
            handsVisual.SetActive(true);
        else
            Debug.LogWarning(
                "WeaponSwitcher: assign PlayerHands to Hands Visual."
            );
    }

    private void ShowHealthItem()
    {
        if (selectedHotbarIndex < 0 ||
            selectedHotbarIndex >= slots.Count)
        {
            return;
        }

        RuntimeHotbarSlot slot = slots[selectedHotbarIndex];

        if (slot.itemType != HotbarItemType.Health)
            return;

        if (healthHandVisual != null)
        {
            healthHandVisual.ShowItem(slot.healthType);
            return;
        }

        if (healthItemVisual != null)
            healthItemVisual.SetActive(true);
    }

    private void ShowArmorItem()
    {
        if (selectedHotbarIndex < 0 ||
            selectedHotbarIndex >= slots.Count)
        {
            return;
        }

        RuntimeHotbarSlot slot = slots[selectedHotbarIndex];

        if (slot.itemType != HotbarItemType.Armor)
            return;

        if (armorHandVisual != null)
        {
            DisablePhysicsOnHeldVisual(armorHandVisual.gameObject);
            armorHandVisual.ShowArmor(slot.armorType);
            return;
        }

        if (armorVisual != null)
        {
            DisablePhysicsOnHeldVisual(armorVisual);
            armorVisual.SetActive(true);
        }
    }

    private void ShowBatteryItem()
    {
        if (selectedHotbarIndex < 0 ||
            selectedHotbarIndex >= slots.Count)
        {
            return;
        }

        RuntimeHotbarSlot slot = slots[selectedHotbarIndex];

        if (slot.itemType != HotbarItemType.Battery)
            return;

        GameObject selectedVisual = null;

        switch (slot.batteryType)
        {
            case FlashlightBatteryType.A:
                selectedVisual = batteryAVisual;
                break;

            case FlashlightBatteryType.AA:
                selectedVisual = batteryAAVisual;
                break;

            case FlashlightBatteryType.AAA:
                selectedVisual = batteryAAAVisual;
                break;
        }

        if (selectedVisual == null)
        {
            // The battery still works without a hand model.
            ShowHands();
            return;
        }

        DisablePhysicsOnHeldVisual(selectedVisual);
        selectedVisual.SetActive(true);
    }

    private void DisablePhysicsOnHeldVisual(GameObject visualRoot)
    {
        if (visualRoot == null)
            return;

        Collider[] colliders =
            visualRoot.GetComponentsInChildren<Collider>(true);

        for (int i = 0; i < colliders.Length; i++)
            colliders[i].enabled = false;

        Rigidbody[] rigidbodies =
            visualRoot.GetComponentsInChildren<Rigidbody>(true);

        for (int i = 0; i < rigidbodies.Length; i++)
        {
            rigidbodies[i].useGravity = false;
            rigidbodies[i].isKinematic = true;
            rigidbodies[i].detectCollisions = false;
        }
    }

    // =========================================================
    // USE SELECTED ITEM
    // =========================================================

    private void HandleUseSelectedBattery()
    {
        if (!Input.GetKeyDown(batteryUseKey))
            return;

        if (HandsActive ||
            selectedHotbarIndex < 0 ||
            selectedHotbarIndex >= slots.Count)
        {
            return;
        }

        RuntimeHotbarSlot slot = slots[selectedHotbarIndex];

        if (slot.itemType != HotbarItemType.Battery)
            return;

        if (flashlightInventory == null)
        {
            Debug.LogWarning("PlayerFlashlightInventory is missing.");
            return;
        }

        int usedBatterySlot = selectedHotbarIndex;

        bool installed = flashlightInventory.InstallBattery(
            slot.batteryType
        );

        if (!installed)
            return;

        ClearSlot(usedBatterySlot);

        int flashlightSlot = FindSlotWithType(
            HotbarItemType.Flashlight
        );

        if (autoSelectFlashlightAfterReload &&
            flashlightSlot >= 0)
        {
            SelectHotbarIndex(flashlightSlot);
        }
        else
        {
            SelectHands();
        }

        if (debugHotbar)
        {
            Debug.Log(
                "Battery used from Slot " +
                (usedBatterySlot + 1) +
                ". Flashlight charge: " +
                Mathf.CeilToInt(
                    flashlightInventory.CurrentCharge
                ) + "%"
            );
        }
    }

    private void HandleUseSelectedItem()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        if (HandsActive)
            return;

        RuntimeHotbarSlot slot = slots[selectedHotbarIndex];

        // Weapon.cs handles guns and melee weapons.
        if (slot.itemType == HotbarItemType.Weapon)
            return;

        switch (slot.itemType)
        {
            case HotbarItemType.NormalGrenade:
                UseNormalGrenade();
                break;

            case HotbarItemType.Molotov:
                UseMolotov();
                break;

            case HotbarItemType.JumpPlatform:
                UseJumpPlatform();
                break;

            case HotbarItemType.Health:
                UseHealthItem();
                break;

            case HotbarItemType.Armor:
                UseArmorItem();
                break;
        }
    }

    private void UseNormalGrenade()
    {
        if (grenadeInventory == null)
            return;

        grenadeInventory.SelectNormalGrenade();
        grenadeInventory.ThrowNormalGrenade();

        RemoveStackableSlotIfEmpty(
            selectedHotbarIndex,
            HotbarItemType.NormalGrenade
        );
    }

    private void UseMolotov()
    {
        if (grenadeInventory == null)
            return;

        grenadeInventory.SelectMolotov();
        grenadeInventory.ThrowMolotov();

        RemoveStackableSlotIfEmpty(
            selectedHotbarIndex,
            HotbarItemType.Molotov
        );
    }

    private void UseJumpPlatform()
    {
        if (jumpPlatformInventory == null)
            return;

        jumpPlatformInventory.PlacePlatform();

        RemoveStackableSlotIfEmpty(
            selectedHotbarIndex,
            HotbarItemType.JumpPlatform
        );
    }

    private void UseHealthItem()
    {
        if (healthInventory == null ||
            selectedHotbarIndex < 0 ||
            selectedHotbarIndex >= slots.Count)
        {
            return;
        }

        RuntimeHotbarSlot slot = slots[selectedHotbarIndex];

        if (slot.itemType != HotbarItemType.Health)
            return;

        int inventoryIndex = FindHealthInventoryIndex(slot.healthType);

        if (inventoryIndex < 0)
        {
            Debug.LogWarning("Selected health item is missing from PlayerHealthInventory.");
            ClearSlot(selectedHotbarIndex);
            UpdateLegacySelectedSlot();
            RefreshVisuals();
            return;
        }

        healthInventory.selectedIndex = inventoryIndex;

        StartCoroutine(UseHealthItemRoutine(selectedHotbarIndex));
    }

    private IEnumerator UseHealthItemRoutine(int slotIndex)
    {
        usingHealthItem = true;

        if (healthHandVisual != null)
        {
            yield return StartCoroutine(
                healthHandVisual.DrinkAnimation()
            );
        }

        bool used = healthInventory.UseSelectedHealthItem();

        usingHealthItem = false;

        if (used)
        {
            ClearSlot(slotIndex);
            UpdateLegacySelectedSlot();
        }

        RefreshVisuals();
    }

    private int FindHealthInventoryIndex(HealthItemType healthType)
    {
        if (healthInventory == null ||
            healthInventory.healthItems == null)
        {
            return -1;
        }

        for (int i = 0; i < healthInventory.healthItems.Count; i++)
        {
            if (healthInventory.healthItems[i] == healthType)
                return i;
        }

        return -1;
    }

    private void UseArmorItem()
    {
        if (armorInventory == null ||
            selectedHotbarIndex < 0 ||
            selectedHotbarIndex >= slots.Count)
        {
            return;
        }

        RuntimeHotbarSlot slot = slots[selectedHotbarIndex];

        if (slot.itemType != HotbarItemType.Armor)
            return;

        int inventoryIndex = FindArmorInventoryIndex(slot.armorType);

        if (inventoryIndex < 0)
        {
            Debug.LogWarning("Selected armor is missing from PlayerArmorInventory.");
            ClearSlot(selectedHotbarIndex);
            UpdateLegacySelectedSlot();
            RefreshVisuals();
            return;
        }

        armorInventory.selectedIndex = inventoryIndex;

        if (!armorInventory.CanUseSelectedArmor())
            return;

        StartCoroutine(UseArmorItemRoutine(selectedHotbarIndex));
    }

    private IEnumerator UseArmorItemRoutine(int slotIndex)
    {
        usingArmorItem = true;

        if (armorHandVisual != null)
        {
            yield return StartCoroutine(
                armorHandVisual.PutArmorAnimation()
            );
        }

        bool used = armorInventory.UseSelectedArmor();

        usingArmorItem = false;

        if (used)
        {
            ClearSlot(slotIndex);
            UpdateLegacySelectedSlot();
        }

        RefreshVisuals();
    }

    private int FindArmorInventoryIndex(ArmorItemType armorType)
    {
        if (armorInventory == null ||
            armorInventory.armorItems == null)
        {
            return -1;
        }

        for (int i = 0; i < armorInventory.armorItems.Count; i++)
        {
            if (armorInventory.armorItems[i] == armorType)
                return i;
        }

        return -1;
    }

    private void RemoveStackableSlotIfEmpty(
        int slotIndex,
        HotbarItemType expectedType
    )
    {
        if (slotIndex < 0 || slotIndex >= slots.Count)
            return;

        if (slots[slotIndex].itemType != expectedType)
            return;

        if (GetStoredCount(expectedType) <= 0)
            ClearSlot(slotIndex);

        UpdateLegacySelectedSlot();
        RefreshVisuals();
    }

    private int GetStoredCount(HotbarItemType itemType)
    {
        switch (itemType)
        {
            case HotbarItemType.NormalGrenade:
                return grenadeInventory == null
                    ? 0
                    : grenadeInventory.GetGrenadeCount(
                        GrenadeType.Normal
                    );

            case HotbarItemType.Molotov:
                return grenadeInventory == null
                    ? 0
                    : grenadeInventory.GetGrenadeCount(
                        GrenadeType.Molotov
                    );

            case HotbarItemType.JumpPlatform:
                return jumpPlatformInventory == null
                    ? 0
                    : jumpPlatformInventory.GetPlatformCount();

            case HotbarItemType.Health:
                return healthInventory == null
                    ? 0
                    : healthInventory.GetItemCount();

            case HotbarItemType.Armor:
                return armorInventory == null
                    ? 0
                    : armorInventory.GetItemCount();

            default:
                return 0;
        }
    }

    // =========================================================
    // ADD STACKABLE ITEMS
    // =========================================================

    public bool CanStoreItem(HotbarItemType itemType)
    {
        if (itemType == HotbarItemType.Empty)
            return false;

        // Every weapon, health item and armor item uses one separate slot.
        if (itemType == HotbarItemType.Weapon ||
            itemType == HotbarItemType.Health ||
            itemType == HotbarItemType.Armor ||
            itemType == HotbarItemType.Flashlight ||
            itemType == HotbarItemType.Battery)
        {
            return FindPreferredEmptySlot() >= 0;
        }

        // Grenades, Molotovs and platforms can stack in one slot.
        if (FindSlotWithType(itemType) >= 0)
            return true;

        return FindPreferredEmptySlot() >= 0;
    }

    public bool RegisterStackableItem(
        HotbarItemType itemType,
        bool selectItem = true
    )
    {
        if (itemType == HotbarItemType.Empty ||
            itemType == HotbarItemType.Weapon ||
            itemType == HotbarItemType.Health ||
            itemType == HotbarItemType.Armor ||
            itemType == HotbarItemType.Flashlight ||
            itemType == HotbarItemType.Battery)
        {
            return false;
        }

        int existingIndex = FindSlotWithType(itemType);

        if (existingIndex >= 0)
        {
            if (selectItem)
                SelectHotbarIndex(existingIndex);

            return true;
        }

        int emptyIndex = FindPreferredEmptySlot();

        if (emptyIndex < 0)
        {
            Debug.Log("Hotbar full. Cannot add " + itemType + ".");
            return false;
        }

        slots[emptyIndex].itemType = itemType;
        slots[emptyIndex].weapon = null;

        if (selectItem)
            SelectHotbarIndex(emptyIndex);

        return true;
    }

    public bool TryAddGrenades(
        GrenadeType grenadeType,
        int amount
    )
    {
        if (grenadeInventory == null)
            return false;

        HotbarItemType hotbarType =
            grenadeType == GrenadeType.Normal
                ? HotbarItemType.NormalGrenade
                : HotbarItemType.Molotov;

        if (!CanStoreItem(hotbarType))
        {
            Debug.Log("Hotbar full. Cannot pick up " + grenadeType);
            return false;
        }

        int before = grenadeInventory.GetGrenadeCount(grenadeType);
        grenadeInventory.AddGrenade(grenadeType, amount);
        int after = grenadeInventory.GetGrenadeCount(grenadeType);

        if (after <= before)
            return false;

        RegisterStackableItem(hotbarType);
        return true;
    }

    public bool TryAddHealthItem(HealthItemType itemType)
    {
        if (healthInventory == null)
        {
            Debug.LogWarning("PlayerHealthInventory is missing.");
            return false;
        }

        int emptyIndex = FindPreferredEmptySlot();

        if (emptyIndex < 0)
        {
            Debug.Log("Hotbar full. Cannot pick up health item.");
            return false;
        }

        bool added = healthInventory.AddHealthItem(itemType);

        if (!added)
            return false;

        slots[emptyIndex].itemType = HotbarItemType.Health;
        slots[emptyIndex].weapon = null;
        slots[emptyIndex].healthType = itemType;

        SelectHotbarIndex(emptyIndex);
        return true;
    }

    public bool TryAddJumpPlatforms(int amount)
    {
        if (jumpPlatformInventory == null)
            return false;

        if (!CanStoreItem(HotbarItemType.JumpPlatform))
        {
            Debug.Log("Hotbar full. Cannot pick up jump platform.");
            return false;
        }

        int before = jumpPlatformInventory.GetPlatformCount();
        jumpPlatformInventory.AddPlatforms(amount);
        int after = jumpPlatformInventory.GetPlatformCount();

        if (after <= before)
            return false;

        RegisterStackableItem(HotbarItemType.JumpPlatform);
        return true;
    }

    // Call this AFTER your existing armor pickup successfully
    // adds an armor item to PlayerArmorInventory.
    public bool TryAddArmorItem(ArmorItemType armorType)
    {
        if (armorInventory == null)
        {
            Debug.LogWarning("PlayerArmorInventory is missing.");
            return false;
        }

        int emptyIndex = FindPreferredEmptySlot();

        if (emptyIndex < 0)
        {
            Debug.Log("Hotbar full. Cannot pick up armor.");
            return false;
        }

        bool added = armorInventory.AddArmorItem(armorType);

        if (!added)
            return false;

        slots[emptyIndex].itemType = HotbarItemType.Armor;
        slots[emptyIndex].weapon = null;
        slots[emptyIndex].armorType = armorType;

        if (autoSelectPickedArmor)
            SelectHotbarIndex(emptyIndex);
        else
            SelectHands();

        if (debugHotbar)
        {
            Debug.Log(
                "Armor added to Slot " +
                (emptyIndex + 1) +
                ": " + armorType
            );
        }

        return true;
    }

    public bool TryAddFlashlight(float startingCharge = 0f)
    {
        if (flashlightInventory == null)
        {
            Debug.LogWarning("PlayerFlashlightInventory is missing.");
            return false;
        }

        if (flashlightInventory.HasFlashlight)
        {
            Debug.Log("You already have a flashlight.");
            return false;
        }

        int emptyIndex = FindPreferredEmptySlot();

        if (emptyIndex < 0)
        {
            Debug.Log("Hotbar full. Cannot pick up flashlight.");
            return false;
        }

        bool added = flashlightInventory.AddFlashlight(startingCharge);

        if (!added)
            return false;

        slots[emptyIndex].itemType = HotbarItemType.Flashlight;
        slots[emptyIndex].weapon = null;

        SelectHotbarIndex(emptyIndex);

        if (debugHotbar)
        {
            Debug.Log(
                "Flashlight added to Slot " +
                (emptyIndex + 1)
            );
        }

        return true;
    }

    public bool TryAddBattery(FlashlightBatteryType batteryType)
    {
        int emptyIndex = FindPreferredEmptySlot();

        if (emptyIndex < 0)
        {
            Debug.Log(
                "Hotbar full. Cannot pick up " +
                batteryType + " battery."
            );
            return false;
        }

        bool batteryWasPlacedInSelectedEmptySlot =
            selectedHotbarIndex == emptyIndex;

        slots[emptyIndex].itemType = HotbarItemType.Battery;
        slots[emptyIndex].weapon = null;
        slots[emptyIndex].batteryType = batteryType;

        if (autoSelectPickedBattery)
        {
            SelectHotbarIndex(emptyIndex);
        }
        else if (batteryWasPlacedInSelectedEmptySlot)
        {
            SelectHands();
        }
        else
        {
            UpdateLegacySelectedSlot();
            RefreshVisuals();
        }

        if (debugHotbar)
        {
            Debug.Log(
                batteryType +
                " battery added to Slot " +
                (emptyIndex + 1)
            );
        }

        return true;
    }

    // Compatibility with older pickup scripts that first add to
    // PlayerArmorInventory and then register the hotbar slot.
    public bool RegisterArmorAfterPickup()
    {
        if (armorInventory == null ||
            armorInventory.armorItems == null ||
            armorInventory.armorItems.Count <= 0)
        {
            return false;
        }

        int emptyIndex = FindPreferredEmptySlot();

        if (emptyIndex < 0)
            return false;

        int registeredArmorSlots = 0;

        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].itemType == HotbarItemType.Armor)
                registeredArmorSlots++;
        }

        int inventoryIndex = Mathf.Clamp(
            registeredArmorSlots,
            0,
            armorInventory.armorItems.Count - 1
        );

        slots[emptyIndex].itemType = HotbarItemType.Armor;
        slots[emptyIndex].weapon = null;
        slots[emptyIndex].armorType =
            armorInventory.armorItems[inventoryIndex];

        return true;
    }

    // =========================================================
    // ADD / DROP WEAPONS
    // =========================================================

    public bool AddWeapon(GameObject weaponPrefab)
    {
        if (weaponPrefab == null)
            return false;

        int emptyIndex = FindPreferredEmptySlot();

        if (emptyIndex < 0)
        {
            Debug.LogWarning("Hotbar full. Cannot pick up weapon.");
            return false;
        }

        GameObject newWeapon = Instantiate(weaponPrefab, transform);

        newWeapon.transform.localPosition =
            weaponPrefab.transform.localPosition;

        newWeapon.transform.localRotation =
            weaponPrefab.transform.localRotation;

        newWeapon.transform.localScale =
            weaponPrefab.transform.localScale;

        Weapon weapon = newWeapon.GetComponent<Weapon>();

        if (weapon == null)
        {
            Debug.LogWarning(
                newWeapon.name +
                " does not have Weapon.cs on its root."
            );

            Destroy(newWeapon);
            return false;
        }

        PrepareWeapon(weapon);

        slots[emptyIndex].itemType = HotbarItemType.Weapon;
        slots[emptyIndex].weapon = weapon;

        newWeapon.SetActive(false);

        SelectHotbarIndex(emptyIndex);
        return true;
    }

    private void PrepareWeapon(Weapon weapon)
    {
        weapon.fpsCamera = fpsCamera;
        weapon.carCamera = carCamera;
        weapon.scopeOverlay = scopeOverlay;
        weapon.normalCrosshairUI = normalCrosshairUI;
    }

    private void HandleDropWeapon()
    {
        if (!Input.GetKeyDown(dropKey))
            return;

        DropCurrentWeapon();
    }

    public void DropCurrentWeapon()
    {
        Weapon activeWeapon = GetActiveWeapon();

        if (activeWeapon == null)
            return;

        if (activeWeapon.pickupPrefab == null)
        {
            Debug.LogWarning(
                activeWeapon.weaponName +
                " has no Pickup Prefab."
            );

            return;
        }

        Vector3 spawnPosition =
            dropPoint != null
                ? dropPoint.position
                : transform.position + transform.forward * 2f;

        GameObject droppedPickup = Instantiate(
            activeWeapon.pickupPrefab,
            spawnPosition,
            Quaternion.identity
        );

        droppedPickup.SetActive(true);

        int oldSlotIndex = selectedHotbarIndex;

        ClearSlot(oldSlotIndex);
        Destroy(activeWeapon.gameObject);

        UpdateLegacySelectedSlot();
        RefreshVisuals();
    }

    // =========================================================
    // SLOT HELPERS
    // =========================================================

    private int FindPreferredEmptySlot()
    {
        // Select an empty slot first, then pick an item to place it there.
        if (selectedHotbarIndex >= 0 &&
            selectedHotbarIndex < slots.Count &&
            slots[selectedHotbarIndex].itemType == HotbarItemType.Empty)
        {
            return selectedHotbarIndex;
        }

        return FindFirstEmptySlot();
    }

    private int FindFirstEmptySlot()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].itemType == HotbarItemType.Empty)
                return i;
        }

        return -1;
    }

    private int FindSlotWithType(HotbarItemType itemType)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].itemType == itemType)
                return i;
        }

        return -1;
    }

    private void ClearSlot(int index)
    {
        if (index < 0 || index >= slots.Count)
            return;

        slots[index].itemType = HotbarItemType.Empty;
        slots[index].weapon = null;
        slots[index].healthType = HealthItemType.Small20;
        slots[index].armorType = ArmorItemType.Strong100;
        slots[index].batteryType = FlashlightBatteryType.A;
    }

    // =========================================================
    // PUBLIC DATA FOR HUD AND OTHER SCRIPTS
    // =========================================================

    public HotbarItemType GetSlotItemType(int index)
    {
        if (index < 0 || index >= slots.Count)
            return HotbarItemType.Empty;

        return slots[index].itemType;
    }

    public bool IsSlotFilled(int index)
    {
        return GetSlotItemType(index) != HotbarItemType.Empty;
    }

    public bool IsSlotSelected(int index)
    {
        return selectedHotbarIndex == index;
    }

    public string GetSlotTitle(int index)
    {
        if (index < 0 || index >= slots.Count)
            return "EMPTY";

        RuntimeHotbarSlot slot = slots[index];

        switch (slot.itemType)
        {
            case HotbarItemType.Weapon:
                if (slot.weapon == null ||
                    string.IsNullOrEmpty(slot.weapon.weaponName))
                {
                    return "WEAPON";
                }

                return slot.weapon.weaponName.ToUpper();

            case HotbarItemType.NormalGrenade:
                return "GRENADE";

            case HotbarItemType.Molotov:
                return "MOLOTOV";

            case HotbarItemType.JumpPlatform:
                return "PLATFORM";

            case HotbarItemType.Health:
                return slot.healthType.ToString().ToUpper();

            case HotbarItemType.Armor:
                return slot.armorType.ToString().ToUpper();

            case HotbarItemType.Flashlight:
                return "FLASHLIGHT";

            case HotbarItemType.Battery:
                return slot.batteryType.ToString().ToUpper() +
                       " BATTERY";

            default:
                return "EMPTY";
        }
    }

    public string GetSlotCountText(int index)
    {
        if (index < 0 || index >= slots.Count)
            return "";

        HotbarItemType itemType = slots[index].itemType;

        if (itemType == HotbarItemType.Weapon ||
            itemType == HotbarItemType.Empty)
        {
            return "";
        }

        if (itemType == HotbarItemType.Flashlight)
        {
            if (flashlightInventory == null)
                return "0%";

            return Mathf.CeilToInt(
                flashlightInventory.CurrentCharge
            ) + "%";
        }

        if (itemType == HotbarItemType.Health ||
            itemType == HotbarItemType.Armor ||
            itemType == HotbarItemType.Battery)
        {
            return "x1";
        }

        return "x" + GetStoredCount(itemType);
    }

    public bool IsFlashlightSelected()
    {
        if (selectedHotbarIndex < 0 ||
            selectedHotbarIndex >= slots.Count)
        {
            return false;
        }

        return slots[selectedHotbarIndex].itemType ==
               HotbarItemType.Flashlight;
    }

    public bool IsBatterySelected()
    {
        if (selectedHotbarIndex < 0 ||
            selectedHotbarIndex >= slots.Count)
        {
            return false;
        }

        return slots[selectedHotbarIndex].itemType ==
               HotbarItemType.Battery;
    }

    public FlashlightBatteryType GetSlotBatteryType(int index)
    {
        if (index < 0 || index >= slots.Count)
            return FlashlightBatteryType.A;

        return slots[index].batteryType;
    }

    public string GetSelectedItemTitle()
    {
        if (HandsActive)
            return "HANDS";

        return GetSlotTitle(selectedHotbarIndex);
    }

    public Weapon GetActiveWeapon()
    {
        if (HandsActive)
            return null;

        RuntimeHotbarSlot slot = slots[selectedHotbarIndex];

        if (slot.itemType != HotbarItemType.Weapon)
            return null;

        return slot.weapon;
    }

    // Returns the Nth weapon, not the Nth hotbar slot.
    // Kept for compatibility with older HUD/ammo scripts.
    public Weapon GetInventoryWeapon(int weaponIndex)
    {
        List<Weapon> weapons = GetWeaponsInHotbarOrder();

        if (weaponIndex < 0 || weaponIndex >= weapons.Count)
            return null;

        return weapons[weaponIndex];
    }

    public int GetWeaponCount()
    {
        return GetWeaponsInHotbarOrder().Count;
    }

    private List<Weapon> GetWeaponsInHotbarOrder()
    {
        List<Weapon> weapons = new List<Weapon>();

        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].itemType == HotbarItemType.Weapon &&
                slots[i].weapon != null)
            {
                weapons.Add(slots[i].weapon);
            }
        }

        return weapons;
    }
}
