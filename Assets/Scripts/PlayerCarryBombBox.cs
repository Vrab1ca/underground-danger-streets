using UnityEngine;

public class PlayerCarryBombBox : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public Transform carryPoint;

    [Header("Disable Combat While Carrying")]
    public GameObject weaponHolder;
    public WeaponSwitcher weaponSwitcher;
    public PlayerGrenadeInventory grenadeInventory;
    public JumpPlatformInventory jumpPlatformInventory;

    [Header("Keys")]
    public KeyCode takeKey = KeyCode.E;
    public KeyCode loadKey = KeyCode.E;
    public KeyCode dropKey = KeyCode.G;

    [Header("Distances")]
    public float pickupDistance = 3f;
    public float loadDistance = 5f;

    [Header("Drop")]
    public float dropForwardForce = 2f;

    private CarryableBombRefillBox heldBox;

    private void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    private void Update()
    {
        if (heldBox == null)
        {
            if (Input.GetKeyDown(takeKey))
                TryPickUpBox();

            return;
        }

        DisableCombat();

        if (Input.GetKeyDown(loadKey))
        {
            TryLoadIntoHelicopter();
            return;
        }

        if (Input.GetKeyDown(dropKey))
        {
            DropBox();
            return;
        }
    }

    private void TryPickUpBox()
    {
        CarryableBombRefillBox box = FindBombBox();

        if (box == null)
        {
            Debug.Log("No bomb box close enough.");
            return;
        }

        heldBox = box;
        heldBox.PickUp(carryPoint);

        DisableCombat();

        Debug.Log("Bomb box carried. Press G to drop or E near helicopter to load.");
    }

    private CarryableBombRefillBox FindBombBox()
    {
        if (playerCamera != null)
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, pickupDistance, ~0, QueryTriggerInteraction.Collide))
            {
                CarryableBombRefillBox box = hit.collider.GetComponentInParent<CarryableBombRefillBox>();

                if (box != null && !box.isHeld)
                    return box;
            }
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, pickupDistance);

        foreach (Collider hit in hits)
        {
            CarryableBombRefillBox box = hit.GetComponentInParent<CarryableBombRefillBox>();

            if (box != null && !box.isHeld)
                return box;
        }

        return null;
    }

    private void TryLoadIntoHelicopter()
    {
        HelicopterBombLoader loader = FindHelicopterLoader();

        if (loader == null)
        {
            Debug.Log("No helicopter close enough to load bomb box.");
            return;
        }

        bool loaded = loader.TryLoadBox(heldBox, transform);

        if (loaded)
        {
            heldBox = null;
            EnableCombat();
        }
    }

    private HelicopterBombLoader FindHelicopterLoader()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, loadDistance);

        foreach (Collider hit in hits)
        {
            HelicopterBombLoader loader = hit.GetComponentInParent<HelicopterBombLoader>();

            if (loader != null)
                return loader;
        }

        return null;
    }

    private void DropBox()
    {
        if (heldBox == null)
            return;

        Vector3 dropVelocity = Vector3.zero;

        if (playerCamera != null)
            dropVelocity = playerCamera.transform.forward * dropForwardForce;

        heldBox.Drop(dropVelocity);
        heldBox = null;

        EnableCombat();
    }

    private void DisableCombat()
    {
        if (weaponHolder != null)
            weaponHolder.SetActive(false);

        if (weaponSwitcher != null)
            weaponSwitcher.enabled = false;

        if (grenadeInventory != null)
            grenadeInventory.enabled = false;

        if (jumpPlatformInventory != null)
            jumpPlatformInventory.enabled = false;
    }

    private void EnableCombat()
    {
        if (weaponHolder != null)
            weaponHolder.SetActive(true);

        if (weaponSwitcher != null)
            weaponSwitcher.enabled = true;

        if (grenadeInventory != null)
            grenadeInventory.enabled = true;

        if (jumpPlatformInventory != null)
            jumpPlatformInventory.enabled = true;
    }

    public bool IsCarryingBombBox()
    {
        return heldBox != null;
    }
}