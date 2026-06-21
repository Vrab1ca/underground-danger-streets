using UnityEngine;

public class HelicopterEnterExit : MonoBehaviour
{
    [Header("References")]
    public HelicopterController helicopterController;
    public GameObject player;
    public Transform seatPoint;
    public Transform exitPoint;
    public Camera playerCamera;
    public Camera helicopterCamera;

    [Header("Disable Player Scripts When Flying")]
    public MonoBehaviour[] playerScriptsToDisable;

    [Header("Settings")]
    public float enterDistance = 5f;
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

        if (helicopterController != null)
            helicopterController.FreezeAtStart();

        SetCamera(playerCamera, true);
        SetCamera(helicopterCamera, false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(enterExitKey))
        {
            if (PlayerInside)
                ExitHelicopter();
            else
                TryEnterHelicopter();
        }
    }

    private void TryEnterHelicopter()
    {
        if (player == null)
            return;

        float distance = Vector3.Distance(player.transform.position, transform.position);

        if (distance <= enterDistance)
        {
            EnterHelicopter();
        }
        else
        {
            Debug.Log("Too far from helicopter. Distance: " + distance);
        }
    }

    private void EnterHelicopter()
    {
        PlayerInside = true;

        if (characterController != null)
            characterController.enabled = false;

        foreach (MonoBehaviour script in playerScriptsToDisable)
        {
            if (script != null)
                script.enabled = false;
        }

        player.transform.SetParent(seatPoint);
        player.transform.localPosition = Vector3.zero;
        player.transform.localRotation = Quaternion.identity;

        player.SetActive(false);

        if (helicopterController != null)
            helicopterController.StartHelicopter();

        SetCamera(playerCamera, false);
        SetCamera(helicopterCamera, true);

        Debug.Log("Entered helicopter.");
    }

    private void ExitHelicopter()
    {
        PlayerInside = false;

        player.SetActive(true);
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

        if (helicopterController != null)
            helicopterController.EngineOffAndFall();

        SetCamera(helicopterCamera, false);
        SetCamera(playerCamera, true);

        Debug.Log("Exited helicopter. Helicopter will fall.");
    }

    private void SetCamera(Camera cam, bool active)
    {
        if (cam == null)
            return;

        cam.enabled = active;

        AudioListener listener = cam.GetComponent<AudioListener>();

        if (listener != null)
            listener.enabled = active;
    }
}