using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FPSIntroManager : MonoBehaviour
{
    [Header("Scene")]
    public string mainMenuSceneName = "MainMenu";

    [Header("UI")]
    public TMP_Text titleText;
    public TMP_Text subtitleText;
    public TMP_Text statusText;
    public TMP_Text tipText;
    public TMP_Text warningText;
    public TMP_Text pressAnyKeyText;

    public RectTransform loadingBarFill;
    public RectTransform scanLine;
    public CanvasGroup finishFillGroup;
    public float finishFillSpeed = 2.5f;

    [Header("Timing")]
    public float introTime = 7f;
    public float typingSpeed = 0.035f;

    [Header("Effects")]
    public float titleShakeAmount = 8f;
    public float scanSpeed = 350f;

    private Vector3 titleOriginalPosition;
    private bool canSkip;

    private string[] statusMessages =
    {
        "BOOTING SURVIVAL SYSTEM...",
        "CHECKING AMMO SUPPLY...",
        "SCANNING ZOMBIE ACTIVITY...",
        "HELICOPTER FUEL: PROBABLY NOT ENOUGH...",
        "LOADING BAD DECISIONS...",
        "PREPARING PLAYER..."
    };

    private string[] tips =
    {
        "Tip: If the zombie is running, you should run faster.",
        "Tip: Fuel cans are useful. Soup is not fuel.",
        "Tip: AWP is strong, but missing is embarrassing.",
        "Tip: Helicopter bombs solve many problems.",
        "Tip: Reload before the zombie hugs you."
    };

    private void Start()
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (titleText != null)
        {
            titleOriginalPosition = titleText.rectTransform.anchoredPosition;
            titleText.text = "";
        }

        if (subtitleText != null)
            subtitleText.text = "";

        if (statusText != null)
            statusText.text = "";

        if (warningText != null)
            warningText.text = "";

        if (pressAnyKeyText != null)
            pressAnyKeyText.text = "";

        if (loadingBarFill != null)
            loadingBarFill.sizeDelta = new Vector2(0f, loadingBarFill.sizeDelta.y);

        if (tipText != null)
            tipText.text = tips[Random.Range(0, tips.Length)];

        StartCoroutine(IntroRoutine());
    }

    private void Update()
    {
        MoveScanLine();
        BlinkPressAnyKey();

        if (canSkip && Input.anyKeyDown)
        {
            LoadMainMenu();
        }
    }

    private IEnumerator IntroRoutine()
    {
        yield return new WaitForSeconds(0.3f);

        if (warningText != null)
            warningText.text = "WARNING: ZOMBIE AREA DETECTED";

        yield return new WaitForSeconds(0.7f);

        if (titleText != null)
            yield return StartCoroutine(TypeText(titleText, "BASSIK"));

        StartCoroutine(GlitchTitle());

        if (subtitleText != null)
            yield return StartCoroutine(TypeText(subtitleText, "SURVIVE THE NIGHT"));

        float timer = 0f;

        while (timer < introTime)
        {
            timer += Time.deltaTime;

            float progress = timer / introTime;

            if (loadingBarFill != null)
            {
                float width = Mathf.Lerp(0f, 500f, progress);
                loadingBarFill.sizeDelta = new Vector2(width, loadingBarFill.sizeDelta.y);
            }

            int index = Mathf.Clamp(Mathf.FloorToInt(progress * statusMessages.Length), 0, statusMessages.Length - 1);

            if (statusText != null)
                statusText.text = statusMessages[index];

            yield return null;
        }

        if (statusText != null)
            statusText.text = "READY.";

        yield return StartCoroutine(FinishFillScreen());

        LoadMainMenu();
    }

    private IEnumerator FinishFillScreen()
    {
        if (finishFillGroup == null)
            yield break;

        finishFillGroup.alpha = 0f;

        while (finishFillGroup.alpha < 1f)
        {
            finishFillGroup.alpha += Time.deltaTime * finishFillSpeed;
            yield return null;
        }

        finishFillGroup.alpha = 1f;

        yield return new WaitForSeconds(0.3f);
    }

    private IEnumerator TypeText(TMP_Text textObject, string text)
    {
        textObject.text = "";

        for (int i = 0; i < text.Length; i++)
        {
            textObject.text += text[i];

            if (Random.Range(0, 100) < 12)
                StartCoroutine(ShakeTitle());

            yield return new WaitForSeconds(typingSpeed);
        }
    }

    private IEnumerator GlitchTitle()
    {
        while (!canSkip)
        {
            if (titleText != null)
            {
                int random = Random.Range(0, 5);

                if (random == 0)
                    titleText.text = "B4SSIK";
                else if (random == 1)
                    titleText.text = "BA55IK";
                else
                    titleText.text = "BASSIK";
            }

            yield return new WaitForSeconds(Random.Range(0.08f, 0.25f));
        }

        if (titleText != null)
            titleText.text = "BASSIK";
    }

    private IEnumerator ShakeTitle()
    {
        if (titleText == null)
            yield break;

        RectTransform rect = titleText.rectTransform;

        float timer = 0f;
        float duration = 0.12f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            rect.anchoredPosition = titleOriginalPosition + new Vector3(
                Random.Range(-titleShakeAmount, titleShakeAmount),
                Random.Range(-titleShakeAmount, titleShakeAmount),
                0f
            );

            yield return null;
        }

        rect.anchoredPosition = titleOriginalPosition;
    }

    private void MoveScanLine()
    {
        if (scanLine == null)
            return;

        Vector2 pos = scanLine.anchoredPosition;
        pos.y -= scanSpeed * Time.deltaTime;

        if (pos.y < -Screen.height)
            pos.y = 50f;

        scanLine.anchoredPosition = pos;
    }

    private void BlinkPressAnyKey()
    {
        if (!canSkip || pressAnyKeyText == null)
            return;

        float alpha = Mathf.Abs(Mathf.Sin(Time.time * 3f));

        Color color = pressAnyKeyText.color;
        color.a = alpha;
        pressAnyKeyText.color = color;
    }

    private void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}