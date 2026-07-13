using UnityEngine;

public class SpecificAmmoBox : MonoBehaviour
{
    [Header("Ammo Type")]
    public WeaponAmmoType ammoType =
        WeaponAmmoType.Rifle;

    public int amount = 30;

    [Header("Pickup")]
    public KeyCode pickupKey = KeyCode.E;
    public float pickupDistance = 3f;
    public bool destroyAfterPickup = true;

    [Header("Weapon Selection")]
    [Tooltip(
        "When enabled, ammo is first added to the currently selected weapon when its ammo type matches."
    )]
    public bool preferSelectedWeapon = true;

    [Header("Animation")]
    public Transform modelToAnimate;
    public float rotateSpeed = 90f;
    public float bobSpeed = 2f;
    public float bobAmount = 0.15f;

    [Header("Debug")]
    public bool debugMessages = true;

    private Transform player;
    private WeaponSwitcher weaponSwitcher;

    private Vector3 startLocalPosition;
    private bool pickedUp;

    private void Start()
    {
        FindPlayerAndWeaponSwitcher();

        if (modelToAnimate == null)
            modelToAnimate = transform;

        startLocalPosition =
            modelToAnimate.localPosition;

        if (debugMessages)
        {
            Debug.Log(
                gameObject.name +
                " ready. Ammo Type: " +
                ammoType +
                " | Amount: " +
                amount
            );
        }
    }

    private void Update()
    {
        AnimateBox();

        if (pickedUp)
            return;

        if (player == null ||
            weaponSwitcher == null)
        {
            FindPlayerAndWeaponSwitcher();
            return;
        }

        float distance =
            Vector3.Distance(
                transform.position,
                player.position
            );

        if (distance > pickupDistance)
            return;

        if (Input.GetKeyDown(pickupKey))
            PickupAmmo();
    }

    private void FindPlayerAndWeaponSwitcher()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject == null)
        {
            if (debugMessages)
            {
                Debug.LogWarning(
                    gameObject.name +
                    " cannot find an object with Player tag."
                );
            }

            return;
        }

        player = playerObject.transform;

        weaponSwitcher =
            playerObject.GetComponentInChildren<WeaponSwitcher>();

        if (weaponSwitcher == null)
        {
            weaponSwitcher =
                FindFirstObjectByType<WeaponSwitcher>();
        }

        if (weaponSwitcher == null &&
            debugMessages)
        {
            Debug.LogWarning(
                gameObject.name +
                " cannot find WeaponSwitcher."
            );
        }
    }

    private void AnimateBox()
    {
        if (modelToAnimate == null)
            return;

        modelToAnimate.Rotate(
            Vector3.up *
            rotateSpeed *
            Time.deltaTime,
            Space.World
        );

        Vector3 position =
            startLocalPosition;

        position.y +=
            Mathf.Sin(
                Time.time * bobSpeed
            ) * bobAmount;

        modelToAnimate.localPosition =
            position;
    }

    private void PickupAmmo()
    {
        if (weaponSwitcher == null)
        {
            Debug.LogWarning(
                "Cannot pick up ammo: WeaponSwitcher is missing."
            );

            return;
        }

        if (amount <= 0)
        {
            Debug.LogWarning(
                gameObject.name +
                " has an invalid ammo amount: " +
                amount
            );

            return;
        }

        Weapon weapon =
            FindWeaponWithSameAmmoType();

        if (weapon == null)
        {
            Debug.LogWarning(
                "You do not own a weapon using ammo type: " +
                ammoType
            );

            return;
        }

        weapon.AddAmmo(amount);

        pickedUp = true;

        if (debugMessages)
        {
            Debug.Log(
                "PICKED AMMO BOX: " +
                ammoType +
                " | Added reserve ammo: +" +
                amount +
                " | Weapon: " +
                weapon.weaponName +
                " | Ammo now: " +
                weapon.AmmoInMagazine +
                " / " +
                weapon.ReserveAmmo
            );
        }

        if (destroyAfterPickup)
            Destroy(gameObject);
    }

    private Weapon FindWeaponWithSameAmmoType()
    {
        // First try the currently selected weapon.
        if (preferSelectedWeapon)
        {
            Weapon selectedWeapon =
                weaponSwitcher.GetActiveWeapon();

            if (IsCorrectWeapon(selectedWeapon))
                return selectedWeapon;
        }

        // Search only weapons actually registered in the hotbar.
        int weaponCount =
            weaponSwitcher.InventoryWeaponCount;

        for (int i = 0;
             i < weaponCount;
             i++)
        {
            Weapon weapon =
                weaponSwitcher.GetInventoryWeapon(i);

            if (IsCorrectWeapon(weapon))
                return weapon;
        }

        return null;
    }

    private bool IsCorrectWeapon(Weapon weapon)
    {
        if (weapon == null)
            return false;

        // Melee weapons do not use ammunition.
        if (weapon.weaponMode ==
            Weapon.WeaponMode.Melee)
        {
            return false;
        }

        return weapon.ammoType == ammoType;
    }
}