using UnityEngine;

public class PlayerFuelCanCarrier : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public Transform carryPoint;

    [Header("Keys")]
    public KeyCode pickupKey = KeyCode.F;
    public KeyCode refuelKey = KeyCode.F;
    public KeyCode dropKey = KeyCode.X;

    [Header("Distances")]
    public float pickupDistance = 3f;
    public float refuelDistance = 4f;

    [Header("Refuel")]
    public float refuelLitersPerSecond = 3f;

    [Header("Layers")]
    public LayerMask fuelCanMask = ~0;
    public LayerMask vehicleMask = ~0;

    [Header("Carry Position")]
    public Vector3 normalLocalPosition = new Vector3(0.35f, -0.35f, 0.8f);
    public Vector3 normalLocalEuler = new Vector3(0f, 0f, 0f);

    [Header("Refueling Animation")]
    public Vector3 refuelLocalPosition = new Vector3(0.45f, -0.25f, 0.8f);
    public Vector3 refuelLocalEuler = new Vector3(0f, 0f, -70f);
    public float rotateSpeed = 8f;
    public float refuelWobbleAmount = 6f;
    public float refuelWobbleSpeed = 8f;

    [Header("Debug")]
    public bool debugMessages = true;

    private FuelCan heldFuelCan;
    private Rigidbody heldRigidbody;
    private Collider[] heldColliders;

    private bool isRefueling;

    private void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    private void Update()
    {
        isRefueling = false;

        if (heldFuelCan == null)
        {
            if (Input.GetKeyDown(pickupKey))
                TryPickupFuelCan();

            return;
        }

        if (Input.GetKeyDown(dropKey))
        {
            DropFuelCan();
            return;
        }

        if (Input.GetKey(refuelKey))
        {
            TryRefuelVehicle();
        }

        UpdateHeldFuelCanVisual();
    }

    private void TryPickupFuelCan()
    {
        FuelCan fuelCan = FindFuelCan();

        if (fuelCan == null)
        {
            if (debugMessages)
                Debug.Log("No fuel can in front or nearby.");

            return;
        }

        PickUpFuelCan(fuelCan);
    }

    private FuelCan FindFuelCan()
    {
        if (playerCamera != null)
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, pickupDistance, fuelCanMask, QueryTriggerInteraction.Collide))
            {
                FuelCan fuelCan = hit.collider.GetComponentInParent<FuelCan>();

                if (fuelCan != null)
                    return fuelCan;
            }
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, pickupDistance, fuelCanMask, QueryTriggerInteraction.Collide);

        FuelCan nearestCan = null;
        float nearestDistance = Mathf.Infinity;

        foreach (Collider hit in hits)
        {
            FuelCan fuelCan = hit.GetComponentInParent<FuelCan>();

            if (fuelCan == null)
                continue;

            float distance = Vector3.Distance(transform.position, fuelCan.transform.position);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestCan = fuelCan;
            }
        }

        return nearestCan;
    }

    private void PickUpFuelCan(FuelCan fuelCan)
    {
        heldFuelCan = fuelCan;
        heldRigidbody = fuelCan.GetComponent<Rigidbody>();
        heldColliders = fuelCan.GetComponentsInChildren<Collider>();

        if (heldRigidbody != null)
        {
            heldRigidbody.isKinematic = true;
            heldRigidbody.useGravity = false;
            heldRigidbody.linearVelocity = Vector3.zero;
            heldRigidbody.angularVelocity = Vector3.zero;
        }

        foreach (Collider col in heldColliders)
        {
            if (col != null)
                col.enabled = false;
        }

        Transform parentPoint = carryPoint;

        if (parentPoint == null && playerCamera != null)
            parentPoint = playerCamera.transform;

        heldFuelCan.transform.SetParent(parentPoint);

        heldFuelCan.transform.localPosition = normalLocalPosition;
        heldFuelCan.transform.localRotation = Quaternion.Euler(normalLocalEuler);

        if (debugMessages)
            Debug.Log("Picked up fuel can.");
    }

    private void DropFuelCan()
    {
        if (heldFuelCan == null)
            return;

        Transform canTransform = heldFuelCan.transform;

        canTransform.SetParent(null);

        foreach (Collider col in heldColliders)
        {
            if (col != null)
                col.enabled = true;
        }

        if (heldRigidbody != null)
        {
            heldRigidbody.isKinematic = false;
            heldRigidbody.useGravity = true;

            if (playerCamera != null)
                heldRigidbody.linearVelocity = playerCamera.transform.forward * 2f;
        }

        if (debugMessages)
            Debug.Log("Dropped fuel can.");

        heldFuelCan = null;
        heldRigidbody = null;
        heldColliders = null;
    }

    private void TryRefuelVehicle()
    {
        if (heldFuelCan == null)
            return;

        if (!heldFuelCan.HasFuel())
        {
            if (debugMessages)
                Debug.Log("Fuel can is empty.");

            return;
        }

        VehicleFuel vehicleFuel = FindNearestVehicleFuel();

        if (vehicleFuel == null)
        {
            if (debugMessages)
                Debug.Log("No car/helicopter close enough to refuel.");

            return;
        }

        if (vehicleFuel.currentFuel >= vehicleFuel.maxFuel)
        {
            if (debugMessages)
                Debug.Log(vehicleFuel.gameObject.name + " fuel is already full.");

            return;
        }

        float fuelThisFrame = refuelLitersPerSecond * Time.deltaTime;

        float beforeVehicleFuel = vehicleFuel.currentFuel;

        float fuelTakenFromCan = heldFuelCan.TakeFuel(fuelThisFrame);

        float addedToVehicle = vehicleFuel.Refuel(fuelTakenFromCan);

        // If vehicle accepted less fuel, return unused fuel to can
        float unusedFuel = fuelTakenFromCan - addedToVehicle;

        if (unusedFuel > 0f)
            heldFuelCan.AddFuel(unusedFuel);

        if (addedToVehicle > 0f)
        {
            isRefueling = true;

            if (debugMessages && Mathf.FloorToInt(beforeVehicleFuel) != Mathf.FloorToInt(vehicleFuel.currentFuel))
            {
                Debug.Log(
                    "Refueling " + vehicleFuel.gameObject.name +
                    " | Vehicle: " + Mathf.RoundToInt(vehicleFuel.currentFuel) +
                    "/" + Mathf.RoundToInt(vehicleFuel.maxFuel) +
                    " | Can: " + Mathf.RoundToInt(heldFuelCan.currentLiters)
                );
            }
        }
    }

    private VehicleFuel FindNearestVehicleFuel()
    {
        Vector3 searchPosition = transform.position;

        if (heldFuelCan != null)
            searchPosition = heldFuelCan.transform.position;

        Collider[] hits = Physics.OverlapSphere(searchPosition, refuelDistance, vehicleMask, QueryTriggerInteraction.Collide);

        VehicleFuel nearestFuel = null;
        float nearestDistance = Mathf.Infinity;

        foreach (Collider hit in hits)
        {
            VehicleFuel fuel = hit.GetComponentInParent<VehicleFuel>();

            if (fuel == null)
                continue;

            float distance = Vector3.Distance(searchPosition, fuel.transform.position);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestFuel = fuel;
            }
        }

        return nearestFuel;
    }

    private void UpdateHeldFuelCanVisual()
    {
        if (heldFuelCan == null)
            return;

        Vector3 targetPosition = isRefueling ? refuelLocalPosition : normalLocalPosition;
        Vector3 targetEuler = isRefueling ? refuelLocalEuler : normalLocalEuler;

        if (isRefueling)
        {
            float wobble = Mathf.Sin(Time.time * refuelWobbleSpeed) * refuelWobbleAmount;
            targetEuler.z += wobble;
        }

        heldFuelCan.transform.localPosition = Vector3.Lerp(
            heldFuelCan.transform.localPosition,
            targetPosition,
            rotateSpeed * Time.deltaTime
        );

        heldFuelCan.transform.localRotation = Quaternion.Slerp(
            heldFuelCan.transform.localRotation,
            Quaternion.Euler(targetEuler),
            rotateSpeed * Time.deltaTime
        );
    }
}