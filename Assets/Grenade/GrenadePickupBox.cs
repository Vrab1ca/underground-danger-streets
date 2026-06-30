using UnityEngine;

public class GrenadePickupBox : MonoBehaviour
{
    [Header("Pickup")]
    public GrenadeType grenadeType = GrenadeType.Normal;
    public int amount = 1;
    public KeyCode pickupKey = KeyCode.F;
    public float pickupDistance = 3f;

    [Header("Destroy")]
    public bool destroyAfterPickup = true;

    private PlayerGrenadeInventory inventory;
    private Transform player;

    private void Start()
    {
        inventory = FindFirstObjectByType<PlayerGrenadeInventory>();

        if (inventory != null)
            player = inventory.transform;
    }

    private void Update()
    {
        if (inventory == null || player == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > pickupDistance)
            return;

        if (Input.GetKeyDown(pickupKey))
        {
            PickupGrenades();
        }
    }

    private void PickupGrenades()
    {
        int before = inventory.GetGrenadeCount(grenadeType);

        inventory.AddGrenade(grenadeType, amount);

        int after = inventory.GetGrenadeCount(grenadeType);

        if (after > before)
        {
            Debug.Log("Picked up " + amount + " " + grenadeType + ". Now: " + after);

            if (destroyAfterPickup)
                Destroy(gameObject);
        }
        else
        {
            Debug.Log("Cannot pick up " + grenadeType + ". Inventory full.");
        }
    }
}