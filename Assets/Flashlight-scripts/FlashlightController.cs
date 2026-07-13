using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    [Header("References")]
    public WeaponSwitcher weaponSwitcher;
    public PlayerFlashlightInventory flashlightInventory;

    [Tooltip("The flashlight model that appears in the player's hand.")]
    public GameObject flashlightHandVisual;

    [Tooltip("The Spot Light inside the hand flashlight model.")]
    public Light flashlightLight;

    [Header("Controls")]
    [Tooltip("Works only while the flashlight hotbar slot is selected.")]
    public KeyCode toggleKey = KeyCode.T;

    [Header("Battery Drain")]
    [Tooltip("Battery percent used every real second while the light is on.")]
    [Min(0f)]
    public float chargeDrainPerSecond = 0.35f;

    [Header("Behaviour")]
    [Tooltip("Turn the light off when another slot or fists are selected.")]
    public bool turnOffWhenDeselected = true;

    [Header("Debug")]
    public bool debugMessages = true;

    private bool flashlightIsOn;
    private bool wasSelectedLastFrame;

    public bool IsOn
    {
        get { return flashlightIsOn; }
    }

    private void Awake()
    {
        FindReferences();
        DisableHeldVisualPhysics();
        SetFlashlightLight(false);

        if (flashlightHandVisual != null)
            flashlightHandVisual.SetActive(false);
    }

    private void LateUpdate()
    {
        FindReferences();

        bool selected =
            weaponSwitcher != null &&
            flashlightInventory != null &&
            flashlightInventory.HasFlashlight &&
            weaponSwitcher.IsFlashlightSelected();

        if (flashlightHandVisual != null &&
            flashlightHandVisual.activeSelf != selected)
        {
            flashlightHandVisual.SetActive(selected);
        }

        if (!selected)
        {
            if (turnOffWhenDeselected &&
                wasSelectedLastFrame)
            {
                flashlightIsOn = false;
            }

            SetFlashlightLight(false);
            wasSelectedLastFrame = false;
            return;
        }

        if (Input.GetKeyDown(toggleKey))
            ToggleFlashlight();

        if (flashlightIsOn)
        {
            bool stillHasCharge =
                flashlightInventory.DrainCharge(
                    chargeDrainPerSecond * Time.deltaTime
                );

            if (!stillHasCharge)
            {
                flashlightIsOn = false;

                if (debugMessages)
                    Debug.Log("Flashlight battery is empty.");
            }
        }

        SetFlashlightLight(
            flashlightIsOn &&
            flashlightInventory.HasCharge
        );

        wasSelectedLastFrame = true;
    }

    private void FindReferences()
    {
        if (weaponSwitcher == null)
            weaponSwitcher = GetComponent<WeaponSwitcher>();

        if (weaponSwitcher == null)
        {
            weaponSwitcher =
                GetComponentInChildren<WeaponSwitcher>(true);
        }

        if (weaponSwitcher == null)
        {
            weaponSwitcher =
                FindFirstObjectByType<WeaponSwitcher>();
        }

        if (flashlightInventory == null)
        {
            flashlightInventory =
                GetComponent<PlayerFlashlightInventory>();
        }

        if (flashlightInventory == null)
        {
            flashlightInventory =
                GetComponentInChildren
                    <PlayerFlashlightInventory>(true);
        }

        if (flashlightInventory == null)
        {
            flashlightInventory =
                FindFirstObjectByType
                    <PlayerFlashlightInventory>();
        }

        if (flashlightLight == null &&
            flashlightHandVisual != null)
        {
            flashlightLight =
                flashlightHandVisual
                    .GetComponentInChildren<Light>(true);
        }
    }

    private void ToggleFlashlight()
    {
        if (flashlightInventory == null ||
            !flashlightInventory.HasFlashlight)
        {
            return;
        }

        if (!flashlightInventory.HasCharge)
        {
            flashlightIsOn = false;
            SetFlashlightLight(false);

            if (debugMessages)
            {
                Debug.Log(
                    "Flashlight has 0% charge. " +
                    "Select a battery slot and press R."
                );
            }

            return;
        }

        flashlightIsOn = !flashlightIsOn;
        SetFlashlightLight(flashlightIsOn);

        if (debugMessages)
        {
            Debug.Log(
                flashlightIsOn
                    ? "Flashlight ON"
                    : "Flashlight OFF"
            );
        }
    }

    private void SetFlashlightLight(bool enabledState)
    {
        if (flashlightLight != null)
            flashlightLight.enabled = enabledState;
    }

    private void DisableHeldVisualPhysics()
    {
        if (flashlightHandVisual == null)
            return;

        Collider[] colliders =
            flashlightHandVisual
                .GetComponentsInChildren<Collider>(true);

        for (int i = 0; i < colliders.Length; i++)
            colliders[i].enabled = false;

        Rigidbody[] rigidbodies =
            flashlightHandVisual
                .GetComponentsInChildren<Rigidbody>(true);

        for (int i = 0; i < rigidbodies.Length; i++)
        {
            rigidbodies[i].useGravity = false;
            rigidbodies[i].isKinematic = true;
            rigidbodies[i].detectCollisions = false;
        }
    }
}
