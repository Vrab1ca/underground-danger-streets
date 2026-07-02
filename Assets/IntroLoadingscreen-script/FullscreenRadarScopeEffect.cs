using UnityEngine;

public class FullscreenRadarScopeEffect : MonoBehaviour
{
    [Header("References")]
    public RectTransform fullScreenArea;
    public RectTransform scopeMover;
    public CanvasGroup radarGroup;

    [Header("Mouse Movement")]
    public float mouseMovePower = 1f;
    public float smoothSpeed = 14f;

    [Header("Extra Fast Motion")]
    public float autoMoveX = 180f;
    public float autoMoveY = 100f;
    public float autoMoveSpeedX = 2.8f;
    public float autoMoveSpeedY = 2.1f;

    [Header("Small Shake")]
    public float shakeAmount = 3f;
    public float shakeSpeed = 25f;

    [Header("Pulse")]
    public float pulseSpeed = 5f;
    public float pulseAmount = 0.05f;

    [Header("Flicker")]
    public bool useFlicker = true;
    public float flickerSpeed = 14f;
    public float flickerAmount = 0.12f;

    [Header("Screen Margin")]
    public float screenMargin = 80f;

    private Vector2 targetPosition;
    private Vector3 startScale;

    private void Start()
    {
        if (fullScreenArea == null)
            fullScreenArea = GetComponent<RectTransform>();

        if (radarGroup == null)
            radarGroup = GetComponent<CanvasGroup>();

        if (scopeMover != null)
            startScale = scopeMover.localScale;
    }

    private void Update()
    {
        MoveScopeFullScreen();
        PulseScope();
        FlickerScope();
    }

    private void MoveScopeFullScreen()
    {
        if (fullScreenArea == null || scopeMover == null)
            return;

        float areaWidth = fullScreenArea.rect.width;
        float areaHeight = fullScreenArea.rect.height;

        float halfWidth = areaWidth * 0.5f;
        float halfHeight = areaHeight * 0.5f;

        float scopeHalfWidth = scopeMover.rect.width * 0.5f;
        float scopeHalfHeight = scopeMover.rect.height * 0.5f;

        float maxX = halfWidth - scopeHalfWidth - screenMargin;
        float maxY = halfHeight - scopeHalfHeight - screenMargin;

        if (maxX < 0f)
            maxX = 0f;

        if (maxY < 0f)
            maxY = 0f;

        Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        Vector2 mouseOffset = (Vector2)Input.mousePosition - screenCenter;

        float normalizedMouseX = Mathf.Clamp(mouseOffset.x / screenCenter.x, -1f, 1f);
        float normalizedMouseY = Mathf.Clamp(mouseOffset.y / screenCenter.y, -1f, 1f);

        Vector2 mouseMove = new Vector2(
            normalizedMouseX * maxX * mouseMovePower,
            normalizedMouseY * maxY * mouseMovePower
        );

        Vector2 autoMove = new Vector2(
            Mathf.Sin(Time.time * autoMoveSpeedX) * autoMoveX,
            Mathf.Cos(Time.time * autoMoveSpeedY) * autoMoveY
        );

        Vector2 shake = new Vector2(
            Mathf.Sin(Time.time * shakeSpeed) * shakeAmount,
            Mathf.Cos(Time.time * shakeSpeed * 1.3f) * shakeAmount
        );

        targetPosition = mouseMove + autoMove + shake;

        targetPosition.x = Mathf.Clamp(targetPosition.x, -maxX, maxX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, -maxY, maxY);

        scopeMover.anchoredPosition = Vector2.Lerp(
            scopeMover.anchoredPosition,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );
    }

    private void PulseScope()
    {
        if (scopeMover == null)
            return;

        float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        scopeMover.localScale = startScale * pulse;
    }

    private void FlickerScope()
    {
        if (!useFlicker)
            return;

        if (radarGroup == null)
            return;

        float flicker = Mathf.Sin(Time.time * flickerSpeed) * flickerAmount;
        radarGroup.alpha = 0.85f + flicker;
    }
}