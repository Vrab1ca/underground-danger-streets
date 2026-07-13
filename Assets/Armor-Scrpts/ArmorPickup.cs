using UnityEngine;

public class ArmorPickup : MonoBehaviour
{
    [Header("Armor Type")]
    public bool randomArmorType = false;
    public ArmorItemType armorType = ArmorItemType.Strong100;

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
        if (randomArmorType)
            armorType = GetRandomArmorType();

        if (modelToAnimate == null)
            modelToAnimate = transform;

        startLocalPosition = modelToAnimate.localPosition;
        FindReferences();
    }

    private void Update()
    {
        AnimatePickup();

        if (pickedUp)
            return;

        if (player == null || weaponSwitcher == null)
        {
            FindReferences();
            return;
        }

        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

        if (distance > pickupDistance)
            return;

        if (Input.GetKeyDown(pickupKey))
            TryPickupArmor();
    }

    private void FindReferences()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject == null)
        {
            if (debugMessages)
                Debug.LogWarning(gameObject.name + " cannot find Player tag.");

            return;
        }

        player = playerObject.transform;

        weaponSwitcher =
            playerObject.GetComponentInChildren<WeaponSwitcher>(true);

        if (weaponSwitcher == null)
            weaponSwitcher = playerObject.GetComponentInParent<WeaponSwitcher>();

        if (weaponSwitcher == null)
            weaponSwitcher = FindFirstObjectByType<WeaponSwitcher>();

        if (weaponSwitcher == null && debugMessages)
            Debug.LogWarning(gameObject.name + " cannot find WeaponSwitcher.");
    }

    private void TryPickupArmor()
    {
        if (weaponSwitcher == null)
        {
            Debug.LogWarning("ArmorPickup cannot find WeaponSwitcher.");
            return;
        }

        bool added = weaponSwitcher.TryAddArmorItem(armorType);

        if (!added)
        {
            if (debugMessages)
            {
                Debug.Log(
                    "Cannot pick up armor " + armorType +
                    ". There is no empty hotbar slot."
                );
            }

            return;
        }

        pickedUp = true;

        if (debugMessages)
            Debug.Log("Picked armor: " + armorType);

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

    private ArmorItemType GetRandomArmorType()
    {
        int randomValue = Random.Range(0, 4);

        switch (randomValue)
        {
            case 0:
                return ArmorItemType.Strong100;
            case 1:
                return ArmorItemType.Strong50;
            case 2:
                return ArmorItemType.Weak100;
            default:
                return ArmorItemType.Weak50;
        }
    }
}