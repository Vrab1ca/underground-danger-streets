using UnityEngine;

public class FlashlightPickup : MonoBehaviour
{
    [Header("Pickup")]
    public KeyCode pickupKey = KeyCode.E;

    [Min(0.1f)]
    public float pickupDistance = 3f;

    [Range(0f, 100f)]
    public float startingCharge = 20f;

    [Header("Case Objects")]
    [Tooltip("The flashlight model visible inside the small case.")]
    public GameObject flashlightObjectInCase;

    [Tooltip("Enable this only when the complete case should disappear.")]
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
            TryPickupFlashlight();
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

    private void TryPickupFlashlight()
    {
        if (weaponSwitcher == null)
        {
            Debug.LogWarning(
                "FlashlightPickup cannot find WeaponSwitcher."
            );
            return;
        }

        bool added =
            weaponSwitcher.TryAddFlashlight(startingCharge);

        if (!added)
            return;

        collected = true;

        if (flashlightObjectInCase != null)
            flashlightObjectInCase.SetActive(false);

        if (debugMessages)
        {
            Debug.Log(
                "Flashlight picked up and added to hotbar."
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
