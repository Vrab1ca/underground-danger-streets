using UnityEngine;

public class SimpleADS : MonoBehaviour
{
    public Vector3 hipPosition;
    public Vector3 aimPosition = new Vector3(0f, -0.08f, 0.2f);
    public float aimSpeed = 12f;
    public Camera fpsCamera;
    public float hipFov = 60f;
    public float aimFov = 45f;

    void Start()
    {
        hipPosition = transform.localPosition;
        if (fpsCamera == null) fpsCamera = Camera.main;
    }

    void Update()
    {
        bool aiming = Input.GetButton("Fire2");
        transform.localPosition = Vector3.Lerp(transform.localPosition, aiming ? aimPosition : hipPosition, aimSpeed * Time.deltaTime);

        if (fpsCamera != null)
            fpsCamera.fieldOfView = Mathf.Lerp(fpsCamera.fieldOfView, aiming ? aimFov : hipFov, aimSpeed * Time.deltaTime);
    }
}
