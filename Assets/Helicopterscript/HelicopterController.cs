using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HelicopterController : MonoBehaviour
{
    [Header("State")]
    public bool canFly;
    public bool engineOn;
    public bool fallingAfterExit;

    [Header("Rotor Visuals")]
    public Transform mainRotor;
    public Transform tailRotor;
    public float rotorSpinSpeed = 1200f;

    [Header("Movement")]
    public float verticalSpeed = 12f;
    public float forwardSpeed = 18f;
    public float turnSpeed = 90f;
    public float boostMultiplier = 1.8f;
    public float maxSpeed = 35f;

    [Header("Tilt")]
    public float pitchAngle = 18f;
    public float rollAngle = 20f;
    public float stabiliseSpeed = 4f;

    [Header("Damping")]
    public float flyingLinearDamping = 0.5f;
    public float flyingAngularDamping = 2f;
    public float fallLinearDamping = 0.2f;
    public float fallAngularDamping = 0.5f;

    [Header("Info")]
    public float speedKmh;
    public float altitude;
    public bool boosting;

    private Rigidbody rb;
    private float yaw;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        yaw = transform.eulerAngles.y;

        FreezeAtStart();
    }

    private void Update()
    {
        speedKmh = rb.linearVelocity.magnitude * 3.6f;
        altitude = transform.position.y;

        SpinRotors();
    }

    private void FixedUpdate()
    {
        if (!canFly)
            return;

        HandleMovement();
        ClampSpeed();
    }

    public void FreezeAtStart()
    {
        canFly = false;
        engineOn = false;
        fallingAfterExit = false;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.useGravity = false;
        rb.isKinematic = true;

        boosting = false;

        Debug.Log("Helicopter frozen at start.");
    }

    public void StartHelicopter()
    {
        canFly = true;
        engineOn = true;
        fallingAfterExit = false;

        rb.isKinematic = false;
        rb.useGravity = false;

        rb.linearDamping = flyingLinearDamping;
        rb.angularDamping = flyingAngularDamping;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        yaw = transform.eulerAngles.y;

        Debug.Log("Helicopter started.");
    }

    public void EngineOffAndFall()
    {
        canFly = false;
        engineOn = false;
        fallingAfterExit = true;

        rb.isKinematic = false;
        rb.useGravity = true;

        rb.linearDamping = fallLinearDamping;
        rb.angularDamping = fallAngularDamping;

        boosting = false;

        Debug.Log("Helicopter engine off. Helicopter is falling.");
    }

    private void HandleMovement()
    {
        float forwardInput = Input.GetAxis("Vertical");   // W / S
        float turnInput = Input.GetAxis("Horizontal");    // A / D

        float upInput = 0f;

        if (Input.GetKey(KeyCode.Space))
            upInput = 1f;

        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C))
            upInput = -1f;

        boosting = Input.GetKey(KeyCode.LeftShift);

        float speedMultiplier = boosting ? boostMultiplier : 1f;

        rb.AddForce(Vector3.up * upInput * verticalSpeed, ForceMode.Acceleration);
        rb.AddForce(transform.forward * forwardInput * forwardSpeed * speedMultiplier, ForceMode.Acceleration);

        yaw += turnInput * turnSpeed * Time.fixedDeltaTime;

        Quaternion targetRotation = Quaternion.Euler(
            forwardInput * pitchAngle,
            yaw,
            -turnInput * rollAngle
        );

        rb.MoveRotation(
            Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime * stabiliseSpeed)
        );

        if (Mathf.Abs(upInput) < 0.1f)
        {
            Vector3 velocity = rb.linearVelocity;
            velocity.y = Mathf.Lerp(velocity.y, 0f, Time.fixedDeltaTime * 1.5f);
            rb.linearVelocity = velocity;
        }
    }

    private void ClampSpeed()
    {
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }

    private void SpinRotors()
    {
        float spin = 0f;

        if (engineOn)
        {
            spin = rotorSpinSpeed;
        }
        else if (fallingAfterExit)
        {
            spin = rotorSpinSpeed * 0.15f;
        }

        if (mainRotor != null)
            mainRotor.Rotate(Vector3.up * spin * Time.deltaTime, Space.Self);

        if (tailRotor != null)
            tailRotor.Rotate(Vector3.forward * spin * Time.deltaTime, Space.Self);
    }
}