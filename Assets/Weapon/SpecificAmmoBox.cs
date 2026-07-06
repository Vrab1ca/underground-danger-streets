using UnityEngine;

public class SpecificAmmoBox : MonoBehaviour
{
    [Header("Ammo Type")]
    public WeaponAmmoType ammoType = WeaponAmmoType.Rifle;
    public int amount = 30;

    [Header("Pickup")]
    public KeyCode pickupKey = KeyCode.E;
    public float pickupDistance = 3f;
    public bool destroyAfterPickup = true;

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
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject == null)
        {
            Debug.LogWarning(gameObject.name + " cannot find Player tag.");
            return;
        }

        player = playerObject.transform;
        weaponSwitcher = FindFirstObjectByType<WeaponSwitcher>();

        if (weaponSwitcher == null)
            Debug.LogWarning(gameObject.name + " cannot find WeaponSwitcher.");

        if (modelToAnimate == null)
            modelToAnimate = transform;

        startLocalPosition = modelToAnimate.localPosition;

        if (debugMessages)
            Debug.Log(gameObject.name + " ready. Ammo Type: " + ammoType + " Amount: " + amount);
    }

    private void Update()
    {
        AnimateBox();

        if (pickedUp)
            return;

        if (player == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > pickupDistance)
            return;

        if (Input.GetKeyDown(pickupKey))
        {
            PickupAmmo();
        }
    }

    private void AnimateBox()
    {
        if (modelToAnimate == null)
            return;

        modelToAnimate.Rotate(Vector3.up * rotateSpeed * Time.deltaTime, Space.World);

        Vector3 pos = startLocalPosition;
        pos.y += Mathf.Sin(Time.time * bobSpeed) * bobAmount;

        modelToAnimate.localPosition = pos;
    }

    private void PickupAmmo()
    {
        if (weaponSwitcher == null)
        {
            Debug.LogWarning("No WeaponSwitcher found.");
            return;
        }

        Weapon weapon = FindWeaponWithSameAmmoType();

        if (weapon == null)
        {
            Debug.LogWarning("You do not have weapon with ammo type: " + ammoType);
            return;
        }

        weapon.AddAmmo(amount);

        pickedUp = true;

        if (debugMessages)
        {
            Debug.Log(
                "PICKED AMMO BOX: " + ammoType +
                " | Added reserve ammo: +" + amount +
                " | Weapon: " + weapon.weaponName +
                " | Ammo now: " + weapon.AmmoInMagazine + " / " + weapon.ReserveAmmo
            );
        }

        if (destroyAfterPickup)
            Destroy(gameObject);
    }

    private Weapon FindWeaponWithSameAmmoType()
    {
        Weapon[] weapons = weaponSwitcher.GetComponentsInChildren<Weapon>(true);

        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] == null)
                continue;

            if (weapons[i].ammoType == ammoType)
                return weapons[i];
        }

        return null;
    }
}