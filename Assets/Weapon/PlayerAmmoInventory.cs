using UnityEngine;

public class PlayerAmmoInventory : MonoBehaviour
{
    [Header("Current Ammo")]
    public int awpAmmo = 0;
    public int ak47Ammo = 0;
    public int glockAmmo = 0;
    public int pumpAmmo = 0;
    public int rifleAmmo = 0;

    [Header("Max Ammo")]
    public int maxAwpAmmo = 30;
    public int maxAk47Ammo = 180;
    public int maxGlockAmmo = 120;
    public int maxPumpAmmo = 40;
    public int maxRifleAmmo = 180;

    public int AddAmmo(WeaponAmmoType ammoType, int amount)
    {
        int before = GetAmmo(ammoType);

        if (ammoType == WeaponAmmoType.AWP)
            awpAmmo = Mathf.Clamp(awpAmmo + amount, 0, maxAwpAmmo);

        if (ammoType == WeaponAmmoType.AK47)
            ak47Ammo = Mathf.Clamp(ak47Ammo + amount, 0, maxAk47Ammo);

        if (ammoType == WeaponAmmoType.Glock)
            glockAmmo = Mathf.Clamp(glockAmmo + amount, 0, maxGlockAmmo);

        if (ammoType == WeaponAmmoType.Pump)
            pumpAmmo = Mathf.Clamp(pumpAmmo + amount, 0, maxPumpAmmo);

        if (ammoType == WeaponAmmoType.Rifle)
            rifleAmmo = Mathf.Clamp(rifleAmmo + amount, 0, maxRifleAmmo);

        int after = GetAmmo(ammoType);
        int added = after - before;

        Debug.Log("Added " + added + " ammo to " + ammoType + ". Now: " + after);

        return added;
    }

    public int TakeAmmo(WeaponAmmoType ammoType, int amount)
    {
        int available = GetAmmo(ammoType);
        int taken = Mathf.Min(available, amount);

        if (ammoType == WeaponAmmoType.AWP)
            awpAmmo -= taken;

        if (ammoType == WeaponAmmoType.AK47)
            ak47Ammo -= taken;

        if (ammoType == WeaponAmmoType.Glock)
            glockAmmo -= taken;

        if (ammoType == WeaponAmmoType.Pump)
            pumpAmmo -= taken;

        if (ammoType == WeaponAmmoType.Rifle)
            rifleAmmo -= taken;

        Debug.Log("Took " + taken + " ammo from " + ammoType + ". Left: " + GetAmmo(ammoType));

        return taken;
    }

    public int GetAmmo(WeaponAmmoType ammoType)
    {
        if (ammoType == WeaponAmmoType.AWP)
            return awpAmmo;

        if (ammoType == WeaponAmmoType.AK47)
            return ak47Ammo;

        if (ammoType == WeaponAmmoType.Glock)
            return glockAmmo;

        if (ammoType == WeaponAmmoType.Pump)
            return pumpAmmo;

        if (ammoType == WeaponAmmoType.Rifle)
            return rifleAmmo;

        return 0;
    }
}