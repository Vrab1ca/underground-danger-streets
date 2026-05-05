using System.Collections;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public int maxAlive = 8;
    public float spawnEverySeconds = 4f;

    private int alive;

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnEverySeconds);

            if (alive >= maxAlive || enemyPrefab == null || spawnPoints.Length == 0)
                continue;

            Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject enemy = Instantiate(enemyPrefab, point.position, point.rotation);
            alive++;

            Health health = enemy.GetComponent<Health>();
            if (health != null)
                health.onDeath.AddListener(() => alive--);
            else
                StartCoroutine(DecreaseLater(enemy));
        }
    }

    IEnumerator DecreaseLater(GameObject enemy)
    {
        while (enemy != null) yield return null;
        alive--;
    }
}
