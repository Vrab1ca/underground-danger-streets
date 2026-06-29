using UnityEngine;

public class CarFuelConsumer : MonoBehaviour
{
    [Header("References")]
    public VehicleFuel fuel;

    [Tooltip("Drag your car controller script here if it is enabled only when player drives.")]
    public MonoBehaviour carControllerScript;

    [Header("Fuel Use")]
    public float drivingFuelUsePerSecond = 0.12f;
    public float boostFuelUsePerSecond = 0.2f;

    [Header("State")]
    public bool playerInside;

    [Header("Keys")]
    public KeyCode boostKey = KeyCode.LeftShift;

    private void Awake()
    {
        if (fuel == null)
            fuel = GetComponent<VehicleFuel>();
    }

    private void Update()
    {
        if (fuel == null)
            return;

        bool carControlActive = playerInside;

        if (carControllerScript != null && carControllerScript.enabled)
            carControlActive = true;

        if (!carControlActive)
            return;

        if (!fuel.HasFuel())
        {
            if (carControllerScript != null)
                carControllerScript.enabled = false;

            Debug.Log("Car is out of fuel.");
            return;
        }

        float vertical = Mathf.Abs(Input.GetAxis("Vertical"));
        float horizontal = Mathf.Abs(Input.GetAxis("Horizontal"));

        bool driving = vertical > 0.1f || horizontal > 0.1f;

        if (!driving)
            return;

        float fuelToUse = drivingFuelUsePerSecond;

        if (Input.GetKey(boostKey))
            fuelToUse += boostFuelUsePerSecond;

        fuel.UseFuel(fuelToUse * Time.deltaTime);
    }

    public void SetPlayerInside(bool inside)
    {
        playerInside = inside;
    }
}