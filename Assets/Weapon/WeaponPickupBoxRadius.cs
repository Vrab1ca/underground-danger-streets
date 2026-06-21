using UnityEngine;

public class WeaponPickupBoxRadius : MonoBehaviour
{
    [Header("Weapon")]
    public GameObject weaponPrefab;

    [Header("Pickup Settings")]
    public KeyCode pickupKey = KeyCode.F;
    public float pickupDistance = 3f;

    private WeaponSwitcher weaponSwitcher;

    private void Start()
    {
        FindWeaponSwitcher();
    }

    private void Update()
    {
        if (weaponSwitcher == null)
            FindWeaponSwitcher();

        if (weaponSwitcher == null)
            return;

        float distance = Vector3.Distance(transform.position, weaponSwitcher.transform.position);

        if (distance <= pickupDistance)
        {
            Debug.Log("Near weapon pickup. Press F.");

            if (Input.GetKeyDown(pickupKey))
            {
                bool picked = weaponSwitcher.AddWeapon(weaponPrefab);

                if (picked)
                {
                    Debug.Log("Picked weapon: " + weaponPrefab.name);
                    Destroy(gameObject);
                }
                else
                {
                    Debug.LogWarning("Cannot pick weapon. Inventory is probably full.");
                }
            }
        }
    }

    private void FindWeaponSwitcher()
    {
        weaponSwitcher = FindFirstObjectByType<WeaponSwitcher>();
    }
}