using UnityEngine;

public class FuelCan : MonoBehaviour
{
    [Header("Fuel Can")]
    public float liters = 10f;

    [Header("Settings")]
    public bool destroyAfterUse = true;

    public void UseCan()
    {
        if (destroyAfterUse)
            Destroy(gameObject);
    }
}