using UnityEngine;

public class PlayerArmor : MonoBehaviour
{
    [Header("Armor")]
    public bool hasArmor;
    public ArmorItemType equippedArmorType;

    public float currentArmor;
    public float maxArmor;

    [Header("Damage To Armor Per Zombie Hit")]
    public float armorDamagePerZombieHit = 3f;

    [Header("Visual")]
    public ArmorBodyVisual armorBodyVisual;

    [Header("Debug")]
    public bool debugMessages = true;

    public bool IsArmorActive
    {
        get
        {
            return hasArmor &&
                   currentArmor > 0f;
        }
    }

    private void Awake()
    {
        FindArmorBodyVisual();
        DisablePhysicsOnArmorVisual();

        if (!hasArmor)
        {
            currentArmor = 0f;
            maxArmor = 0f;
        }
    }

    private void FindArmorBodyVisual()
    {
        if (armorBodyVisual != null)
            return;

        armorBodyVisual =
            GetComponentInChildren<ArmorBodyVisual>(true);

        if (armorBodyVisual == null)
        {
            armorBodyVisual =
                GetComponentInParent<ArmorBodyVisual>();
        }

        if (armorBodyVisual == null)
        {
            armorBodyVisual =
                FindFirstObjectByType<ArmorBodyVisual>();
        }
    }

    public void EquipArmor(
        ArmorItemType armorType
    )
    {
        FindArmorBodyVisual();
        DisablePhysicsOnArmorVisual();

        equippedArmorType = armorType;
        hasArmor = true;

        switch (armorType)
        {
            case ArmorItemType.Strong100:
                maxArmor = 100f;
                armorDamagePerZombieHit = 3f;
                break;

            case ArmorItemType.Strong50:
                maxArmor = 50f;
                armorDamagePerZombieHit = 3f;
                break;

            case ArmorItemType.Weak100:
                maxArmor = 100f;
                armorDamagePerZombieHit = 5f;
                break;

            case ArmorItemType.Weak50:
                maxArmor = 50f;
                armorDamagePerZombieHit = 5f;
                break;

            default:
                maxArmor = 50f;
                armorDamagePerZombieHit = 5f;
                break;
        }

        // Equipping armor changes only armor health.
        // It never changes PlayerHealth.
        currentArmor = maxArmor;

        if (armorBodyVisual != null)
        {
            armorBodyVisual.ShowEquippedArmor(
                armorType
            );
        }

        DisablePhysicsOnArmorVisual();

        if (debugMessages)
        {
            Debug.Log(
                "ARMOR EQUIPPED: " +
                equippedArmorType +
                " | Armor health: " +
                currentArmor +
                " / " +
                maxArmor +
                " | Player HP was not changed."
            );
        }
    }

    public float ProtectFromDamage(
        float incomingDamage
    )
    {
        if (incomingDamage <= 0f)
            return 0f;

        // No armor means the damage continues to HP.
        if (!IsArmorActive)
            return incomingDamage;

        // While armor is active, only armor loses health.
        float armorDamage =
            Mathf.Max(
                0f,
                armorDamagePerZombieHit
            );

        currentArmor -= armorDamage;

        currentArmor =
            Mathf.Clamp(
                currentArmor,
                0f,
                maxArmor
            );

        if (debugMessages)
        {
            Debug.Log(
                "ARMOR BLOCKED DAMAGE" +
                " | Armor damage: -" +
                armorDamage +
                " | Armor left: " +
                currentArmor +
                " / " +
                maxArmor +
                " | Player HP damage: 0"
            );
        }

        if (currentArmor <= 0f)
        {
            BreakArmor();
        }

        // The complete current hit is blocked.
        // HP must not lose health from this hit.
        return 0f;
    }

    public void BreakArmor()
    {
        hasArmor = false;
        currentArmor = 0f;

        if (armorBodyVisual != null)
        {
            armorBodyVisual.HideEquippedArmor();
        }

        if (debugMessages)
        {
            Debug.Log(
                "Armor broke. The next enemy hit can damage HP."
            );
        }
    }

    private void DisablePhysicsOnArmorVisual()
    {
        if (armorBodyVisual == null)
            return;

        Collider[] colliders =
            armorBodyVisual.GetComponentsInChildren
                <Collider>(true);

        for (int i = 0;
             i < colliders.Length;
             i++)
        {
            colliders[i].enabled = false;
        }

        Rigidbody[] rigidbodies =
            armorBodyVisual.GetComponentsInChildren
                <Rigidbody>(true);

        for (int i = 0;
             i < rigidbodies.Length;
             i++)
        {
            rigidbodies[i].useGravity = false;
            rigidbodies[i].isKinematic = true;
            rigidbodies[i].detectCollisions = false;
        }
    }
}