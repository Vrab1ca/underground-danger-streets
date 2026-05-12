using UnityEngine;

public class CarCameraController : MonoBehaviour
{
    [Header("References")]
    public Transform firstPersonPoint;
    public Transform thirdPersonPoint;
    public Transform cameraTarget;

    [Header("Settings")]
    public KeyCode switchCameraKey = KeyCode.V;
    public float cameraSmoothness = 10f;
    public float mouseSensitivity = 120f;
    public float minPitch = -40f;
    public float maxPitch = 65f;

    private Camera playerCamera;
    private bool thirdPerson = false;

    private float yaw;
    private float pitch;

    void Update()
    {
        if (playerCamera == null)
            return;

        if (Input.GetKeyDown(switchCameraKey))
            thirdPerson = !thirdPerson;

        HandleMouseLook();
        UpdateCameraPosition();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

    void UpdateCameraPosition()
    {
        Transform point = thirdPerson ? thirdPersonPoint : firstPersonPoint;

        if (point == null)
            return;

        playerCamera.transform.position = Vector3.Lerp(
            playerCamera.transform.position,
            point.position,
            cameraSmoothness * Time.deltaTime
        );

        Quaternion targetRotation;

        if (thirdPerson)
        {
            targetRotation = Quaternion.Euler(pitch, transform.eulerAngles.y + yaw, 0f);
        }
        else
        {
            targetRotation = Quaternion.Euler(pitch, transform.eulerAngles.y + yaw, 0f);
        }

        playerCamera.transform.rotation = Quaternion.Lerp(
            playerCamera.transform.rotation,
            targetRotation,
            cameraSmoothness * Time.deltaTime
        );
    }

    public void SetPlayerCamera(Camera cam)
    {
        playerCamera = cam;

        yaw = 0f;
        pitch = 0f;
    }

    public void SetModeFirstPerson()
    {
        thirdPerson = false;
    }
}