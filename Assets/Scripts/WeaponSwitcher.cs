using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    [Header("Inventory")]
    public int selectedWeapon = 0;
    public int maxWeapons = 2;

    [Header("Camera References")]
    public Camera fpsCamera;
    public Camera carCamera;

    [Header("Drop")]
    public Transform dropPoint;
    public KeyCode dropKey = KeyCode.G;

    private void Start()
    {
        SelectWeapon();
    }

    private void Update()
    {
        if (Input.GetKeyDown(dropKey))
            DropCurrentWeapon();

        if (transform.childCount == 0)
            return;

        int previousWeapon = selectedWeapon;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            selectedWeapon = 0;

        if (Input.GetKeyDown(KeyCode.Alpha2))
            selectedWeapon = 1;

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f)
            selectedWeapon++;

        if (scroll < 0f)
            selectedWeapon--;

        if (selectedWeapon >= transform.childCount)
            selectedWeapon = 0;

        if (selectedWeapon < 0)
            selectedWeapon = transform.childCount - 1;

        if (previousWeapon != selectedWeapon)
            SelectWeapon();
    }

    public bool AddWeapon(GameObject weaponPrefab)
    {
        if (weaponPrefab == null)
        {
            Debug.LogWarning("Weapon prefab is missing.");
            return false;
        }

        if (transform.childCount >= maxWeapons)
        {
            Debug.LogWarning("Inventory full. You can carry only 2 weapons. Press G to drop one.");
            return false;
        }

        GameObject newWeapon = Instantiate(weaponPrefab, transform);

        newWeapon.transform.localPosition = Vector3.zero;
        newWeapon.transform.localRotation = Quaternion.identity;
        newWeapon.transform.localScale = weaponPrefab.transform.localScale;

        PrepareWeapon(newWeapon);

        selectedWeapon = transform.childCount - 1;
        SelectWeapon();

        Debug.Log("Picked weapon: " + newWeapon.name);

        return true;
    }

    private void PrepareWeapon(GameObject weaponObject)
    {
        Weapon weapon = weaponObject.GetComponent<Weapon>();

        if (weapon != null)
        {
            weapon.fpsCamera = fpsCamera;
            weapon.carCamera = carCamera;
        }

        SimpleADS ads = weaponObject.GetComponent<SimpleADS>();

        if (ads != null)
        {
            ads.fpsCamera = fpsCamera;
            ads.carCamera = carCamera;
        }
    }

    public void DropCurrentWeapon()
    {
        Weapon activeWeapon = GetActiveWeapon();

        if (activeWeapon == null)
        {
            Debug.Log("No weapon to drop.");
            return;
        }

        if (activeWeapon.pickupPrefab == null)
        {
            Debug.LogWarning("Pickup Prefab missing on weapon: " + activeWeapon.weaponName);
            return;
        }

        Vector3 spawnPosition;

        if (dropPoint != null)
            spawnPosition = dropPoint.position;
        else
            spawnPosition = transform.position + transform.forward * 2f;

        GameObject droppedPickup = Instantiate(
            activeWeapon.pickupPrefab,
            spawnPosition,
            Quaternion.identity
        );

        // Make sure dropped pickup is active
        droppedPickup.SetActive(true);

        // Make sure radius pickup script has the correct weapon
        WeaponPickupBoxRadius pickupScript = droppedPickup.GetComponent<WeaponPickupBoxRadius>();

        if (pickupScript != null)
        {
            pickupScript.pickupDistance = 3f;
            pickupScript.pickupKey = KeyCode.F;
        }

        Debug.Log("Dropped weapon: " + activeWeapon.weaponName);

        Destroy(activeWeapon.gameObject);

        selectedWeapon = 0;

        Invoke(nameof(SelectWeapon), 0.05f);
    }

    public void SelectWeapon()
    {
        if (transform.childCount == 0)
            return;

        selectedWeapon = Mathf.Clamp(selectedWeapon, 0, transform.childCount - 1);

        for (int i = 0; i < transform.childCount; i++)
        {
            bool active = i == selectedWeapon;
            transform.GetChild(i).gameObject.SetActive(active);
        }
    }

    public Weapon GetActiveWeapon()
    {
        if (transform.childCount == 0)
            return null;

        selectedWeapon = Mathf.Clamp(selectedWeapon, 0, transform.childCount - 1);

        return transform.GetChild(selectedWeapon).GetComponent<Weapon>();
    }
}