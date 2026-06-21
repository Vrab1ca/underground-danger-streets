using UnityEngine;

public class JumpPlatformPickup : MonoBehaviour
{
    public int platformAmount = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        JumpPlatformInventory inventory = other.GetComponentInParent<JumpPlatformInventory>();

        if (inventory == null)
            inventory = other.GetComponentInChildren<JumpPlatformInventory>();

        if (inventory == null)
            return;

        inventory.AddPlatforms(platformAmount);

        Destroy(gameObject);
    }
}