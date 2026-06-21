using UnityEngine;

public class JumpPlatformInventory : MonoBehaviour
{
    [Header("Inventory")]
    public int maxPlatforms = 3;
    public int currentPlatforms = 3;

    [Header("Platform")]
    public GameObject jumpPlatformPrefab;
    public KeyCode placeKey = KeyCode.T;

    [Header("Placement")]
    public Camera playerCamera;
    public float placeDistance = 2f;
    public float rayHeight = 2f;
    public float rayDistance = 6f;
    public LayerMask groundMask = ~0;

    private void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        currentPlatforms = Mathf.Clamp(currentPlatforms, 0, maxPlatforms);
    }

    private void Update()
    {
        if (Input.GetKeyDown(placeKey))
        {
            PlacePlatform();
        }
    }

    public void AddPlatforms(int amount)
    {
        currentPlatforms += amount;

        if (currentPlatforms > maxPlatforms)
            currentPlatforms = maxPlatforms;

        Debug.Log("Jump platforms: " + currentPlatforms + " / " + maxPlatforms);
    }

    private void PlacePlatform()
    {
        if (currentPlatforms <= 0)
        {
            Debug.Log("No jump platforms left.");
            return;
        }

        if (jumpPlatformPrefab == null)
        {
            Debug.LogWarning("Jump Platform Prefab is missing.");
            return;
        }

        Vector3 placePosition = GetPlacePosition();
        Quaternion placeRotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);

        Instantiate(jumpPlatformPrefab, placePosition, placeRotation);

        currentPlatforms--;

        Debug.Log("Placed jump platform. Left: " + currentPlatforms);
    }

    private Vector3 GetPlacePosition()
    {
        Vector3 forward = transform.forward;

        if (playerCamera != null)
            forward = playerCamera.transform.forward;

        Vector3 startPosition = transform.position + forward * placeDistance;
        startPosition.y += rayHeight;

        if (Physics.Raycast(startPosition, Vector3.down, out RaycastHit hit, rayDistance, groundMask, QueryTriggerInteraction.Ignore))
        {
            return hit.point + Vector3.up * 0.05f;
        }

        Vector3 fallback = transform.position + transform.forward * placeDistance;
        fallback.y = transform.position.y - 0.8f;

        return fallback;
    }
}