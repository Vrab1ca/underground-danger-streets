using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    [Header("Scene")]
    public string mainMenuSceneName = "MainMenu";

    [Header("UI")]
    public TMP_Text logoText;
    public TMP_Text messageText;
    public TMP_Text skipText;
    public CanvasGroup redFlashGroup;

    [Header("Timing")]
    public float startDelay = 0.5f;
    public float typeSpeed = 0.035f;
    public float messageWaitTime = 0.8f;

    [Header("Effects")]
    public float logoPulseSpeed = 3f;
    public float logoPulseAmount = 0.08f;
    public float shakeAmount = 8f;

    private bool skipping;
    private Vector3 logoOriginalScale;
    private Vector3 logoOriginalPosition;

    private string[] introMessages =
    {
        "WARNING: Zombies may run faster than your internet.",
        "Loading useless survival tips...",
        "Tip: If a zombie asks for Wi-Fi, run.",
        "Calibrating very serious helicopter physics...",
        "Filling fuel cans with suspicious liquid...",
        "Teaching zombies how to miss leg day...",
        "Preparing BASSIK..."
    };

    private void Start()
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (logoText != null)
        {
            logoOriginalScale = logoText.transform.localScale;
            logoOriginalPosition = logoText.rectTransform.anchoredPosition;
            logoText.text = "";
        }

        if (messageText != null)
            messageText.text = "";

        if (skipText != null)
            skipText.text = "Press SPACE to skip";

        if (redFlashGroup != null)
            redFlashGroup.alpha = 0f;

        StartCoroutine(IntroRoutine());
    }

    private void Update()
    {
        AnimateLogo();

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Escape))
        {
            SkipIntro();
        }
    }

    private void AnimateLogo()
    {
        if (logoText == null)
            return;

        float pulse = 1f + Mathf.Sin(Time.time * logoPulseSpeed) * logoPulseAmount;
        logoText.transform.localScale = logoOriginalScale * pulse;
    }

    private IEnumerator IntroRoutine()
    {
        yield return new WaitForSeconds(startDelay);

        if (logoText != null)
            yield return StartCoroutine(TypeText(logoText, "BASSIK"));

        yield return FlashRed();

        for (int i = 0; i < introMessages.Length; i++)
        {
            if (skipping)
                yield break;

            yield return StartCoroutine(TypeText(messageText, introMessages[i]));
            yield return new WaitForSeconds(messageWaitTime);
        }

        yield return StartCoroutine(TypeText(messageText, "Press any key to survive... just kidding. Starting game."));

        yield return new WaitForSeconds(1f);

        LoadMainMenu();
    }

    private IEnumerator TypeText(TMP_Text targetText, string text)
    {
        if (targetText == null)
            yield break;

        targetText.text = "";

        for (int i = 0; i < text.Length; i++)
        {
            if (skipping)
                yield break;

            targetText.text += text[i];

            if (Random.Range(0, 100) < 8)
                StartCoroutine(ShakeLogo());

            yield return new WaitForSeconds(typeSpeed);
        }
    }

    private IEnumerator ShakeLogo()
    {
        if (logoText == null)
            yield break;

        RectTransform rect = logoText.rectTransform;

        float timer = 0f;
        float duration = 0.15f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            rect.anchoredPosition = logoOriginalPosition + new Vector3(
                Random.Range(-shakeAmount, shakeAmount),
                Random.Range(-shakeAmount, shakeAmount),
                0f
            );

            yield return null;
        }

        rect.anchoredPosition = logoOriginalPosition;
    }

    private IEnumerator FlashRed()
    {
        if (redFlashGroup == null)
            yield break;

        redFlashGroup.alpha = 0.6f;

        while (redFlashGroup.alpha > 0f)
        {
            redFlashGroup.alpha -= Time.deltaTime * 2f;
            yield return null;
        }

        redFlashGroup.alpha = 0f;
    }

    private void SkipIntro()
    {
        if (skipping)
            return;

        skipping = true;
        LoadMainMenu();
    }

    private void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}