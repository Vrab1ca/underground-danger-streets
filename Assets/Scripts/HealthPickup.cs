using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [Header("Health Item")]
    public HealthItemType itemType = HealthItemType.Small20;

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

    private WeaponSwitcher weaponSwitcher;
    private Transform player;
    private Vector3 startLocalPosition;
    private bool pickedUp;

    private void Start()
    {
        FindReferences();

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
            FindReferences();
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > pickupDistance)
            return;

        if (Input.GetKeyDown(pickupKey))
            TryPickup();
    }

    private void FindReferences()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject == null)
            return;

        player = playerObject.transform;
        weaponSwitcher = playerObject.GetComponentInChildren<WeaponSwitcher>(true);

        if (weaponSwitcher == null)
            weaponSwitcher = playerObject.GetComponentInParent<WeaponSwitcher>();

        if (weaponSwitcher == null)
            weaponSwitcher = FindFirstObjectByType<WeaponSwitcher>();
    }

    private void TryPickup()
    {
        if (weaponSwitcher == null)
        {
            Debug.LogWarning("HealthPickup cannot find WeaponSwitcher.");
            return;
        }

        // WeaponSwitcher puts this exact health item into one new empty slot.
        bool added = weaponSwitcher.TryAddHealthItem(itemType);

        if (!added)
        {
            if (debugMessages)
                Debug.Log("Cannot pick up health item. No empty hotbar slot.");

            return;
        }

        pickedUp = true;

        if (debugMessages)
            Debug.Log("Picked health item: " + itemType);

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
        position.y += Mathf.Sin(Time.time * bobSpeed) * bobAmount;
        modelToAnimate.localPosition = position;
    }
}