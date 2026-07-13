using UnityEngine;

public class GrenadePickupBox : MonoBehaviour
{
    [Header("Grenade")]
    public GrenadeType grenadeType = GrenadeType.Normal;
    public int amount = 1;

    [Header("Pickup")]
    public KeyCode pickupKey = KeyCode.F;
    public float pickupDistance = 3f;
    public bool destroyAfterPickup = true;

    [Header("Optional Animation")]
    public Transform modelToAnimate;
    public float rotateSpeed = 70f;
    public float bobSpeed = 2f;
    public float bobAmount = 0.1f;

    private WeaponSwitcher weaponSwitcher;
    private Transform player;
    private Vector3 startLocalPosition;
    private bool pickedUp;

    private void Start()
    {
        FindPlayerAndInventory();

        if (modelToAnimate == null)
            modelToAnimate = transform;

        startLocalPosition = modelToAnimate.localPosition;
    }

    private void Update()
    {
        AnimatePickup();

        if (pickedUp)
            return;

        if (weaponSwitcher == null || player == null)
        {
            FindPlayerAndInventory();
            return;
        }

        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

        if (distance > pickupDistance)
            return;

        if (Input.GetKeyDown(pickupKey))
            TryPickup();
    }

    private void FindPlayerAndInventory()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;

            weaponSwitcher =
                playerObject.GetComponentInChildren<WeaponSwitcher>();
        }

        if (weaponSwitcher == null)
        {
            weaponSwitcher =
                FindFirstObjectByType<WeaponSwitcher>();
        }
    }

    private void TryPickup()
    {
        if (weaponSwitcher == null)
        {
            Debug.LogWarning(
                "GrenadePickupBox cannot find WeaponSwitcher."
            );

            return;
        }

        bool added = weaponSwitcher.TryAddGrenades(
            grenadeType,
            amount
        );

        if (!added)
        {
            Debug.Log(
                "Cannot pick up " +
                grenadeType +
                ". Hotbar or grenade inventory is full."
            );

            return;
        }

        pickedUp = true;

        Debug.Log(
            "Picked up " +
            amount +
            " " +
            grenadeType +
            "."
        );

        if (destroyAfterPickup)
            Destroy(gameObject);
    }

    private void AnimatePickup()
    {
        if (modelToAnimate == null)
            return;

        modelToAnimate.Rotate(
            Vector3.up * rotateSpeed * Time.deltaTime,
            Space.World
        );

        Vector3 position = startLocalPosition;

        position.y +=
            Mathf.Sin(Time.time * bobSpeed) *
            bobAmount;

        modelToAnimate.localPosition = position;
    }
}