using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStaminaHUD : MonoBehaviour
{
    [Header("References")]
    public PlayerStamina playerStamina;
    public Slider staminaSlider;
    public TMP_Text staminaText;

    [Header("Fade UI")]
    public CanvasGroup staminaCanvasGroup;
    public float fadeInSpeed = 8f;
    public float fadeOutSpeed = 2f;
    public float hideDelayAfterFull = 1.2f;

    [Header("Optional Fill Color")]
    public Image fillImage;
    public Color highColor = Color.green;
    public Color mediumColor = Color.yellow;
    public Color lowColor = Color.red;

    private float hideTimer;
    private float lastStamina;

    private void Start()
    {
        if (playerStamina != null)
            lastStamina = playerStamina.currentStamina;

        if (staminaCanvasGroup != null)
        {
            staminaCanvasGroup.alpha = 0f;
            staminaCanvasGroup.interactable = false;
            staminaCanvasGroup.blocksRaycasts = false;
        }

        if (staminaSlider != null)
        {
            staminaSlider.minValue = 0f;
            staminaSlider.maxValue = 1f;
            staminaSlider.interactable = false;
        }
    }

    private void Update()
    {
        if (playerStamina == null || staminaSlider == null)
            return;

        UpdateStaminaBar();
        UpdateFade();
    }

    private void UpdateStaminaBar()
    {
        float staminaPercent = playerStamina.StaminaPercent;

        staminaSlider.value = staminaPercent;

        if (staminaText != null)
        {
            int staminaNumber = Mathf.RoundToInt(playerStamina.currentStamina);
            staminaText.text = "Stamina: " + staminaNumber;
        }

        if (fillImage != null)
        {
            if (staminaPercent > 0.5f)
                fillImage.color = highColor;
            else if (staminaPercent > 0.25f)
                fillImage.color = mediumColor;
            else
                fillImage.color = lowColor;
        }
    }

    private void UpdateFade()
    {
        if (staminaCanvasGroup == null)
            return;

        bool staminaChanged = Mathf.Abs(playerStamina.currentStamina - lastStamina) > 0.1f;
        bool staminaNotFull = playerStamina.currentStamina < playerStamina.maxStamina - 0.5f;

        bool shouldShow = staminaChanged || staminaNotFull;

        if (shouldShow)
        {
            hideTimer = hideDelayAfterFull;

            staminaCanvasGroup.alpha = Mathf.MoveTowards(
                staminaCanvasGroup.alpha,
                1f,
                fadeInSpeed * Time.deltaTime
            );
        }
        else
        {
            hideTimer -= Time.deltaTime;

            if (hideTimer <= 0f)
            {
                staminaCanvasGroup.alpha = Mathf.MoveTowards(
                    staminaCanvasGroup.alpha,
                    0f,
                    fadeOutSpeed * Time.deltaTime
                );
            }
        }

        lastStamina = playerStamina.currentStamina;
    }
}