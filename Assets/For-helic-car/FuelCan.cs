using UnityEngine;

public class FuelCan : MonoBehaviour
{
    [Header("Fuel Can")]
    public float maxLiters = 10f;
    public float currentLiters = 10f;

    [Header("Empty Can")]
    public bool destroyWhenEmpty = false;

    public bool HasFuel()
    {
        return currentLiters > 0.01f;
    }

    public float TakeFuel(float amount)
    {
        if (amount <= 0f)
            return 0f;

        if (currentLiters <= 0f)
        {
            currentLiters = 0f;
            return 0f;
        }

        float taken = Mathf.Min(amount, currentLiters);

        currentLiters -= taken;

        if (currentLiters <= 0f)
        {
            currentLiters = 0f;

            if (destroyWhenEmpty)
                Destroy(gameObject);
        }

        return taken;
    }

    public void AddFuel(float amount)
    {
        if (amount <= 0f)
            return;

        currentLiters += amount;

        if (currentLiters > maxLiters)
            currentLiters = maxLiters;
    }

    private void OnValidate()
    {
        currentLiters = Mathf.Clamp(currentLiters, 0f, maxLiters);
    }
}