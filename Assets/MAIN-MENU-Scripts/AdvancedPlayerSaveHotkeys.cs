using UnityEngine;
using UnityEngine.SceneManagement;

public class AdvancedPlayerSaveHotkeys : MonoBehaviour
{
    [Header("Keys")]
    public KeyCode saveKey = KeyCode.F5;
    public KeyCode loadLatestKey = KeyCode.F9;
    public KeyCode mainMenuKey = KeyCode.Escape;

    [Header("Scenes")]
    public string mainMenuSceneName = "MainMenu";

    [Header("Options")]
    public bool escapeGoesToMainMenu = true;

    private void Update()
    {
        if (Input.GetKeyDown(saveKey))
        {
            SaveGameButton();
        }

        if (Input.GetKeyDown(loadLatestKey))
        {
            LoadLatestButton();
        }

        if (escapeGoesToMainMenu && Input.GetKeyDown(mainMenuKey))
        {
            GoToMainMenu();
        }
    }

    public void SaveGameButton()
    {
        AdvancedSaveSystem.CreateNewSave(transform);
    }

    public void LoadLatestButton()
    {
        AdvancedSaveSystem.LoadLatestSave();
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(mainMenuSceneName);
    }
}