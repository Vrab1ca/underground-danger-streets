using UnityEngine;

public class PlayerStamina : MonoBehaviour
{
    [Header("Stamina")]
    public float maxStamina = 100f;
    public float currentStamina = 100f;

    [Header("Run")]
    public float runStaminaCostPerSecond = 18f;
    public float minStaminaToRun = 5f;

    [Header("Jump")]
    public float jumpStaminaCost = 18f;

    [Header("Recover")]
    public float staminaRecoverPerSecond = 14f;
    public float recoverDelayAfterUse = 0.35f;

    [Header("Debug")]
    public bool showNotEnoughMessage = true;
    public float notEnoughMessageCooldown = 0.8f;

    private float nextRecoverTime;
    private float nextNotEnoughMessageTime;

    // This fixes PlayerStaminaHUD.cs error.
    public float StaminaPercent
    {
        get
        {
            if (maxStamina <= 0f)
                return 0f;

            return currentStamina / maxStamina;
        }
    }

    private void Awake()
    {
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
    }

    private void Update()
    {
        RecoverStamina();
        SendStaminaToHUD();
    }

    public bool CanRun()
    {
        return currentStamina >= minStaminaToRun;
    }

    public bool CanSprint()
    {
        return CanRun();
    }

    public void UseRunStamina()
    {
        if (currentStamina <= 0f)
            return;

        currentStamina -= runStaminaCostPerSecond * Time.deltaTime;
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

        nextRecoverTime = Time.time + recoverDelayAfterUse;

        SendStaminaToHUD();
    }

    public bool TryUseJumpStamina()
    {
        if (currentStamina < jumpStaminaCost)
        {
            ShowNotEnoughStaminaMessage();
            return false;
        }

        currentStamina -= jumpStaminaCost;
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

        nextRecoverTime = Time.time + recoverDelayAfterUse;

        SendStaminaToHUD();

        return true;
    }

    private void RecoverStamina()
    {
        if (Time.time < nextRecoverTime)
            return;

        if (currentStamina >= maxStamina)
            return;

        currentStamina += staminaRecoverPerSecond * Time.deltaTime;
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
    }

    private void ShowNotEnoughStaminaMessage()
    {
        if (!showNotEnoughMessage)
            return;

        if (Time.time < nextNotEnoughMessageTime)
            return;

        nextNotEnoughMessageTime = Time.time + notEnoughMessageCooldown;

        Debug.Log("Not enough stamina.");
    }

    private void SendStaminaToHUD()
    {
        if (ManualFPSHUDUI.Instance != null)
        {
            ManualFPSHUDUI.Instance.SetStamina(currentStamina, maxStamina);
        }
    }
}