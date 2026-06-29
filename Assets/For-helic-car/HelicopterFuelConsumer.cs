using UnityEngine;

public class HelicopterFuelConsumer : MonoBehaviour
{
    [Header("References")]
    public HelicopterController helicopterController;
    public VehicleFuel fuel;

    [Header("Fuel Use Per Second")]
    public float hoverFuelUse = 0.08f;
    public float moveFuelUse = 0.18f;
    public float upDownFuelUse = 0.15f;
    public float boostFuelUse = 0.25f;

    private void Awake()
    {
        if (helicopterController == null)
            helicopterController = GetComponent<HelicopterController>();

        if (fuel == null)
            fuel = GetComponent<VehicleFuel>();
    }

    private void FixedUpdate()
    {
        if (helicopterController == null || fuel == null)
            return;

        if (!helicopterController.canFly)
            return;

        if (!fuel.HasFuel())
        {
            helicopterController.EngineOffAndFall();
            return;
        }

        float fuelToUse = hoverFuelUse;

        float forwardInput = Mathf.Abs(Input.GetAxis("Vertical"));
        float turnInput = Mathf.Abs(Input.GetAxis("Horizontal"));

        bool moving = forwardInput > 0.1f || turnInput > 0.1f;

        bool upOrDown =
            Input.GetKey(KeyCode.Space) ||
            Input.GetKey(KeyCode.LeftControl) ||
            Input.GetKey(KeyCode.C);

        bool boosting = Input.GetKey(KeyCode.LeftShift);

        if (moving)
            fuelToUse += moveFuelUse;

        if (upOrDown)
            fuelToUse += upDownFuelUse;

        if (boosting)
            fuelToUse += boostFuelUse;

        bool stillHasFuel = fuel.UseFuel(fuelToUse * Time.fixedDeltaTime);

        if (!stillHasFuel)
            helicopterController.EngineOffAndFall();
    }
}