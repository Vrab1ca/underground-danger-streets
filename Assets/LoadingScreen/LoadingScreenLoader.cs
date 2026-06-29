using UnityEngine;
using UnityEngine.SceneManagement;

public static class LoadingScreenLoader
{
    private const string TargetSceneKey = "Loading_TargetScene";

    public static void LoadScene(string sceneName)
    {
        PlayerPrefs.SetString(TargetSceneKey, sceneName);
        PlayerPrefs.Save();

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene("LoadingScreen");
    }

    public static string GetTargetScene()
    {
        return PlayerPrefs.GetString(TargetSceneKey, "Level1");
    }
}