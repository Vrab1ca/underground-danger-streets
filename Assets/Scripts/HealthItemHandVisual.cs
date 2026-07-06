using System.Collections;
using UnityEngine;

public class HealthItemHandVisual : MonoBehaviour
{
    [Header("Models")]
    public GameObject small20Model;
    public GameObject full100Model;
    public GameObject regen50Model;
    public GameObject medium50Model;

    [Header("Drink Animation")]
    public float drinkTime = 0.65f;
    public Vector3 normalLocalPosition = new Vector3(0.35f, -0.35f, 0.75f);
    public Vector3 normalLocalEuler = new Vector3(0f, 0f, 0f);

    public Vector3 drinkLocalPosition = new Vector3(0.1f, -0.05f, 0.45f);
    public Vector3 drinkLocalEuler = new Vector3(-65f, 25f, 20f);

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

    public void ShowItem(HealthItemType itemType)
    {
        gameObject.SetActive(true);

        HideAllModels();

        if (itemType == HealthItemType.Small20 && small20Model != null)
            small20Model.SetActive(true);

        if (itemType == HealthItemType.Full100 && full100Model != null)
            full100Model.SetActive(true);

        if (itemType == HealthItemType.Regen50 && regen50Model != null)
            regen50Model.SetActive(true);

        if (itemType == HealthItemType.Medium50 && medium50Model != null)
            medium50Model.SetActive(true);

        transform.localPosition = normalLocalPosition;
        transform.localRotation = Quaternion.Euler(normalLocalEuler);

        Debug.Log("Showing health item in hand: " + itemType);
    }

    public void HideItem()
    {
        HideAllModels();
        gameObject.SetActive(false);
    }

    private void HideAllModels()
    {
        if (small20Model != null)
            small20Model.SetActive(false);

        if (full100Model != null)
            full100Model.SetActive(false);

        if (regen50Model != null)
            regen50Model.SetActive(false);

        if (medium50Model != null)
            medium50Model.SetActive(false);
    }

    public IEnumerator DrinkAnimation()
    {
        isAnimating = true;

        float timer = 0f;

        Vector3 startPos = normalLocalPosition;
        Quaternion startRot = Quaternion.Euler(normalLocalEuler);

        Vector3 endPos = drinkLocalPosition;
        Quaternion endRot = Quaternion.Euler(drinkLocalEuler);

        while (timer < drinkTime * 0.5f)
        {
            timer += Time.deltaTime;

            float t = timer / (drinkTime * 0.5f);
            t = Mathf.Clamp01(t);

            transform.localPosition = Vector3.Lerp(startPos, endPos, t);
            transform.localRotation = Quaternion.Slerp(startRot, endRot, t);

            yield return null;
        }

        timer = 0f;

        Vector3 backStartPos = transform.localPosition;
        Quaternion backStartRot = transform.localRotation;

        while (timer < drinkTime * 0.5f)
        {
            timer += Time.deltaTime;

            float t = timer / (drinkTime * 0.5f);
            t = Mathf.Clamp01(t);

            transform.localPosition = Vector3.Lerp(backStartPos, normalLocalPosition, t);
            transform.localRotation = Quaternion.Slerp(backStartRot, Quaternion.Euler(normalLocalEuler), t);

            yield return null;
        }

        transform.localPosition = normalLocalPosition;
        transform.localRotation = Quaternion.Euler(normalLocalEuler);

        isAnimating = false;
    }
}