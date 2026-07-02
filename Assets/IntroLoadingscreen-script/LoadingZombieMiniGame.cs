using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingZombieMiniGame : MonoBehaviour
{
    [Header("References")]
    public RectTransform spawnArea;
    public RectTransform scopeMover;
    public TMP_Text scoreText;

    [Header("Spawn")]
    public float spawnEverySeconds = 0.8f;
    public int maxZombiesOnScreen = 5;

    [Header("Zombie UI")]
    public Vector2 zombieSize = new Vector2(70f, 70f);
    public Color zombiePanelColor = new Color(0.1f, 1f, 0.15f, 0.18f);
    public Color zombieTextColor = new Color(0.1f, 1f, 0.15f, 0.75f);

    [Header("Kill")]
    public KeyCode shootKey = KeyCode.Mouse0;
    public float killCooldown = 0.15f;

    private float spawnTimer;
    private float nextKillTime;
    private int score;

    private readonly List<LoadingZombieTarget> zombies = new List<LoadingZombieTarget>();

    private void Start()
    {
        score = 0;
        UpdateScoreText();
    }

    private void Update()
    {
        CleanupNullZombies();
        SpawnZombies();
        HandleShoot();
    }

    private void SpawnZombies()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer < spawnEverySeconds)
            return;

        spawnTimer = 0f;

        if (zombies.Count >= maxZombiesOnScreen)
            return;

        CreateZombie();
    }

    private void CreateZombie()
    {
        if (spawnArea == null)
            return;

        GameObject zombieObject = new GameObject("Loading_Zombie_Target");
        zombieObject.transform.SetParent(spawnArea, false);

        RectTransform rect = zombieObject.AddComponent<RectTransform>();
        rect.sizeDelta = zombieSize;
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);

        Image image = zombieObject.AddComponent<Image>();
        image.color = zombiePanelColor;
        image.raycastTarget = false;

        GameObject textObject = new GameObject("ZombieText");
        textObject.transform.SetParent(zombieObject.transform, false);

        RectTransform textRect = textObject.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        TMP_Text text = textObject.AddComponent<TextMeshProUGUI>();
        text.text = "Z";
        text.fontSize = 46f;
        text.alignment = TextAlignmentOptions.Center;
        text.color = zombieTextColor;
        text.raycastTarget = false;

        LoadingZombieTarget target = zombieObject.AddComponent<LoadingZombieTarget>();
        target.rectTransform = rect;
        target.backgroundImage = image;
        target.zombieText = text;

        Vector2 pos = GetRandomPosition();
        target.Setup(this, pos);

        zombies.Add(target);
    }

    private Vector2 GetRandomPosition()
    {
        if (spawnArea == null)
            return Vector2.zero;

        float width = spawnArea.rect.width;
        float height = spawnArea.rect.height;

        float x = Random.Range(-width * 0.38f, width * 0.38f);
        float y = Random.Range(-height * 0.30f, height * 0.30f);

        return new Vector2(x, y);
    }

    private void HandleShoot()
    {
        if (!Input.GetKeyDown(shootKey))
            return;

        if (Time.time < nextKillTime)
            return;

        nextKillTime = Time.time + killCooldown;

        LoadingZombieTarget bestTarget = null;
        float bestDistance = Mathf.Infinity;

        foreach (LoadingZombieTarget zombie in zombies)
        {
            if (zombie == null)
                continue;

            if (!zombie.IsScopeOnTarget(scopeMover))
                continue;

            float distance = Vector3.Distance(scopeMover.position, zombie.transform.position);

            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestTarget = zombie;
            }
        }

        if (bestTarget != null)
        {
            bestTarget.Kill();
        }
    }

    public void AddScore()
    {
        score++;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = "Zombies Killed: " + score;
    }

    private void CleanupNullZombies()
    {
        for (int i = zombies.Count - 1; i >= 0; i--)
        {
            if (zombies[i] == null)
                zombies.RemoveAt(i);
        }
    }
}