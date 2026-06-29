using UnityEngine;

public class HelicopterBombDropper : MonoBehaviour
{
    [Header("References")]
    public HelicopterController helicopterController;
    public GameObject bombPrefab;
    public Transform bombDropPoint;

    [Header("Bomb Settings")]
    public int maxBombs = 5;
    public int currentBombs = 5;
    public KeyCode dropKey = KeyCode.B;
    public float dropCooldown = 0.7f;
    public float downwardDropSpeed = 5f;

    [Header("Fuel")]
    public VehicleFuel fuel;
    public float fuelCostPerBomb = 3f;

    private Rigidbody helicopterRigidbody;
    private float nextDropTime;

    private void Awake()
    {
        helicopterRigidbody = GetComponent<Rigidbody>();

        if (currentBombs > maxBombs)
            currentBombs = maxBombs;

        if (fuel == null)
            fuel = GetComponent<VehicleFuel>();

        if (fuel == null)
            fuel = GetComponentInParent<VehicleFuel>();
    }

    private void Update()
    {
        if (helicopterController == null)
            return;

        if (!helicopterController.canFly)
            return;

        if (Input.GetKeyDown(dropKey))
        {
            DropBomb();
        }
    }

    public void DropBomb()
    {
        if (Time.time < nextDropTime)
            return;

        if (currentBombs <= 0)
        {
            Debug.Log("No bombs left.");
            return;
        }

        if (bombPrefab == null)
        {
            Debug.LogWarning("Bomb prefab is missing.");
            return;
        }

        if (fuel != null && !fuel.UseFuel(fuelCostPerBomb))
        {
            Debug.Log("Not enough fuel to drop bomb.");
            return;
        }

        Vector3 spawnPosition;

        if (bombDropPoint != null)
            spawnPosition = bombDropPoint.position;
        else
            spawnPosition = transform.position - transform.up;

        GameObject bombObject = Instantiate(
            bombPrefab,
            spawnPosition,
            Quaternion.identity
        );

        IgnoreHelicopterCollision(bombObject);

        Vector3 startVelocity = Vector3.down * downwardDropSpeed;

        if (helicopterRigidbody != null)
            startVelocity += helicopterRigidbody.linearVelocity;

        HelicopterBomb bomb = bombObject.GetComponent<HelicopterBomb>();

        if (bomb != null)
        {
            bomb.Launch(startVelocity);
        }
        else
        {
            Rigidbody bombRb = bombObject.GetComponent<Rigidbody>();

            if (bombRb != null)
                bombRb.linearVelocity = startVelocity;
        }

        currentBombs--;
        nextDropTime = Time.time + dropCooldown;

        Debug.Log("Bomb dropped. Bombs left: " + currentBombs);
    }

    public void AddBombs(int amount)
    {
        currentBombs += amount;

        if (currentBombs > maxBombs)
            currentBombs = maxBombs;

        Debug.Log("Bombs added. Current bombs: " + currentBombs);
    }

    private void IgnoreHelicopterCollision(GameObject bombObject)
    {
        Collider[] helicopterColliders = GetComponentsInChildren<Collider>();
        Collider[] bombColliders = bombObject.GetComponentsInChildren<Collider>();

        foreach (Collider helicopterCollider in helicopterColliders)
        {
            foreach (Collider bombCollider in bombColliders)
            {
                Physics.IgnoreCollision(helicopterCollider, bombCollider);
            }
        }
    }
}