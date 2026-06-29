using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveDropdownMenu : MonoBehaviour
{
    [Header("UI")]
    public TMP_Dropdown saveDropdown;
    public Button loadButton;
    public Button deleteButton;

    [Header("Delete Confirm Panel")]
    public GameObject deleteConfirmPanel;
    public TMP_Text deleteConfirmText;
    public Button yesDeleteButton;
    public Button cancelDeleteButton;

    private List<AdvancedSaveSystem.SaveData> saves = new List<AdvancedSaveSystem.SaveData>();
    private string selectedDeleteId;

    private void Start()
    {
        if (deleteConfirmPanel != null)
            deleteConfirmPanel.SetActive(false);

        RefreshDropdown();
    }

    private void OnEnable()
    {
        RefreshDropdown();
    }

    public void RefreshDropdown()
    {
        saves = AdvancedSaveSystem.GetSaves();

        if (saveDropdown != null)
        {
            saveDropdown.ClearOptions();

            List<string> options = new List<string>();

            if (saves.Count == 0)
            {
                options.Add("No saves found");
            }
            else
            {
                foreach (AdvancedSaveSystem.SaveData save in saves)
                {
                    string option =
                        save.saveName +
                        " | " +
                        save.sceneName +
                        " | " +
                        save.dateTime;

                    options.Add(option);
                }
            }

            saveDropdown.AddOptions(options);
            saveDropdown.value = 0;
            saveDropdown.RefreshShownValue();
        }

        bool hasSaves = saves.Count > 0;

        if (loadButton != null)
            loadButton.interactable = hasSaves;

        if (deleteButton != null)
            deleteButton.interactable = hasSaves;
    }

    public void LoadSelectedSave()
    {
        if (saves.Count == 0)
        {
            Debug.Log("No save selected.");
            return;
        }

        int index = GetDropdownIndex();
        string saveId = saves[index].id;

        AdvancedSaveSystem.LoadSave(saveId);
    }

    public void AskDeleteSelectedSave()
    {
        if (saves.Count == 0)
            return;

        int index = GetDropdownIndex();
        AdvancedSaveSystem.SaveData save = saves[index];

        selectedDeleteId = save.id;

        if (deleteConfirmPanel != null)
            deleteConfirmPanel.SetActive(true);

        if (deleteConfirmText != null)
        {
            deleteConfirmText.text =
                "Are you sure you want to delete this save?\n\n" +
                save.saveName;
        }
    }

    public void ConfirmDeleteSave()
    {
        if (string.IsNullOrEmpty(selectedDeleteId))
            return;

        AdvancedSaveSystem.DeleteSave(selectedDeleteId);

        selectedDeleteId = "";

        if (deleteConfirmPanel != null)
            deleteConfirmPanel.SetActive(false);

        RefreshDropdown();
    }

    public void CancelDeleteSave()
    {
        selectedDeleteId = "";

        if (deleteConfirmPanel != null)
            deleteConfirmPanel.SetActive(false);
    }

    private int GetDropdownIndex()
    {
        if (saveDropdown == null)
            return 0;

        return Mathf.Clamp(saveDropdown.value, 0, saves.Count - 1);
    }
}