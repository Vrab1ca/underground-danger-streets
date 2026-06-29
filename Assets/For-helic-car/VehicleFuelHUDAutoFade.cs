using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VehicleFuelHUDAutoFade : MonoBehaviour
{
    [Header("UI")]
    public CanvasGroup fuelCanvasGroup;
    public Slider fuelSlider;
    public TMP_Text fuelText;

    [Header("Helicopter")]
    public HelicopterEnterExit helicopterEnterExit;

    [Header("Car")]
    public CarEnterExit carEnterExit;

    [Header("Fade")]
    public float fadeInSpeed = 8f;
    public float fadeOutSpeed = 2f;

    private VehicleFuel currentFuel;
    private string currentVehicleName;

    private void Start()
    {
        if (fuelSlider != null)
        {
            fuelSlider.minValue = 0f;
            fuelSlider.maxValue = 1f;
            fuelSlider.interactable = false;
        }

        if (fuelCanvasGroup != null)
        {
            fuelCanvasGroup.alpha = 0f;
            fuelCanvasGroup.interactable = false;
            fuelCanvasGroup.blocksRaycasts = false;
        }
    }

    private void Update()
    {
        FindCurrentVehicle();
        UpdateFuelUI();
        UpdateFade();
    }

    private void FindCurrentVehicle()
    {
        currentFuel = null;
        currentVehicleName = "";

        if (helicopterEnterExit != null && helicopterEnterExit.PlayerInside)
        {
            currentFuel = helicopterEnterExit.GetComponent<VehicleFuel>();
            currentVehicleName = "Helicopter Fuel";
            return;
        }

        if (carEnterExit != null && carEnterExit.PlayerInside)
        {
            currentFuel = carEnterExit.GetComponent<VehicleFuel>();
            currentVehicleName = "Car Fuel";
            return;
        }
    }

    private void UpdateFuelUI()
    {
        if (currentFuel == null)
            return;

        if (fuelSlider != null)
            fuelSlider.value = currentFuel.FuelPercent;

        if (fuelText != null)
        {
            fuelText.text =
                currentVehicleName + ": " +
                Mathf.RoundToInt(currentFuel.currentFuel) +
                " / " +
                Mathf.RoundToInt(currentFuel.maxFuel) +
                " L";
        }
    }

    private void UpdateFade()
    {
        if (fuelCanvasGroup == null)
            return;

        float targetAlpha = currentFuel != null ? 1f : 0f;

        float speed = targetAlpha > fuelCanvasGroup.alpha
            ? fadeInSpeed
            : fadeOutSpeed;

        fuelCanvasGroup.alpha = Mathf.MoveTowards(
            fuelCanvasGroup.alpha,
            targetAlpha,
            speed * Time.deltaTime
        );
    }
}