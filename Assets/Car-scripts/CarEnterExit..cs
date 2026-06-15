using UnityEngine;

public class CarEnterExit : MonoBehaviour
{
    [Header("References")]
    public CarController carController;
    public GameObject player;
    public Transform seatPoint;
    public Transform exitPoint;
    public Camera playerCamera;
    public Camera carCamera;

    [Header("Disable These Player Scripts When Driving")]
    public MonoBehaviour[] playerScriptsToDisable;

    [Header("Settings")]
    public float enterDistance = 8f;
    public KeyCode enterExitKey = KeyCode.E;

    public bool PlayerInside { get; private set; }

    private CharacterController characterController;

    private void Start()
    {
        if (player != null)
        {
            characterController = player.GetComponent<CharacterController>();
        }

        PlayerInside = false;

        if (carController != null)
            carController.canDrive = false;

        SetCamera(playerCamera, true);
        SetCamera(carCamera, false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(enterExitKey))
        {
            Debug.Log("E pressed");

            if (PlayerInside)
            {
                ExitCar();
            }
            else
            {
                TryEnterCar();
            }
        }
    }

    private void TryEnterCar()
    {
        if (player == null)
        {
            Debug.LogError("Player is missing!");
            return;
        }

        float distance = Vector3.Distance(player.transform.position, transform.position);

        Debug.Log("Distance to car: " + distance);

        if (distance <= enterDistance)
        {
            EnterCar();
        }
        else
        {
            Debug.Log("Too far from car. Go closer or increase Enter Distance.");
        }
    }

    private void EnterCar()
    {
        Debug.Log("Entered car");

        PlayerInside = true;

        if (characterController != null)
            characterController.enabled = false;

        foreach (MonoBehaviour script in playerScriptsToDisable)
        {
            if (script != null)
                script.enabled = false;
        }

        player.transform.position = seatPoint.position;
        player.transform.rotation = seatPoint.rotation;

        player.SetActive(false);

        if (carController != null)
            carController.canDrive = true;

        SetCamera(playerCamera, false);
        SetCamera(carCamera, true);
    }

    private void ExitCar()
    {
        Debug.Log("Exited car");

        PlayerInside = false;

        player.SetActive(true);

        if (characterController != null)
            characterController.enabled = false;

        player.transform.position = exitPoint.position;
        player.transform.rotation = exitPoint.rotation;

        if (characterController != null)
            characterController.enabled = true;

        foreach (MonoBehaviour script in playerScriptsToDisable)
        {
            if (script != null)
                script.enabled = true;
        }

        if (carController != null)
            carController.canDrive = false;

        SetCamera(carCamera, false);
        SetCamera(playerCamera, true);
    }

    private void SetCamera(Camera cam, bool active)
    {
        if (cam == null)
            return;

        cam.gameObject.SetActive(active);
        cam.enabled = active;

        AudioListener listener = cam.GetComponent<AudioListener>();

        if (listener != null)
            listener.enabled = active;
    }
}