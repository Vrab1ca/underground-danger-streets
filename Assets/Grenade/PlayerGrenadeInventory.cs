using UnityEngine;

public class PlayerGrenadeInventory : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public Transform throwPoint;

    [Header("Grenade Prefabs")]
    public GameObject normalGrenadePrefab;
    public GameObject molotovPrefab;

    [Header("Normal Grenades")]
    public int normalGrenades = 3;
    public int maxNormalGrenades = 3;

    [Header("Molotovs")]
    public int molotovs = 2;
    public int maxMolotovs = 2;

    [Header("Selected Grenade")]
    public GrenadeType selectedGrenade = GrenadeType.Normal;

    [Header("Throw Settings")]
    public float throwForce = 18f;
    public float upwardForce = 2f;

    [Header("Old Controls Optional")]
    public bool useOldQZControls = false;
    public KeyCode oldThrowKey = KeyCode.Q;
    public KeyCode oldSwitchKey = KeyCode.Z;

    private void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        ClampGrenades();
    }

    private void Update()
    {
        if (!useOldQZControls)
            return;

        if (Input.GetKeyDown(oldSwitchKey))
            NextGrenade();

        if (Input.GetKeyDown(oldThrowKey))
            ThrowSelectedGrenade();
    }

    private void OnValidate()
    {
        ClampGrenades();
    }

    private void ClampGrenades()
    {
        normalGrenades = Mathf.Clamp(normalGrenades, 0, maxNormalGrenades);
        molotovs = Mathf.Clamp(molotovs, 0, maxMolotovs);
    }

    public void SelectNormalGrenade()
    {
        selectedGrenade = GrenadeType.Normal;
        Debug.Log("Selected Normal Grenade");
    }

    public void SelectMolotov()
    {
        selectedGrenade = GrenadeType.Molotov;
        Debug.Log("Selected Molotov");
    }

    public void NextGrenade()
    {
        if (selectedGrenade == GrenadeType.Normal)
            selectedGrenade = GrenadeType.Molotov;
        else
            selectedGrenade = GrenadeType.Normal;

        Debug.Log("Selected grenade: " + selectedGrenade);
    }

    public void PreviousGrenade()
    {
        NextGrenade();
    }

    public void ThrowSelectedGrenade()
    {
        if (selectedGrenade == GrenadeType.Normal)
        {
            ThrowNormalGrenade();
            return;
        }

        if (selectedGrenade == GrenadeType.Molotov)
        {
            ThrowMolotov();
            return;
        }
    }

    public void ThrowNormalGrenade()
    {
        if (normalGrenades <= 0)
        {
            Debug.Log("No normal grenades.");
            return;
        }

        if (normalGrenadePrefab == null)
        {
            Debug.LogWarning("Normal Grenade Prefab is missing.");
            return;
        }

        ThrowGrenadePrefab(normalGrenadePrefab);
        normalGrenades--;

        Debug.Log("Threw Normal Grenade. Left: " + normalGrenades);
    }

    public void ThrowMolotov()
    {
        if (molotovs <= 0)
        {
            Debug.Log("No molotovs.");
            return;
        }

        if (molotovPrefab == null)
        {
            Debug.LogWarning("Molotov Prefab is missing.");
            return;
        }

        ThrowGrenadePrefab(molotovPrefab);
        molotovs--;

        Debug.Log("Threw Molotov. Left: " + molotovs);
    }

    private void ThrowGrenadePrefab(GameObject grenadePrefab)
    {
        Transform spawnPoint = throwPoint;

        if (spawnPoint == null && playerCamera != null)
            spawnPoint = playerCamera.transform;

        if (spawnPoint == null)
        {
            Debug.LogWarning("No ThrowPoint or Camera found.");
            return;
        }

        GameObject grenade = Instantiate(
            grenadePrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        Rigidbody rb = grenade.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 forceDirection = spawnPoint.forward * throwForce;
            forceDirection += Vector3.up * upwardForce;

            rb.AddForce(forceDirection, ForceMode.Impulse);
        }
    }

    public void AddGrenade(GrenadeType type, int amount)
    {
        if (type == GrenadeType.Normal)
        {
            normalGrenades += amount;

            if (normalGrenades > maxNormalGrenades)
                normalGrenades = maxNormalGrenades;
        }

        if (type == GrenadeType.Molotov)
        {
            molotovs += amount;

            if (molotovs > maxMolotovs)
                molotovs = maxMolotovs;
        }
    }

    public void AddGrenades(GrenadeType type, int amount)
    {
        AddGrenade(type, amount);
    }

    public void AddNormalGrenades(int amount)
    {
        normalGrenades += amount;

        if (normalGrenades > maxNormalGrenades)
            normalGrenades = maxNormalGrenades;
    }

    public void AddMolotovs(int amount)
    {
        molotovs += amount;

        if (molotovs > maxMolotovs)
            molotovs = maxMolotovs;
    }

    public int GetGrenadeCount(GrenadeType type)
    {
        if (type == GrenadeType.Normal)
            return normalGrenades;

        if (type == GrenadeType.Molotov)
            return molotovs;

        return 0;
    }
}