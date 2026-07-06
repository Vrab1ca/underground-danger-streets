using System.Collections;
using UnityEngine;

public class ArmorHandVisual : MonoBehaviour
{
    [Header("Models")]
    public GameObject strong100Model;
    public GameObject strong50Model;
    public GameObject weak100Model;
    public GameObject weak50Model;

    [Header("Hand Position")]
    public Vector3 normalLocalPosition = new Vector3(0.25f, -0.45f, 0.85f);
    public Vector3 normalLocalEuler = new Vector3(0f, 0f, 0f);

    [Header("Put Armor Animation")]
    public float putTime = 0.75f;
    public Vector3 putLocalPosition = new Vector3(0f, -0.75f, 0.55f);
    public Vector3 putLocalEuler = new Vector3(60f, 0f, 0f);

    private bool isAnimating;

    public bool IsAnimating
    {
        get { return isAnimating; }
    }

    private void OnEnable()
    {
        transform.localPosition = normalLocalPosition;
        transform.localRotation = Quaternion.Euler(normalLocalEuler);
    }

    public void ShowArmor(ArmorItemType armorType)
    {
        gameObject.SetActive(true);
        HideAllModels();

        if (armorType == ArmorItemType.Strong100 && strong100Model != null)
            strong100Model.SetActive(true);

        if (armorType == ArmorItemType.Strong50 && strong50Model != null)
            strong50Model.SetActive(true);

        if (armorType == ArmorItemType.Weak100 && weak100Model != null)
            weak100Model.SetActive(true);

        if (armorType == ArmorItemType.Weak50 && weak50Model != null)
            weak50Model.SetActive(true);

        transform.localPosition = normalLocalPosition;
        transform.localRotation = Quaternion.Euler(normalLocalEuler);
    }

    public void HideArmor()
    {
        HideAllModels();
        gameObject.SetActive(false);
    }

    private void HideAllModels()
    {
        if (strong100Model != null) strong100Model.SetActive(false);
        if (strong50Model != null) strong50Model.SetActive(false);
        if (weak100Model != null) weak100Model.SetActive(false);
        if (weak50Model != null) weak50Model.SetActive(false);
    }

    public IEnumerator PutArmorAnimation()
    {
        isAnimating = true;

        float timer = 0f;

        Vector3 startPos = normalLocalPosition;
        Quaternion startRot = Quaternion.Euler(normalLocalEuler);

        Vector3 endPos = putLocalPosition;
        Quaternion endRot = Quaternion.Euler(putLocalEuler);

        while (timer < putTime * 0.5f)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / (putTime * 0.5f));

            transform.localPosition = Vector3.Lerp(startPos, endPos, t);
            transform.localRotation = Quaternion.Slerp(startRot, endRot, t);

            yield return null;
        }

        timer = 0f;

        Vector3 backStartPos = transform.localPosition;
        Quaternion backStartRot = transform.localRotation;

        while (timer < putTime * 0.5f)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / (putTime * 0.5f));

            transform.localPosition = Vector3.Lerp(backStartPos, normalLocalPosition, t);
            transform.localRotation = Quaternion.Slerp(backStartRot, Quaternion.Euler(normalLocalEuler), t);

            yield return null;
        }

        transform.localPosition = normalLocalPosition;
        transform.localRotation = Quaternion.Euler(normalLocalEuler);

        isAnimating = false;
    }
}