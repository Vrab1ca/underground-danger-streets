using UnityEngine;

public class VehicleHUDConnector : MonoBehaviour
{
    public enum VehicleType
    {
        Car,
        Helicopter
    }

    [Header("Vehicle Type")]
    public VehicleType vehicleType = VehicleType.Car;

    [Header("Fuel")]
    public float currentFuel = 100f;
    public float maxFuel = 100f;
    public bool useFuel = true;
    public float fuelUsePerSecond = 2f;

    [Header("Helicopter Bombs")]
    public int currentBombs = 5;
    public int maxBombs = 5;
    public KeyCode dropBombKey = KeyCode.B;

    [Header("Enter / Exit")]
    public KeyCode enterExitKey = KeyCode.E;
    public float enterDistance = 4f;
    public bool playerInside;

    [Header("Prompt")]
    public string enterText = "Press E to enter vehicle";
    public string exitText = "Press E to exit vehicle";

    [Header("Debug")]
    public bool debugMessages = true;

    private Transform player;

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
            player = playerObject.transform;
        else
            Debug.LogWarning(gameObject.name + " cannot find Player tag.");
    }

    private void Update()
    {
        if (player == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (!playerInside)
        {
            if (distance <= enterDistance)
            {
                ShowPrompt(enterText);

                if (Input.GetKeyDown(enterExitKey))
                    EnterVehicle();
            }
            else
            {
                HidePrompt();
            }

            return;
        }

        ShowPrompt(exitText);

        if (Input.GetKeyDown(enterExitKey))
        {
            ExitVehicle();
            return;
        }

        UpdateFuel();
        UpdateVehicleHUD();
        HandleHelicopterBombs();
    }

    private void EnterVehicle()
    {
        playerInside = true;

        if (debugMessages)
            Debug.Log("Entered vehicle: " + vehicleType);

        UpdateVehicleHUD();
    }

    private void ExitVehicle()
    {
        playerInside = false;

        if (ManualFPSHUDUI.Instance != null)
        {
            if (vehicleType == VehicleType.Car)
                ManualFPSHUDUI.Instance.HideCarHUD();

            if (vehicleType == VehicleType.Helicopter)
                ManualFPSHUDUI.Instance.HideHelicopterHUD();

            ManualFPSHUDUI.Instance.HideInteraction();
        }

        if (debugMessages)
            Debug.Log("Exited vehicle: " + vehicleType);
    }

    private void UpdateFuel()
    {
        if (!useFuel)
            return;

        if (currentFuel <= 0f)
        {
            currentFuel = 0f;
            return;
        }

        currentFuel -= fuelUsePerSecond * Time.deltaTime;
        currentFuel = Mathf.Clamp(currentFuel, 0f, maxFuel);
    }

    private void UpdateVehicleHUD()
    {
        if (ManualFPSHUDUI.Instance == null)
            return;

        if (vehicleType == VehicleType.Car)
        {
            ManualFPSHUDUI.Instance.ShowCarFuel(currentFuel, maxFuel);
        }

        if (vehicleType == VehicleType.Helicopter)
        {
            ManualFPSHUDUI.Instance.ShowHelicopterHUD(
                currentFuel,
                maxFuel,
                currentBombs,
                maxBombs
            );
        }
    }

    private void HandleHelicopterBombs()
    {
        if (vehicleType != VehicleType.Helicopter)
            return;

        if (!Input.GetKeyDown(dropBombKey))
            return;

        if (currentBombs <= 0)
        {
            Debug.Log("No helicopter bombs left.");
            return;
        }

        currentBombs--;

        Debug.Log("Dropped helicopter bomb. Bombs left: " + currentBombs);

        UpdateVehicleHUD();
    }

    private void ShowPrompt(string message)
    {
        if (ManualFPSHUDUI.Instance != null)
            ManualFPSHUDUI.Instance.ShowInteraction(message);
    }

    private void HidePrompt()
    {
        if (ManualFPSHUDUI.Instance != null)
            ManualFPSHUDUI.Instance.HideInteraction();
    }

    private void OnDisable()
    {
        if (ManualFPSHUDUI.Instance != null)
        {
            ManualFPSHUDUI.Instance.HideInteraction();
            ManualFPSHUDUI.Instance.HideVehicleHUD();
        }
    }
}