using TMPro;
using UnityEngine;

public class CarHUD : MonoBehaviour
{
    [Header("References")]
    public CarController carController;
    public CarEnterExit carEnterExit;

    [Header("UI Text")]
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI gearText;
    public TextMeshProUGUI rpmText;
    public TextMeshProUGUI driftText;
    public TextMeshProUGUI helpText;

    private void Update()
    {
        if (carController == null || carEnterExit == null)
            return;

        bool insideCar = carEnterExit.PlayerInside;

        if (speedText != null)
        {
            speedText.gameObject.SetActive(insideCar);
            speedText.text = Mathf.RoundToInt(carController.speedKmh) + " KM/H";
        }

        if (gearText != null)
        {
            gearText.gameObject.SetActive(insideCar);
            gearText.text = "Gear: " + carController.gear;
        }

        if (rpmText != null)
        {
            rpmText.gameObject.SetActive(insideCar);
            rpmText.text = "RPM: " + Mathf.RoundToInt(carController.rpm);
        }

        if (driftText != null)
        {
            driftText.gameObject.SetActive(insideCar);
            driftText.text = carController.isDrifting ? "DRIFTING" : "";
        }

        if (helpText != null)
        {
            if (insideCar)
            {
                helpText.text = "E - Exit | W/S - Drive | A/D - Steer | Space - Drift | Shift - Brake";
            }
            else
            {
                helpText.text = "Go near the car and press E";
            }
        }
    }
}