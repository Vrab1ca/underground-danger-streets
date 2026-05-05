using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public GameObject weaponPrefab;
    public Transform weaponHolder;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (weaponHolder == null)
        {
            WeaponSwitcher switcher = other.GetComponentInChildren<WeaponSwitcher>();
            if (switcher != null) weaponHolder = switcher.transform;
        }

        if (weaponPrefab != null && weaponHolder != null)
        {
            GameObject weapon = Instantiate(weaponPrefab, weaponHolder);
            weapon.transform.localPosition = Vector3.zero;
            weapon.transform.localRotation = Quaternion.identity;
            Destroy(gameObject);
        }
    }
}
