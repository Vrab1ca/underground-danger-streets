using UnityEngine;
using TMPro;

public class BasicHUD : MonoBehaviour
{
    public WeaponSwitcher weaponSwitcher;
    public PlayerHealth playerHealth;

    public TMP_Text ammoText;
    public TMP_Text healthText;

    void Update()
    {
        if (weaponSwitcher != null && ammoText != null)
        {
            Weapon weapon = weaponSwitcher.GetActiveWeapon();

            if (weapon == null)
                ammoText.text = "Ammo: -";
            else
                ammoText.text = "Ammo: " + weapon.AmmoInMagazine + " / " + weapon.ReserveAmmo;
        }

        if (playerHealth != null && healthText != null)
        {
            healthText.text = "Health: " + Mathf.CeilToInt(playerHealth.currentHealth);
        }
    }
}