using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingZombieTarget : MonoBehaviour
{
    [Header("UI")]
    public RectTransform rectTransform;
    public Image backgroundImage;
    public TMP_Text zombieText;

    [Header("Movement")]
    public float moveSpeed = 45f;
    public float wobbleAmount = 12f;
    public float wobbleSpeed = 5f;

    [Header("Life")]
    public float lifeTime = 2.5f;
    public float fadeSpeed = 4f;

    private Vector2 moveDirection;
    private Vector2 startPosition;
    private float timer;
    private bool dying;

    private LoadingZombieMiniGame miniGame;

    public void Setup(LoadingZombieMiniGame owner, Vector2 startPos)
    {
        miniGame = owner;

        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        rectTransform.anchoredPosition = startPos;
        startPosition = startPos;

        moveDirection = Random.insideUnitCircle.normalized;

        if (moveDirection == Vector2.zero)
            moveDirection = Vector2.right;

        timer = 0f;
        dying = false;
    }

    private void Update()
    {
        if (dying)
        {
            FadeOut();
            return;
        }

        timer += Time.deltaTime;

        MoveTarget();

        if (timer >= lifeTime)
        {
            StartDying(false);
        }
    }

    private void MoveTarget()
    {
        if (rectTransform == null)
            return;

        Vector2 pos = rectTransform.anchoredPosition;

        pos += moveDirection * moveSpeed * Time.deltaTime;

        pos.y += Mathf.Sin(Time.time * wobbleSpeed) * wobbleAmount * Time.deltaTime;

        rectTransform.anchoredPosition = pos;
    }

    public bool IsScopeOnTarget(RectTransform scopeMover)
    {
        if (scopeMover == null || rectTransform == null)
            return false;

        Vector3 scopeWorldPos = scopeMover.position;
        Vector3 targetWorldPos = rectTransform.position;

        float distance = Vector3.Distance(scopeWorldPos, targetWorldPos);

        return distance <= 45f;
    }

    public void Kill()
    {
        if (dying)
            return;

        StartDying(true);
    }

    private void StartDying(bool killedByPlayer)
    {
        dying = true;

        if (killedByPlayer && miniGame != null)
            miniGame.AddScore();

        if (zombieText != null)
        {
            if (killedByPlayer)
                zombieText.text = "X";
            else
                zombieText.text = "...";
        }
    }

    private void FadeOut()
    {
        if (backgroundImage != null)
        {
            Color color = backgroundImage.color;
            color.a = Mathf.MoveTowards(color.a, 0f, fadeSpeed * Time.deltaTime);
            backgroundImage.color = color;
        }

        if (zombieText != null)
        {
            Color textColor = zombieText.color;
            textColor.a = Mathf.MoveTowards(textColor.a, 0f, fadeSpeed * Time.deltaTime);
            zombieText.color = textColor;
        }

        if (backgroundImage == null || backgroundImage.color.a <= 0.02f)
        {
            Destroy(gameObject);
        }
    }
}