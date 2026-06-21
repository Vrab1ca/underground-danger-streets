using UnityEngine;

public class SimpleADS : MonoBehaviour
{
    public Vector3 hipPosition;
    public Vector3 aimPosition = new Vector3(0f, -0.08f, 0.2f);
    public float aimSpeed = 12f;

    public Camera fpsCamera;
    public Camera carCamera;

    public float hipFov = 60f;
    public float aimFov = 45f;

    private void Start()
    {
        hipPosition = transform.localPosition;

        if (fpsCamera == null)
            fpsCamera = Camera.main;
    }

    private void Update()
    {
        bool aiming = Input.GetButton("Fire2"); // Right Mouse

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            aiming ? aimPosition : hipPosition,
            aimSpeed * Time.deltaTime
        );

        Camera cam = GetCurrentCamera();

        if (cam != null)
        {
            cam.fieldOfView = Mathf.Lerp(
                cam.fieldOfView,
                aiming ? aimFov : hipFov,
                aimSpeed * Time.deltaTime
            );
        }
    }

    private Camera GetCurrentCamera()
    {
        if (carCamera != null && carCamera.enabled)
            return carCamera;

        if (fpsCamera != null)
            return fpsCamera;

        if (Camera.main != null)
            return Camera.main;

        return null;
    }
}