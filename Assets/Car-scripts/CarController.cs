using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Wheel Colliders")]
    public WheelCollider frontLeftCollider;
    public WheelCollider frontRightCollider;
    public WheelCollider rearLeftCollider;
    public WheelCollider rearRightCollider;

    [Header("Wheel Meshes")]
    public Transform frontLeftMesh;
    public Transform frontRightMesh;
    public Transform rearLeftMesh;
    public Transform rearRightMesh;

    [Header("Car Settings")]
    public float motorPower = 1800f;
    public float brakePower = 4000f;
    public float handbrakePower = 7000f;
    public float maxSteerAngle = 32f;
    public float normalRearGrip = 1.3f;
    public float driftRearGrip = 0.45f;
    public float downForce = 25f;

    [Header("Speed Settings")]
    public float normalMaxSpeed = 120f;
    public float boostMaxSpeed = 190f;
    public float boostMultiplier = 2f;

    [Header("Controls Fix")]
    public bool invertMotorDirection = true;

    [Header("Center Of Mass")]
    public Transform centerOfMass;

    [Header("Car Info")]
    public bool canDrive;
    public float speedKmh;
    public float rpm;
    public int gear = 1;
    public bool isDrifting;
    public bool isBoosting;

    private Rigidbody rb;

    private float horizontalInput;
    private float verticalInput;
    private bool handbrakeInput;
    private bool boostInput;

    private WheelFrictionCurve rearLeftSidewaysFriction;
    private WheelFrictionCurve rearRightSidewaysFriction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (centerOfMass != null)
        {
            rb.centerOfMass = centerOfMass.localPosition;
        }

        rearLeftSidewaysFriction = rearLeftCollider.sidewaysFriction;
        rearRightSidewaysFriction = rearRightCollider.sidewaysFriction;

        canDrive = false;
    }

    private void Update()
    {
        CalculateCarInfo();
        UpdateWheelMeshes();

        if (!canDrive)
        {
            horizontalInput = 0f;
            verticalInput = 0f;
            handbrakeInput = false;
            boostInput = false;
            isBoosting = false;
            return;
        }

        GetInput();
    }

    private void FixedUpdate()
    {
        if (!canDrive)
        {
            StopCar();
            return;
        }

        HandleMotor();
        HandleSteering();
        HandleHandbrake();
        HandleDrift();
        AddDownForce();
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal"); // A / D
        verticalInput = Input.GetAxis("Vertical");     // W / S

        handbrakeInput = Input.GetKey(KeyCode.Space);
        boostInput = Input.GetKey(KeyCode.LeftShift);

        isBoosting = boostInput && verticalInput > 0f;
    }

    private void HandleMotor()
    {
        float currentMotorPower = isBoosting ? motorPower * boostMultiplier : motorPower;
        float currentMaxSpeed = isBoosting ? boostMaxSpeed : normalMaxSpeed;

        float input = verticalInput;

        // This fixes your problem: W goes forward, S goes backward.
        if (invertMotorDirection)
        {
            input *= -1f;
        }

        float torque = input * currentMotorPower;

        // Stop adding power after max speed
        if (speedKmh > currentMaxSpeed && verticalInput > 0f)
        {
            torque = 0f;
        }

        rearLeftCollider.motorTorque = torque;
        rearRightCollider.motorTorque = torque;
    }

    private void HandleSteering()
    {
        float steerAngle = horizontalInput * maxSteerAngle;

        frontLeftCollider.steerAngle = steerAngle;
        frontRightCollider.steerAngle = steerAngle;
    }

    private void HandleHandbrake()
    {
        frontLeftCollider.brakeTorque = 0f;
        frontRightCollider.brakeTorque = 0f;
        rearLeftCollider.brakeTorque = 0f;
        rearRightCollider.brakeTorque = 0f;

        if (handbrakeInput)
        {
            rearLeftCollider.brakeTorque = handbrakePower;
            rearRightCollider.brakeTorque = handbrakePower;
        }
    }

    private void HandleDrift()
    {
        isDrifting = handbrakeInput;

        float rearGrip = isDrifting ? driftRearGrip : normalRearGrip;

        rearLeftSidewaysFriction.stiffness = rearGrip;
        rearRightSidewaysFriction.stiffness = rearGrip;

        rearLeftCollider.sidewaysFriction = rearLeftSidewaysFriction;
        rearRightCollider.sidewaysFriction = rearRightSidewaysFriction;
    }

    private void StopCar()
    {
        frontLeftCollider.motorTorque = 0f;
        frontRightCollider.motorTorque = 0f;
        rearLeftCollider.motorTorque = 0f;
        rearRightCollider.motorTorque = 0f;

        frontLeftCollider.brakeTorque = 0f;
        frontRightCollider.brakeTorque = 0f;
        rearLeftCollider.brakeTorque = 0f;
        rearRightCollider.brakeTorque = 0f;
    }

    private void AddDownForce()
    {
        rb.AddForce(-transform.up * speedKmh * downForce);
    }

    private void CalculateCarInfo()
    {
        speedKmh = rb.linearVelocity.magnitude * 3.6f;

        gear = Mathf.Clamp(Mathf.FloorToInt(speedKmh / 35f) + 1, 1, 6);

        if (canDrive)
        {
            float gearStartSpeed = (gear - 1) * 35f;
            float gearEndSpeed = gear * 35f;
            float gearPercent = Mathf.InverseLerp(gearStartSpeed, gearEndSpeed, speedKmh);

            rpm = Mathf.Lerp(900f, 7000f, gearPercent);
        }
        else
        {
            rpm = 0f;
        }
    }

    private void UpdateWheelMeshes()
    {
        UpdateOneWheel(frontLeftCollider, frontLeftMesh);
        UpdateOneWheel(frontRightCollider, frontRightMesh);
        UpdateOneWheel(rearLeftCollider, rearLeftMesh);
        UpdateOneWheel(rearRightCollider, rearRightMesh);
    }

    private void UpdateOneWheel(WheelCollider wheelCollider, Transform wheelMesh)
    {
        if (wheelCollider == null || wheelMesh == null)
            return;

        wheelCollider.GetWorldPose(out Vector3 position, out Quaternion rotation);

        wheelMesh.position = position;
        wheelMesh.rotation = rotation;
    }
}