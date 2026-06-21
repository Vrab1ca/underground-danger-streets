using UnityEngine;

public class GrenadePickupBox : MonoBehaviour
{
    [Header("Pickup")]
    public GrenadeType grenadeType = GrenadeType.Normal;
    public int amount = 1;

    private void OnTriggerEnter(Collider other)
    {
        PlayerGrenadeInventory inventory = other.GetComponentInParent<PlayerGrenadeInventory>();

        if (inventory == null)
            inventory = other.GetComponentInChildren<PlayerGrenadeInventory>();

        if (inventory == null)
            return;

        inventory.AddGrenades(grenadeType, amount);

        Destroy(gameObject);
    }
}