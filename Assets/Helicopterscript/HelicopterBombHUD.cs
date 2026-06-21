using TMPro;
using UnityEngine;

public class HelicopterBombHUD : MonoBehaviour
{
    public HelicopterBombDropper bombDropper;
    public HelicopterController helicopterController;
    public TextMeshProUGUI bombText;

    private void Update()
    {
        if (bombText == null)
            return;

        if (bombDropper == null)
        {
            bombText.text = "Bombs: -";
            return;
        }

        bombText.text = "Bombs: " + bombDropper.currentBombs + " / " + bombDropper.maxBombs;

        if (helicopterController != null)
        {
            bombText.gameObject.SetActive(helicopterController.canFly);
        }
    }
}