using UnityEngine;

public class VehicleFuel : MonoBehaviour
{
    [Header("Fuel")]
    public float maxFuel = 50f;
    public float currentFuel = 50f;

    [Header("Debug")]
    public bool debugMessages = true;

    public float FuelPercent
    {
        get
        {
            if (maxFuel <= 0f)
                return 0f;

            return currentFuel / maxFuel;
        }
    }

    private void Awake()
    {
        currentFuel = Mathf.Clamp(currentFuel, 0f, maxFuel);
    }

    private void OnValidate()
    {
        currentFuel = Mathf.Clamp(currentFuel, 0f, maxFuel);
    }

    public bool HasFuel()
    {
        return currentFuel > 0.01f;
    }

    public bool UseFuel(float liters)
    {
        if (liters <= 0f)
            return HasFuel();

        if (currentFuel <= 0f)
        {
            currentFuel = 0f;
            return false;
        }

        currentFuel -= liters;

        if (currentFuel <= 0f)
        {
            currentFuel = 0f;

            if (debugMessages)
                Debug.Log(gameObject.name + " is out of fuel.");

            return false;
        }

        return true;
    }

    public float Refuel(float liters)
    {
        if (liters <= 0f)
            return 0f;

        if (currentFuel >= maxFuel)
        {
            if (debugMessages)
                Debug.Log(gameObject.name + " fuel already full.");

            return 0f;
        }

        float before = currentFuel;

        currentFuel += liters;

        if (currentFuel > maxFuel)
            currentFuel = maxFuel;

        float added = currentFuel - before;

        if (debugMessages)
            Debug.Log(gameObject.name + " refueled +" + added + "L. Fuel: " + currentFuel + " / " + maxFuel);

        return added;
    }
}