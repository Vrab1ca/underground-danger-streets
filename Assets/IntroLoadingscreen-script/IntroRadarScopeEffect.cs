using UnityEngine;

public class IntroRadarScopeEffect : MonoBehaviour
{
    [Header("References")]
    public RectTransform radarRoot;
    public CanvasGroup radarGroup;

    [Header("Mouse Smooth Move")]
    public float maxMoveX = 35f;
    public float maxMoveY = 22f;
    public float smoothSpeed = 8f;

    [Header("Idle Motion")]
    public float idleMoveAmount = 6f;
    public float idleMoveSpeed = 1.5f;

    [Header("Pulse")]
    public float pulseSpeed = 3f;
    public float pulseAmount = 0.04f;

    [Header("Fade Flicker")]
    public bool useFlicker = true;
    public float flickerSpeed = 12f;
    public float flickerAmount = 0.15f;

    private Vector2 startPosition;
    private Vector3 startScale;

    private void Start()
    {
        if (radarRoot == null)
            radarRoot = GetComponent<RectTransform>();

        if (radarGroup == null)
            radarGroup = GetComponent<CanvasGroup>();

        if (radarRoot != null)
        {
            startPosition = radarRoot.anchoredPosition;
            startScale = radarRoot.localScale;
        }
    }

    private void Update()
    {
        MoveWithMouseSmooth();
        PulseScope();
        FlickerScope();
    }

    private void MoveWithMouseSmooth()
    {
        if (radarRoot == null)
            return;

        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 mouseOffset = (Vector2)Input.mousePosition - screenCenter;

        float normalizedX = Mathf.Clamp(mouseOffset.x / screenCenter.x, -1f, 1f);
        float normalizedY = Mathf.Clamp(mouseOffset.y / screenCenter.y, -1f, 1f);

        Vector2 targetPosition = startPosition + new Vector2(
            normalizedX * maxMoveX,
            normalizedY * maxMoveY
        );

        Vector2 idleOffset = new Vector2(
            Mathf.Sin(Time.time * idleMoveSpeed) * idleMoveAmount,
            Mathf.Cos(Time.time * idleMoveSpeed * 0.8f) * idleMoveAmount
        );

        targetPosition += idleOffset;

        radarRoot.anchoredPosition = Vector2.Lerp(
            radarRoot.anchoredPosition,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );
    }

    private void PulseScope()
    {
        if (radarRoot == null)
            return;

        float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        radarRoot.localScale = startScale * pulse;
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