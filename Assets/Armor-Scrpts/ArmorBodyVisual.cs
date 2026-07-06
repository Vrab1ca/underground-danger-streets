using UnityEngine;

public class ArmorBodyVisual : MonoBehaviour
{
    [Header("Equipped Armor Models")]
    public GameObject strong100BodyModel;
    public GameObject strong50BodyModel;
    public GameObject weak100BodyModel;
    public GameObject weak50BodyModel;

    private void Awake()
    {
        HideEquippedArmor();
    }

    public void ShowEquippedArmor(ArmorItemType armorType)
    {
        gameObject.SetActive(true);

        HideAllModelsOnly();

        if (armorType == ArmorItemType.Strong100 && strong100BodyModel != null)
            strong100BodyModel.SetActive(true);

        if (armorType == ArmorItemType.Strong50 && strong50BodyModel != null)
            strong50BodyModel.SetActive(true);

        if (armorType == ArmorItemType.Weak100 && weak100BodyModel != null)
            weak100BodyModel.SetActive(true);

        if (armorType == ArmorItemType.Weak50 && weak50BodyModel != null)
            weak50BodyModel.SetActive(true);

        Debug.Log("SHOWING BODY ARMOR: " + armorType);
    }

    public void HideEquippedArmor()
    {
        // IMPORTANT:
        // Do NOT turn off the parent object.
        // Only hide the child models.
        HideAllModelsOnly();
    }

    private void HideAllModelsOnly()
    {
        if (strong100BodyModel != null)
            strong100BodyModel.SetActive(false);

        if (strong50BodyModel != null)
            strong50BodyModel.SetActive(false);

        if (weak100BodyModel != null)
            weak100BodyModel.SetActive(false);

        if (weak50BodyModel != null)
            weak50BodyModel.SetActive(false);
    }
}