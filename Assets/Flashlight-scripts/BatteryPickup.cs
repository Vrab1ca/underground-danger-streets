using UnityEngine;

public class BatteryPickup : MonoBehaviour
{
    [Header("Battery")]
    public FlashlightBatteryType batteryType =
        FlashlightBatteryType.A;

    [Header("Pickup")]
    public KeyCode pickupKey = KeyCode.E;

    [Min(0.1f)]
    public float pickupDistance = 3f;

    [Header("Case Objects")]
    [Tooltip("The battery model visible inside the small case.")]
    public GameObject batteryObjectInCase;

    [Tooltip("Enable only when the complete case should disappear.")]
    public bool destroyWholeCaseAfterPickup = false;

    [Header("Debug")]
    public bool debugMessages = true;

    private Transform player;
    private WeaponSwitcher weaponSwitcher;
    private bool collected;

    private void Start()
    {
        FindPlayerReferences();
    }

    private void Update()
    {
        if (collected)
            return;

        if (player == null || weaponSwitcher == null)
        {
            FindPlayerReferences();
            return;
        }

        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

        if (distance > pickupDistance)
            return;

        if (Input.GetKeyDown(pickupKey))
            TryPickupBattery();
    }

    private void FindPlayerReferences()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject == null)
            return;

        player = playerObject.transform;

        weaponSwitcher =
            playerObject.GetComponentInChildren
                <WeaponSwitcher>(true);

        if (weaponSwitcher == null)
        {
            weaponSwitcher =
                FindFirstObjectByType<WeaponSwitcher>();
        }
    }

    private void TryPickupBattery()
    {
        if (weaponSwitcher == null)
        {
            Debug.LogWarning(
                "BatteryPickup cannot find WeaponSwitcher."
            );
            return;
        }

        bool added =
            weaponSwitcher.TryAddBattery(batteryType);

        if (!added)
            return;

        collected = true;

        if (batteryObjectInCase != null)
            batteryObjectInCase.SetActive(false);

        if (debugMessages)
        {
            Debug.Log(
                batteryType +
                " battery picked up into a separate hotbar slot."
            );
        }

        if (destroyWholeCaseAfterPickup)
        {
            Destroy(gameObject);
        }
        else
        {
            enabled = false;
        }
    }
}
