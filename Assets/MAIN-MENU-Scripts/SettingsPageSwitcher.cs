using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPageSwitcher : MonoBehaviour
{
    [Header("Pages")]
    public GameObject[] pages;

    [Header("Page Names")]
    public string[] pageNames;

    [Header("UI")]
    public TMP_Text pageTitleText;
    public TMP_Text pageNumberText;
    public Button previousButton;
    public Button nextButton;

    private int currentPage;

    private void Start()
    {
        ShowPage(0);
    }

    public void NextPage()
    {
        currentPage++;

        if (currentPage >= pages.Length)
            currentPage = 0;

        ShowPage(currentPage);
    }

    public void PreviousPage()
    {
        currentPage--;

        if (currentPage < 0)
            currentPage = pages.Length - 1;

        ShowPage(currentPage);
    }

    public void ShowPage(int pageIndex)
    {
        if (pages == null || pages.Length == 0)
            return;

        currentPage = Mathf.Clamp(pageIndex, 0, pages.Length - 1);

        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i] != null)
                pages[i].SetActive(i == currentPage);
        }

        if (pageTitleText != null)
        {
            if (pageNames != null && currentPage < pageNames.Length)
                pageTitleText.text = pageNames[currentPage];
            else
                pageTitleText.text = "Settings";
        }

        if (pageNumberText != null)
        {
            pageNumberText.text = (currentPage + 1) + " / " + pages.Length;
        }
    }
}