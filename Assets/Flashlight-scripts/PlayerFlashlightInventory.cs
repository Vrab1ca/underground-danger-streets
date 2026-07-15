using UnityEngine;

public enum FlashlightBatteryType
{
    A,
    AA,
    AAA
}

public class PlayerFlashlightInventory : MonoBehaviour
{
    [Header("Flashlight")]
    [SerializeField]
    private bool hasFlashlight;

    [SerializeField]
    [Range(0f, 100f)]
    private float currentCharge;

    [Header("Battery Charge Amounts")]
    [Tooltip("A battery adds 20 percent charge.")]
    [Min(0f)]
    public float aChargeAmount = 20f;

    [Tooltip("AA battery adds 50 percent charge.")]
    [Min(0f)]
    public float aaChargeAmount = 50f;

    [Tooltip("AAA battery adds 100 percent charge.")]
    [Min(0f)]
    public float aaaChargeAmount = 100f;

    [Header("Debug")]
    public bool debugMessages = true;

    public bool HasFlashlight
    {
        get { return hasFlashlight; }
    }

    public float CurrentCharge
    {
        get { return currentCharge; }
    }

    public bool HasCharge
    {
        get { return currentCharge > 0.01f; }
    }

    private void OnValidate()
    {
        currentCharge = Mathf.Clamp(
            currentCharge,
            0f,
            100f
        );

        aChargeAmount = Mathf.Max(0f, aChargeAmount);
        aaChargeAmount = Mathf.Max(0f, aaChargeAmount);
        aaaChargeAmount = Mathf.Max(0f, aaaChargeAmount);
    }

    public bool AddFlashlight(float startingCharge)
    {
        if (hasFlashlight)
        {
            if (debugMessages)
                Debug.Log("You already have a flashlight.");

            return false;
        }

        hasFlashlight = true;
        currentCharge = Mathf.Clamp(
            startingCharge,
            0f,
            100f
        );

        if (debugMessages)
        {
            Debug.Log(
                "FLASHLIGHT COLLECTED | Charge: " +
                Mathf.CeilToInt(currentCharge) + "%"
            );
        }

        return true;
    }

    public bool RemoveFlashlight(out float remainingCharge)
    {
        remainingCharge = currentCharge;

        if (!hasFlashlight)
        {
            remainingCharge = 0f;
            return false;
        }

        hasFlashlight = false;
        currentCharge = 0f;

        if (debugMessages)
        {
            Debug.Log(
                "FLASHLIGHT REMOVED FROM INVENTORY | Dropped charge: " +
                Mathf.CeilToInt(remainingCharge) + "%"
            );
        }

        return true;
    }

    public float GetBatteryChargeAmount(
        FlashlightBatteryType batteryType
    )
    {
        switch (batteryType)
        {
            case FlashlightBatteryType.A:
                return aChargeAmount;

            case FlashlightBatteryType.AA:
                return aaChargeAmount;

            case FlashlightBatteryType.AAA:
                return aaaChargeAmount;

            default:
                return 0f;
        }
    }

    public bool InstallBattery(
        FlashlightBatteryType batteryType
    )
    {
        if (!hasFlashlight)
        {
            if (debugMessages)
            {
                Debug.Log(
                    "You cannot use the battery because " +
                    "you do not have a flashlight."
                );
            }

            return false;
        }

        if (currentCharge >= 99.99f)
        {
            if (debugMessages)
            {
                Debug.Log(
                    "Flashlight charge is already full. " +
                    "The battery was not used."
                );
            }

            return false;
        }

        float addedCharge =
            GetBatteryChargeAmount(batteryType);

        if (addedCharge <= 0f)
            return false;

        float chargeBefore = currentCharge;

        currentCharge = Mathf.Clamp(
            currentCharge + addedCharge,
            0f,
            100f
        );

        if (debugMessages)
        {
            Debug.Log(
                "INSTALLED " + batteryType +
                " BATTERY | " +
                Mathf.CeilToInt(chargeBefore) + "% -> " +
                Mathf.CeilToInt(currentCharge) + "%"
            );
        }

        return true;
    }

    public bool DrainCharge(float amount)
    {
        if (!hasFlashlight || currentCharge <= 0f)
        {
            currentCharge = 0f;
            return false;
        }

        currentCharge = Mathf.Max(
            0f,
            currentCharge - Mathf.Max(0f, amount)
        );

        return currentCharge > 0.01f;
    }
}