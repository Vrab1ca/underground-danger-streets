using UnityEngine;

public class PlayerGrenadeInventory : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public Transform throwPoint;

    [Header("Grenade Prefabs")]
    public GameObject normalGrenadePrefab;
    public GameObject molotovGrenadePrefab;

    [Header("Inventory")]
    public int maxNormalGrenades = 3;
    public int normalGrenades = 0;

    public int maxMolotovs = 3;
    public int molotovs = 0;

    [Header("Throw Settings")]
    public KeyCode throwKey = KeyCode.Q;
    public KeyCode switchKey = KeyCode.Z;
    public float throwForce = 16f;
    public float upwardForce = 2f;

    [Header("Selected")]
    public GrenadeType selectedGrenade = GrenadeType.Normal;

    private void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        normalGrenades = Mathf.Clamp(normalGrenades, 0, maxNormalGrenades);
        molotovs = Mathf.Clamp(molotovs, 0, maxMolotovs);
    }

    private void Update()
    {
        if (Input.GetKeyDown(switchKey))
        {
            SwitchGrenade();
        }

        if (Input.GetKeyDown(throwKey))
        {
            ThrowGrenade();
        }
    }

    public void AddGrenades(GrenadeType type, int amount)
    {
        if (type == GrenadeType.Normal)
        {
            normalGrenades += amount;

            if (normalGrenades > maxNormalGrenades)
                normalGrenades = maxNormalGrenades;

            Debug.Log("Normal grenades: " + normalGrenades + " / " + maxNormalGrenades);
        }
        else if (type == GrenadeType.Molotov)
        {
            molotovs += amount;

            if (molotovs > maxMolotovs)
                molotovs = maxMolotovs;

            Debug.Log("Molotovs: " + molotovs + " / " + maxMolotovs);
        }
    }

    private void SwitchGrenade()
    {
        if (selectedGrenade == GrenadeType.Normal)
            selectedGrenade = GrenadeType.Molotov;
        else
            selectedGrenade = GrenadeType.Normal;

        Debug.Log("Selected grenade: " + selectedGrenade);
    }

    private void ThrowGrenade()
    {
        GameObject prefabToThrow = null;

        if (selectedGrenade == GrenadeType.Normal)
        {
            if (normalGrenades <= 0)
            {
                Debug.Log("No normal grenades.");
                return;
            }

            prefabToThrow = normalGrenadePrefab;
        }
        else if (selectedGrenade == GrenadeType.Molotov)
        {
            if (molotovs <= 0)
            {
                Debug.Log("No molotovs.");
                return;
            }

            prefabToThrow = molotovGrenadePrefab;
        }

        if (prefabToThrow == null)
        {
            Debug.LogWarning("Grenade prefab is missing.");
            return;
        }

        Vector3 spawnPosition;

        if (throwPoint != null)
            spawnPosition = throwPoint.position;
        else
            spawnPosition = transform.position + transform.forward * 1f + Vector3.up * 1f;

        GameObject grenadeObject = Instantiate(
            prefabToThrow,
            spawnPosition,
            Quaternion.identity
        );

        GrenadeProjectile projectile = grenadeObject.GetComponent<GrenadeProjectile>();

        if (projectile != null)
            projectile.SetOwner(transform);

        Rigidbody rb = grenadeObject.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 direction;

            if (playerCamera != null)
                direction = playerCamera.transform.forward;
            else
                direction = transform.forward;

            Vector3 force = direction * throwForce + Vector3.up * upwardForce;

            rb.AddForce(force, ForceMode.Impulse);
            rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);
        }

        if (selectedGrenade == GrenadeType.Normal)
            normalGrenades--;

        else if (selectedGrenade == GrenadeType.Molotov)
            molotovs--;

        Debug.Log("Thrown grenade: " + selectedGrenade);
    }
}