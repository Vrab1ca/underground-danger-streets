using UnityEngine;

public class CarEnterExit : MonoBehaviour
{
    [Header("References")]
    public GameObject player;
    public Camera playerCamera;

    public Transform driverSeat;
    public Transform exitPoint;

    public BetterCarController carController;
    public CarHealth carHealth;
    public CarCameraController carCameraController;

    [Header("Settings")]
    public float enterDistance = 3f;
    public KeyCode enterKey = KeyCode.E;

    private bool isInside = false;

    private CharacterController playerController;
    private PlayerMotor playerMotor;
    private MouseLook mouseLook;

    void Awake()
    {
        if (carController == null)
            carController = GetComponent<BetterCarController>();

        if (carHealth == null)
            carHealth = GetComponent<CarHealth>();

        if (carCameraController == null)
            carCameraController = GetComponent<CarCameraController>();

        if (player != null)
        {
            playerController = player.GetComponent<CharacterController>();
            playerMotor = player.GetComponent<PlayerMotor>();

            if (playerCamera == null)
                playerCamera = player.GetComponentInChildren<Camera>();

            if (playerCamera != null)
                mouseLook = playerCamera.GetComponent<MouseLook>();
        }

        if (carCameraController != null)
            carCameraController.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(enterKey))
        {
            if (isInside)
                ExitCar();
            else
                TryEnterCar();
        }

        if (isInside && carHealth != null && carHealth.IsBroken)
            ExitCar();
    }

    void TryEnterCar()
    {
        if (player == null)
            return;

        if (carController == null)
            return;

        if (carHealth != null && carHealth.IsBroken)
            return;

        float distance = Vector3.Distance(player.transform.position, transform.position);

        if (distance <= enterDistance)
            EnterCar();
    }

    void EnterCar()
    {
        isInside = true;

        if (playerController != null)
            playerController.enabled = false;

        if (playerMotor != null)
            playerMotor.enabled = false;

        if (mouseLook != null)
            mouseLook.enabled = false;

        player.transform.SetParent(driverSeat);
        player.transform.localPosition = Vector3.zero;
        player.transform.localRotation = Quaternion.identity;

        if (playerCamera != null)
            playerCamera.gameObject.SetActive(true);

        if (carCameraController != null)
        {
            carCameraController.enabled = true;
            carCameraController.SetPlayerCamera(playerCamera);
            carCameraController.SetModeFirstPerson();
        }

        carController.SetDriving(true);
    }

    void ExitCar()
    {
        isInside = false;

        player.transform.SetParent(null);

        if (exitPoint != null)
            player.transform.position = exitPoint.position;
        else
            player.transform.position = transform.position - transform.right * 2f;

        player.transform.rotation = Quaternion.identity;

        if (playerController != null)
            playerController.enabled = true;

        if (playerMotor != null)
            playerMotor.enabled = true;

        if (mouseLook != null)
            mouseLook.enabled = true;

        if (carCameraController != null)
            carCameraController.enabled = false;

        if (playerCamera != null)
        {
            playerCamera.transform.localPosition = new Vector3(0f, 1.6f, 0f);
            playerCamera.transform.localRotation = Quaternion.identity;
            playerCamera.gameObject.SetActive(true);
        }

        if (carController != null)
            carController.SetDriving(false);
    }
}