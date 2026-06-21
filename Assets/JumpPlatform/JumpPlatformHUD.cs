using TMPro;
using UnityEngine;

public class JumpPlatformHUD : MonoBehaviour
{
    public JumpPlatformInventory inventory;
    public TextMeshProUGUI platformText;

    private void Update()
    {
        if (platformText == null)
            return;

        if (inventory == null)
        {
            platformText.text = "Jump Platforms: -";
            return;
        }

        platformText.text = "Jump Platforms: " + inventory.currentPlatforms + " / " + inventory.maxPlatforms;
    }
}