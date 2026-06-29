using TMPro;
using UnityEngine;

public class PlayerLookInfo : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;

    [Header("Vehicle Check")]
    public CarEnterExit carEnterExit;
    public HelicopterEnterExit helicopterEnterExit;

    [Header("UI")]
    public CanvasGroup lookInfoCanvasGroup;
    public TMP_Text titleText;
    public TMP_Text descriptionText;

    [Header("Raycast")]
    public float lookDistance = 6f;
    public LayerMask lookMask = ~0;

    [Header("Fade")]
    public float fadeInSpeed = 10f;
    public float fadeOutSpeed = 4f;

    private LookAtInfo currentInfo;

    private void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        if (lookInfoCanvasGroup != null)
        {
            lookInfoCanvasGroup.alpha = 0f;
            lookInfoCanvasGroup.interactable = false;
            lookInfoCanvasGroup.blocksRaycasts = false;
        }
    }

    private void Update()
    {
        if (IsPlayerInsideVehicle())
        {
            currentInfo = null;
            FadeOutOnly();
            return;
        }

        CheckLookObject();
        UpdateUI();
    }

    private bool IsPlayerInsideVehicle()
    {
        if (carEnterExit != null && carEnterExit.PlayerInside)
            return true;

        if (helicopterEnterExit != null && helicopterEnterExit.PlayerInside)
            return true;

        return false;
    }

    private void CheckLookObject()
    {
        currentInfo = null;

        if (playerCamera == null)
            return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, lookDistance, lookMask, QueryTriggerInteraction.Collide))
        {
            LookAtInfo info = hit.collider.GetComponentInParent<LookAtInfo>();

            if (info == null)
                return;

            float distance = Vector3.Distance(playerCamera.transform.position, info.transform.position);

            if (distance <= info.showDistance)
                currentInfo = info;
        }
    }

    private void UpdateUI()
    {
        if (lookInfoCanvasGroup == null)
            return;

        bool shouldShow = currentInfo != null;

        if (shouldShow)
        {
            if (titleText != null)
                titleText.text = currentInfo.objectName;

            if (descriptionText != null)
                descriptionText.text = currentInfo.description;

            lookInfoCanvasGroup.alpha = Mathf.MoveTowards(
                lookInfoCanvasGroup.alpha,
                1f,
                fadeInSpeed * Time.deltaTime
            );
        }
        else
        {
            FadeOutOnly();
        }
    }

    private void FadeOutOnly()
    {
        if (lookInfoCanvasGroup == null)
            return;

        lookInfoCanvasGroup.alpha = Mathf.MoveTowards(
            lookInfoCanvasGroup.alpha,
            0f,
            fadeOutSpeed * Time.deltaTime
        );
    }
}