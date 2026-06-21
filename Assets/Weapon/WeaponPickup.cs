using UnityEngine;

public class WeaponPickupBox : MonoBehaviour
{
    public GameObject weaponPrefab;
    public KeyCode pickupKey = KeyCode.F;

    private WeaponSwitcher weaponSwitcher;

    private void OnTriggerEnter(Collider other)
    {
        TryFindWeaponSwitcher(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryFindWeaponSwitcher(other);

        if (weaponSwitcher != null && Input.GetKeyDown(pickupKey))
        {
            bool picked = weaponSwitcher.AddWeapon(weaponPrefab);

            if (picked)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        WeaponSwitcher switcher = other.GetComponentInChildren<WeaponSwitcher>();

        if (switcher == null)
            switcher = other.GetComponentInParent<WeaponSwitcher>();

        if (switcher == weaponSwitcher)
            weaponSwitcher = null;
    }

    private void TryFindWeaponSwitcher(Collider other)
    {
        if (weaponSwitcher != null)
            return;

        WeaponSwitcher switcher = other.GetComponentInChildren<WeaponSwitcher>();

        if (switcher == null)
            switcher = other.GetComponentInParent<WeaponSwitcher>();

        if (switcher == null)
            return;

        weaponSwitcher = switcher;

        Debug.Log("Press F to pick weapon.");
    }
}