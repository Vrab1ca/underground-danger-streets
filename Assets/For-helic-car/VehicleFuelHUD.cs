using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VehicleFuelHUD : MonoBehaviour
{
    [Header("References")]
    public VehicleFuel targetFuel;
    public Slider fuelSlider;
    public TMP_Text fuelText;

    [Header("Text")]
    public string fuelName = "Fuel";

    private void Start()
    {
        if (fuelSlider != null)
        {
            fuelSlider.minValue = 0f;
            fuelSlider.maxValue = 1f;
            fuelSlider.interactable = false;
        }
    }

    private void Update()
    {
        if (targetFuel == null || fuelSlider == null)
            return;

        fuelSlider.value = targetFuel.FuelPercent;

        if (fuelText != null)
        {
            fuelText.text =
                fuelName + ": " +
                Mathf.RoundToInt(targetFuel.currentFuel) +
                " / " +
                Mathf.RoundToInt(targetFuel.maxFuel) +
                " L";
        }
    }

    public void SetTargetFuel(VehicleFuel newTarget)
    {
        targetFuel = newTarget;
    }
}