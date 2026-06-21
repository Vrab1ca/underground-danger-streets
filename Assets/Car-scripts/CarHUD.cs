using TMPro;
using UnityEngine;

public class CarHUD : MonoBehaviour
{
    [Header("References")]
    public CarController carController;
    public CarEnterExit carEnterExit;

    [Header("UI Texts")]
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI rpmText;
    public TextMeshProUGUI gearText;
    public TextMeshProUGUI boostText;
    public TextMeshProUGUI driftText;
    public TextMeshProUGUI helpText;

    private void Update()
    {
        if (carController == null || carEnterExit == null)
            return;

        bool insideCar = carEnterExit.PlayerInside;

        UpdateTextVisibility(insideCar);

        if (insideCar)
        {
            speedText.text = Mathf.RoundToInt(carController.speedKmh) + " KM/H";
            rpmText.text = "RPM: " + Mathf.RoundToInt(carController.rpm);
            gearText.text = "Gear: " + carController.gear;

            boostText.text = carController.isBoosting ? "BOOST" : "";
            driftText.text = carController.isDrifting ? "DRIFTING" : "";

            helpText.text = "W/S - Drive | A/D - Steer | Shift - Boost | Space - Drift | E - Exit | Mouse - Shoot";
        }
        else
        {
            helpText.text = "Go near the car and press E";
        }
    }

    private void UpdateTextVisibility(bool insideCar)
    {
        if (speedText != null)
            speedText.gameObject.SetActive(insideCar);

        if (rpmText != null)
            rpmText.gameObject.SetActive(insideCar);

        if (gearText != null)
            gearText.gameObject.SetActive(insideCar);

        if (boostText != null)
            boostText.gameObject.SetActive(insideCar);

        if (driftText != null)
            driftText.gameObject.SetActive(insideCar);

        if (helpText != null)
            helpText.gameObject.SetActive(true);
    }
}