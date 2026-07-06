using TMPro;
using UnityEngine;

public class ArmorHUD : MonoBehaviour
{
    [Header("References")]
    public PlayerArmor playerArmor;
    public TMP_Text armorText;

    private void Start()
    {
        if (playerArmor == null)
            playerArmor = FindFirstObjectByType<PlayerArmor>();
    }

    private void Update()
    {
        if (playerArmor == null)
            playerArmor = FindFirstObjectByType<PlayerArmor>();

        if (armorText == null)
            return;

        if (playerArmor == null || !playerArmor.hasArmor)
        {
            armorText.text =
                "Armor: 0 / 0" +
                "\nType: None";

            return;
        }

        armorText.text =
            "Armor: " +
            Mathf.CeilToInt(playerArmor.currentArmor) +
            " / " +
            Mathf.CeilToInt(playerArmor.maxArmor) +
            "\nType: " +
            playerArmor.equippedArmorType;
    }
}