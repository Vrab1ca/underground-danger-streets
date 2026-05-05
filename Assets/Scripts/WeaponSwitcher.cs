using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    public int selectedWeapon = 0;

    void Start()
    {
        SelectWeapon();
    }

    void Update()
    {
        // Ако няма оръжия вътре в WeaponHolder, не прави нищо.
        if (transform.childCount == 0)
            return;

        int previous = selectedWeapon;

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f)
        {
            selectedWeapon++;

            if (selectedWeapon >= transform.childCount)
                selectedWeapon = 0;
        }

        if (scroll < 0f)
        {
            selectedWeapon--;

            if (selectedWeapon < 0)
                selectedWeapon = transform.childCount - 1;
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                selectedWeapon = i;
        }

        if (previous != selectedWeapon)
            SelectWeapon();
    }

    void SelectWeapon()
    {
        if (transform.childCount == 0)
            return;

        selectedWeapon = Mathf.Clamp(selectedWeapon, 0, transform.childCount - 1);

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(i == selectedWeapon);
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