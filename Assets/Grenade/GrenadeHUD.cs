using TMPro;
using UnityEngine;

public class GrenadeHUD : MonoBehaviour
{
    public PlayerGrenadeInventory inventory;
    public TextMeshProUGUI grenadeText;

    private void Update()
    {
        if (grenadeText == null)
            return;

        if (inventory == null)
        {
            grenadeText.text = "Grenades: -";
            return;
        }

        grenadeText.text =
            "Selected: " + inventory.selectedGrenade +
            "\nNormal: " + inventory.normalGrenades + " / " + inventory.maxNormalGrenades +
            "\nMolotov: " + inventory.molotovs + " / " + inventory.maxMolotovs;
    }
}