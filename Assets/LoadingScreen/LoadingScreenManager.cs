using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreenManager : MonoBehaviour
{
    [Header("UI")]
    public Slider loadingSlider;
    public TMP_Text loadingText;
    public TMP_Text percentText;
    public TMP_Text tipText;
    public CanvasGroup fadeCanvasGroup;

    [Header("Settings")]
    public float minimumLoadingTime = 2f;
    public float fadeSpeed = 2f;

    [Header("Tips")]
    public string[] tips =
    {
        "Tip: Use cover when zombies get close.",
        "Tip: Save your ammo for dangerous enemies.",
        "Tip: Refuel vehicles before long trips.",
        "Tip: Helicopter bombs use extra fuel.",
        "Tip: Sprinting and jumping use stamina."
    };

    private void Start()
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (loadingSlider != null)
        {
            loadingSlider.minValue = 0f;
            loadingSlider.maxValue = 1f;
            loadingSlider.value = 0f;
        }

        if (tipText != null && tips.Length > 0)
        {
            int randomIndex = Random.Range(0, tips.Length);
            tipText.text = tips[randomIndex];
        }

        if (fadeCanvasGroup != null)
            fadeCanvasGroup.alpha = 1f;

        string targetScene = LoadingScreenLoader.GetTargetScene();

        StartCoroutine(LoadSceneRoutine(targetScene));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        float timer = 0f;

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            timer += Time.deltaTime;

            float realProgress = Mathf.Clamp01(operation.progress / 0.9f);
            float timeProgress = Mathf.Clamp01(timer / minimumLoadingTime);

            float displayedProgress = Mathf.Min(realProgress, timeProgress);

            UpdateUI(displayedProgress);

            if (fadeCanvasGroup != null && fadeCanvasGroup.alpha > 0f)
            {
                fadeCanvasGroup.alpha = Mathf.MoveTowards(
                    fadeCanvasGroup.alpha,
                    0f,
                    fadeSpeed * Time.deltaTime
                );
            }

            if (operation.progress >= 0.9f && timer >= minimumLoadingTime)
            {
                UpdateUI(1f);

                yield return new WaitForSeconds(0.3f);

                yield return StartCoroutine(FadeOut());

                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    private void UpdateUI(float progress)
    {
        if (loadingSlider != null)
            loadingSlider.value = progress;

        if (percentText != null)
            percentText.text = Mathf.RoundToInt(progress * 100f) + "%";

        if (loadingText != null)
        {
            if (progress < 1f)
                loadingText.text = "Loading...";
            else
                loadingText.text = "Starting...";
        }
    }

    private IEnumerator FadeOut()
    {
        if (fadeCanvasGroup == null)
            yield break;

        while (fadeCanvasGroup.alpha < 1f)
        {
            fadeCanvasGroup.alpha = Mathf.MoveTowards(
                fadeCanvasGroup.alpha,
                1f,
                fadeSpeed * Time.deltaTime
            );

            yield return null;
        }
    }
}