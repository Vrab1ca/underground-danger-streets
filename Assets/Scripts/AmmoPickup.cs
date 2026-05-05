using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    public int ammoAmount = 30;

    void OnTriggerEnter(Collider other)
    {
        WeaponSwitcher switcher = other.GetComponentInChildren<WeaponSwitcher>();
        if (switcher == null) return;

        Weapon weapon = switcher.GetActiveWeapon();
        if (weapon == null) return;

        weapon.AddAmmo(ammoAmount);
        Destroy(gameObject);
    }
}
