using UnityEngine;

public class BetterCarController : MonoBehaviour
{
    [Header("Wheel Colliders")]
    public WheelCollider frontLeftCollider;
    public WheelCollider frontRightCollider;
    public WheelCollider rearLeftCollider;
    public WheelCollider rearRightCollider;

    [Header("Wheel Visuals")]
    public Transform frontLeftVisual;
    public Transform frontRightVisual;
    public Transform rearLeftVisual;
    public Transform rearRightVisual;

    [Header("Car Power")]
    public float motorForce = 2200f;
    public float brakeForce = 4500f;
    public float handbrakeForce = 7000f;
    public float maxSpeed = 38f;

    [Header("Steering")]
    public float maxSteerAngle = 32f;
    public float steeringSmoothness = 7f;
    public float highSpeedSteerLimit = 12f;

    [Header("Stability")]
    public Vector3 centerOfMassOffset = new Vector3(0f, -0.7f, 0f);
    public float downForce = 120f;
    public float antiRollForce = 6000f;

    [Header("Traction")]
    public float forwardStiffness = 1.6f;
    public float sidewaysStiffness = 1.9f;

    [Header("State")]
    public bool canDrive = false;
    public bool broken = false;

    private Rigidbody rb;
    private float currentSteerAngle;
    private float throttleInput;
    private float steerInput;
    private bool brakeInput;
    private bool handbrakeInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.centerOfMass += centerOfMassOffset;
        }

        SetupWheelFriction(frontLeftCollider);
        SetupWheelFriction(frontRightCollider);
        SetupWheelFriction(rearLeftCollider);
        SetupWheelFriction(rearRightCollider);
    }

    void FixedUpdate()
    {
        if (broken)
        {
            ApplyAllBrakes(brakeForce);
            return;
        }

        if (!canDrive)
        {
            ApplyAllBrakes(brakeForce * 0.3f);
            return;
        }

        ReadInput();
        HandleMotor();
        HandleSteering();
        ApplyDownforce();
        ApplyAntiRoll(frontLeftCollider, frontRightCollider);
        ApplyAntiRoll(rearLeftCollider, rearRightCollider);
        LimitSpeed();
    }

    void Update()
    {
        UpdateWheelVisual(frontLeftCollider, frontLeftVisual);
        UpdateWheelVisual(frontRightCollider, frontRightVisual);
        UpdateWheelVisual(rearLeftCollider, rearLeftVisual);
        UpdateWheelVisual(rearRightCollider, rearRightVisual);
    }

    void ReadInput()
    {
        throttleInput = Input.GetAxis("Vertical");
        steerInput = Input.GetAxis("Horizontal");

        brakeInput = Input.GetKey(KeyCode.S);
        handbrakeInput = Input.GetKey(KeyCode.Space);
    }

    void HandleMotor()
    {
        float speed = rb.linearVelocity.magnitude;

        float motor = throttleInput * motorForce;

        if (speed >= maxSpeed && throttleInput > 0f)
            motor = 0f;

        rearLeftCollider.motorTorque = motor;
        rearRightCollider.motorTorque = motor;

        float brake = 0f;

        if (brakeInput && Vector3.Dot(rb.linearVelocity, transform.forward) > 1f)
            brake = brakeForce;

        if (handbrakeInput)
            brake = handbrakeForce;

        frontLeftCollider.brakeTorque = brake;
        frontRightCollider.brakeTorque = brake;

        if (handbrakeInput)
        {
            rearLeftCollider.brakeTorque = handbrakeForce;
            rearRightCollider.brakeTorque = handbrakeForce;
        }
        else
        {
            rearLeftCollider.brakeTorque = brake;
            rearRightCollider.brakeTorque = brake;
        }
    }

    void HandleSteering()
    {
        float speedPercent = Mathf.Clamp01(rb.linearVelocity.magnitude / maxSpeed);

        float allowedSteer = Mathf.Lerp(maxSteerAngle, highSpeedSteerLimit, speedPercent);

        float targetSteer = steerInput * allowedSteer;

        currentSteerAngle = Mathf.Lerp(
            currentSteerAngle,
            targetSteer,
            steeringSmoothness * Time.fixedDeltaTime
        );

        frontLeftCollider.steerAngle = currentSteerAngle;
        frontRightCollider.steerAngle = currentSteerAngle;
    }

    void ApplyAllBrakes(float amount)
    {
        frontLeftCollider.brakeTorque = amount;
        frontRightCollider.brakeTorque = amount;
        rearLeftCollider.brakeTorque = amount;
        rearRightCollider.brakeTorque = amount;

        frontLeftCollider.motorTorque = 0f;
        frontRightCollider.motorTorque = 0f;
        rearLeftCollider.motorTorque = 0f;
        rearRightCollider.motorTorque = 0f;
    }

    void ApplyDownforce()
    {
        if (rb == null)
            return;

        rb.AddForce(-transform.up * downForce * rb.linearVelocity.magnitude);
    }

    void ApplyAntiRoll(WheelCollider leftWheel, WheelCollider rightWheel)
    {
        WheelHit hit;

        float travelLeft = 1f;
        float travelRight = 1f;

        bool groundedLeft = leftWheel.GetGroundHit(out hit);

        if (groundedLeft)
        {
            travelLeft = (-leftWheel.transform.InverseTransformPoint(hit.point).y - leftWheel.radius)
                         / leftWheel.suspensionDistance;
        }

        bool groundedRight = rightWheel.GetGroundHit(out hit);

        if (groundedRight)
        {
            travelRight = (-rightWheel.transform.InverseTransformPoint(hit.point).y - rightWheel.radius)
                          / rightWheel.suspensionDistance;
        }

        float antiRoll = (travelLeft - travelRight) * antiRollForce;

        if (groundedLeft)
            rb.AddForceAtPosition(leftWheel.transform.up * -antiRoll, leftWheel.transform.position);

        if (groundedRight)
            rb.AddForceAtPosition(rightWheel.transform.up * antiRoll, rightWheel.transform.position);
    }

    void LimitSpeed()
    {
        if (rb == null)
            return;

        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }

    void SetupWheelFriction(WheelCollider wheel)
    {
        if (wheel == null)
            return;

        WheelFrictionCurve forward = wheel.forwardFriction;
        forward.stiffness = forwardStiffness;
        wheel.forwardFriction = forward;

        WheelFrictionCurve sideways = wheel.sidewaysFriction;
        sideways.stiffness = sidewaysStiffness;
        wheel.sidewaysFriction = sideways;
    }

    void UpdateWheelVisual(WheelCollider wheelCollider, Transform wheelVisual)
    {
        if (wheelCollider == null || wheelVisual == null)
            return;

        wheelCollider.GetWorldPose(out Vector3 position, out Quaternion rotation);

        wheelVisual.position = position;
        wheelVisual.rotation = rotation;
    }

    public void SetDriving(bool value)
    {
        if (broken)
        {
            canDrive = false;
            return;
        }

        canDrive = value;
    }

    public void BreakCar()
    {
        broken = true;
        canDrive = false;
        ApplyAllBrakes(brakeForce);
    }

    public float GetSpeedKmh()
    {
        if (rb == null)
            return 0f;

        return rb.linearVelocity.magnitude * 3.6f;
    }
}