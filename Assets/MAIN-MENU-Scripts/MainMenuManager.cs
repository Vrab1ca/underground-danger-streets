using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Scene")]
    public string gameSceneName = "Level1";

    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject settingsPanel;
    public GameObject storyPanel;
    public GameObject creditsPanel;
    public GameObject loadPanel;

    [Header("Buttons")]
    public Button continueButton;
    public Button slot1Button;
    public Button slot2Button;
    public Button slot3Button;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ShowMainPanel();
        UpdateSaveButtons();
    }

    public void NewGame()
    {
        PlayerPrefs.SetInt("AdvancedSave_ShouldLoad", 0);
        PlayerPrefs.Save();

        LoadingScreenLoader.LoadScene(gameSceneName);
    }

    public void ContinueGame()
    {
        if (AdvancedSaveSystem.HasAnySave())
        {
            AdvancedSaveSystem.LoadLatestSave();
        }
        else
        {
            Debug.Log("No save found.");
        }
    }

    public void LoadSlot1()
    {
        LoadSlot(1);
    }

    public void LoadSlot2()
    {
        LoadSlot(2);
    }

    public void LoadSlot3()
    {
        LoadSlot(3);
    }

    private void LoadSlot(int slot)
    {
        if (!HasSave(slot))
        {
            Debug.Log("Slot " + slot + " is empty.");
            return;
        }

        PlayerPrefs.SetInt("CurrentSlot", slot);
        PlayerPrefs.SetInt("LoadSavedGame", 1);
        PlayerPrefs.SetInt("LastSlot", slot);
        PlayerPrefs.Save();

        string sceneName = PlayerPrefs.GetString("SaveSlot" + slot + "_Scene", gameSceneName);

        LoadingScreenLoader.LoadScene(gameSceneName);
    }

    public void ShowMainPanel()
    {
        HideAllPanels();

        if (mainPanel != null)
            mainPanel.SetActive(true);

        UpdateSaveButtons();
    }

    public void ShowSettingsPanel()
    {
        HideAllPanels();

        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void ShowStoryPanel()
    {
        HideAllPanels();

        if (storyPanel != null)
            storyPanel.SetActive(true);
    }

    public void ShowCreditsPanel()
    {
        HideAllPanels();

        if (creditsPanel != null)
            creditsPanel.SetActive(true);
    }

    public void ShowLoadPanel()
    {
        HideAllPanels();

        if (loadPanel != null)
            loadPanel.SetActive(true);

        UpdateSaveButtons();
    }

    public void QuitGame()
    {
        Debug.Log("Quit game.");
        Application.Quit();
    }

    private void HideAllPanels()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (storyPanel != null) storyPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);
        if (loadPanel != null) loadPanel.SetActive(false);
    }

    private void UpdateSaveButtons()
    {
        bool hasAnySave = HasSave(1) || HasSave(2) || HasSave(3);

        if (continueButton != null)
            continueButton.interactable = hasAnySave;

        if (slot1Button != null)
            slot1Button.interactable = HasSave(1);

        if (slot2Button != null)
            slot2Button.interactable = HasSave(2);

        if (slot3Button != null)
            slot3Button.interactable = HasSave(3);
    }

    private bool HasSave(int slot)
    {
        return PlayerPrefs.GetInt("SaveSlot" + slot + "_HasSave", 0) == 1;
    }

    private void DeleteSlot(int slot)
    {
        PlayerPrefs.DeleteKey("SaveSlot" + slot + "_HasSave");
        PlayerPrefs.DeleteKey("SaveSlot" + slot + "_Scene");
        PlayerPrefs.DeleteKey("SaveSlot" + slot + "_X");
        PlayerPrefs.DeleteKey("SaveSlot" + slot + "_Y");
        PlayerPrefs.DeleteKey("SaveSlot" + slot + "_Z");
        PlayerPrefs.DeleteKey("SaveSlot" + slot + "_RotY");
        PlayerPrefs.Save();
    }
}