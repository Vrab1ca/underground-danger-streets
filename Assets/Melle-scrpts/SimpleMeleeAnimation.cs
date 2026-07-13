using System.Collections;
using UnityEngine;

public class SimpleMeleeAnimation : MonoBehaviour
{
    [Header("Attack Movement")]
    public Vector3 attackPositionOffset = new Vector3(0f, 0f, 0.25f);
    public Vector3 attackRotationOffset = new Vector3(-20f, 35f, 10f);

    [Header("Speed")]
    public float moveToAttackTime = 0.08f;
    public float returnTime = 0.12f;

    private Vector3 startLocalPosition;
    private Quaternion startLocalRotation;

    private Coroutine animationCoroutine;

    private void Awake()
    {
        SaveStartPosition();
    }

    private void OnEnable()
    {
        SaveStartPosition();
        ResetPosition();
    }

    private void SaveStartPosition()
    {
        startLocalPosition = transform.localPosition;
        startLocalRotation = transform.localRotation;
    }

    public void PlayAttack()
    {
        if (!gameObject.activeInHierarchy)
            return;

        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        animationCoroutine = StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        Vector3 attackPosition = startLocalPosition + attackPositionOffset;

        Quaternion attackRotation =
            startLocalRotation *
            Quaternion.Euler(attackRotationOffset);

        float timer = 0f;

        while (timer < moveToAttackTime)
        {
            timer += Time.deltaTime;

            float percent = moveToAttackTime <= 0f
                ? 1f
                : timer / moveToAttackTime;

            transform.localPosition = Vector3.Lerp(
                startLocalPosition,
                attackPosition,
                percent
            );

            transform.localRotation = Quaternion.Slerp(
                startLocalRotation,
                attackRotation,
                percent
            );

            yield return null;
        }

        timer = 0f;

        while (timer < returnTime)
        {
            timer += Time.deltaTime;

            float percent = returnTime <= 0f
                ? 1f
                : timer / returnTime;

            transform.localPosition = Vector3.Lerp(
                attackPosition,
                startLocalPosition,
                percent
            );

            transform.localRotation = Quaternion.Slerp(
                attackRotation,
                startLocalRotation,
                percent
            );

            yield return null;
        }

        ResetPosition();
        animationCoroutine = null;
    }

    public void ResetPosition()
    {
        transform.localPosition = startLocalPosition;
        transform.localRotation = startLocalRotation;
    }

    private void OnDisable()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }

        ResetPosition();
    }
}