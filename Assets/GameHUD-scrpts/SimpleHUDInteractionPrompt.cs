using UnityEngine;

public class SimpleHUDInteractionPrompt : MonoBehaviour
{
    public string message = "Press E to interact";
    public float showDistance = 3f;

    private Transform player;

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
            player = playerObject.transform;
    }

    private void Update()
    {
        if (player == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= showDistance)
        {
            if (ManualFPSHUDUI.Instance != null)
                ManualFPSHUDUI.Instance.ShowInteraction(message);
        }
        else
        {
            if (ManualFPSHUDUI.Instance != null)
                ManualFPSHUDUI.Instance.HideInteraction();
        }
    }

    private void OnDisable()
    {
        if (ManualFPSHUDUI.Instance != null)
            ManualFPSHUDUI.Instance.HideInteraction();
    }
}