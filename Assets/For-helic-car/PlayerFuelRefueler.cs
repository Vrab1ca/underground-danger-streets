using UnityEngine;

public class PlayerFuelRefueler : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;

    [Header("Keys")]
    public KeyCode refuelKey = KeyCode.F;

    [Header("Distances")]
    public float fuelCanInteractDistance = 3f;
    public float vehicleRefuelDistance = 6f;

    [Header("Layers")]
    public LayerMask fuelCanMask = ~0;
    public LayerMask vehicleMask = ~0;

    private void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetKeyDown(refuelKey))
            TryRefuelVehicle();
    }

    private void TryRefuelVehicle()
    {
        FuelCan fuelCan = FindFuelCan();

        if (fuelCan == null)
        {
            Debug.Log("No fuel can nearby or in front.");
            return;
        }

        VehicleFuel vehicleFuel = FindNearestVehicleFuel(fuelCan.transform.position);

        if (vehicleFuel == null)
        {
            Debug.Log("No car/helicopter near fuel can.");
            return;
        }

        float added = vehicleFuel.Refuel(fuelCan.liters);

        if (added > 0f)
        {
            fuelCan.UseCan();
            Debug.Log("Used fuel can. Added: " + added + " liters.");
        }
        else
        {
            Debug.Log("Vehicle fuel is already full.");
        }
    }

    private FuelCan FindFuelCan()
    {
        if (playerCamera != null)
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, fuelCanInteractDistance, fuelCanMask, QueryTriggerInteraction.Collide))
            {
                FuelCan fuelCan = hit.collider.GetComponentInParent<FuelCan>();

                if (fuelCan != null)
                    return fuelCan;
            }
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, fuelCanInteractDistance, fuelCanMask, QueryTriggerInteraction.Collide);

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

    private VehicleFuel FindNearestVehicleFuel(Vector3 fuelCanPosition)
    {
        Collider[] hits = Physics.OverlapSphere(fuelCanPosition, vehicleRefuelDistance, vehicleMask, QueryTriggerInteraction.Collide);

        VehicleFuel nearestFuel = null;
        float nearestDistance = Mathf.Infinity;

        foreach (Collider hit in hits)
        {
            VehicleFuel fuel = hit.GetComponentInParent<VehicleFuel>();

            if (fuel == null)
                continue;

            float distance = Vector3.Distance(fuelCanPosition, fuel.transform.position);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestFuel = fuel;
            }
        }

        return nearestFuel;
    }
}