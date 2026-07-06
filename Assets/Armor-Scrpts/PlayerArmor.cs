using UnityEngine;

public class PlayerArmor : MonoBehaviour
{
    [Header("Armor")]
    public bool hasArmor;
    public ArmorItemType equippedArmorType;

    public float currentArmor;
    public float maxArmor;

    [Header("Zombie damage to armor")]
    public float armorDamagePerZombieHit;

    [Header("Visual")]
    public ArmorBodyVisual armorBodyVisual;

    [Header("Debug")]
    public bool debugMessages = true;

    private void Awake()
    {
        FindArmorBodyVisual();
    }

    private void FindArmorBodyVisual()
    {
        if (armorBodyVisual != null)
            return;

        armorBodyVisual = FindFirstObjectByType<ArmorBodyVisual>();

        if (armorBodyVisual == null)
            Debug.LogWarning("PlayerArmor cannot find ArmorBodyVisual. Assign EquippedArmorVisual manually.");
    }

    public void EquipArmor(ArmorItemType armorType)
    {
        FindArmorBodyVisual();

        equippedArmorType = armorType;
        hasArmor = true;

        if (armorType == ArmorItemType.Strong100)
        {
            maxArmor = 100f;
            armorDamagePerZombieHit = 3f;
        }
        else if (armorType == ArmorItemType.Strong50)
        {
            maxArmor = 50f;
            armorDamagePerZombieHit = 3f;
        }
        else if (armorType == ArmorItemType.Weak100)
        {
            maxArmor = 100f;
            armorDamagePerZombieHit = 5f;
        }
        else if (armorType == ArmorItemType.Weak50)
        {
            maxArmor = 50f;
            armorDamagePerZombieHit = 5f;
        }

        currentArmor = maxArmor;

        if (armorBodyVisual != null)
            armorBodyVisual.ShowEquippedArmor(armorType);
        else
            Debug.LogWarning("Armor equipped, but ArmorBodyVisual is missing.");

        Debug.Log(
            "EQUIPPED ARMOR: " + equippedArmorType +
            " | Armor: " + currentArmor + " / " + maxArmor +
            " | Zombie damage to armor: " + armorDamagePerZombieHit
        );
    }

    public float ProtectFromDamage(float incomingDamage)
    {
        if (!hasArmor || currentArmor <= 0f)
            return incomingDamage;

        currentArmor -= armorDamagePerZombieHit;

        if (currentArmor < 0f)
            currentArmor = 0f;

        Debug.Log(
            "Armor damaged: -" + armorDamagePerZombieHit +
            " | Armor left: " + currentArmor + " / " + maxArmor
        );

        if (currentArmor <= 0f)
        {
            hasArmor = false;
            currentArmor = 0f;

            if (armorBodyVisual != null)
                armorBodyVisual.HideEquippedArmor();

            Debug.Log("Armor broke.");
        }

        return 0f;
    }
}