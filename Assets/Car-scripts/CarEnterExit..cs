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

    [Header("Disable ONLY movement scripts here")]
    public MonoBehaviour[] playerScriptsToDisable;

    [Header("Optional: hide player body while driving")]
    public GameObject[] objectsToHideWhenDriving;

    [Header("Settings")]
    public float enterDistance = 8f;
    public KeyCode enterExitKey = KeyCode.E;

    public bool PlayerInside { get; private set; }

    private CharacterController characterController;
    private Transform originalPlayerParent;

    private void Start()
    {
        if (player != null)
        {
            characterController = player.GetComponent<CharacterController>();
            originalPlayerParent = player.transform.parent;
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
            if (PlayerInside)
                ExitCar();
            else
                TryEnterCar();
        }
    }

    private void TryEnterCar()
    {
        if (player == null)
            return;

        float distance = Vector3.Distance(player.transform.position, transform.position);

        if (distance <= enterDistance)
            EnterCar();
        else
            Debug.Log("Too far from car. Distance: " + distance);
    }

    private void EnterCar()
    {
        PlayerInside = true;

        if (characterController != null)
            characterController.enabled = false;

        foreach (MonoBehaviour script in playerScriptsToDisable)
        {
            if (script != null)
                script.enabled = false;
        }

        // Put player inside the car and make player follow the car
        player.transform.SetParent(seatPoint);
        player.transform.localPosition = Vector3.zero;
        player.transform.localRotation = Quaternion.identity;

        // Hide only body/visual objects if you want
        foreach (GameObject obj in objectsToHideWhenDriving)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        if (carController != null)
            carController.canDrive = true;

        SetCamera(playerCamera, false);
        SetCamera(carCamera, true);

        Debug.Log("Entered car. Weapon should still work.");
    }

    private void ExitCar()
    {
        PlayerInside = false;

        player.transform.SetParent(originalPlayerParent);

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

        foreach (GameObject obj in objectsToHideWhenDriving)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        if (carController != null)
            carController.canDrive = false;

        SetCamera(carCamera, false);
        SetCamera(playerCamera, true);

        Debug.Log("Exited car.");
    }

    private void SetCamera(Camera cam, bool active)
    {
        if (cam == null)
            return;

        // Do NOT use cam.gameObject.SetActive(false)
        // because WeaponHolder is child of Main Camera.
        cam.enabled = active;

        AudioListener listener = cam.GetComponent<AudioListener>();

        if (listener != null)
            listener.enabled = active;
    }
}