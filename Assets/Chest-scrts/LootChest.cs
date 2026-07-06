using UnityEngine;

public class LootChest : MonoBehaviour
{
    public enum ChestType
    {
        Bronze,
        Silver,
        Gold
    }

    [Header("Chest Type")]
    public ChestType chestType = ChestType.Bronze;

    [Header("Open Settings")]
    public KeyCode openKey = KeyCode.E;
    public float openDistance = 3f;
    public bool destroyChestAfterOpen = false;
    public GameObject chestVisual;

    [Header("Drop Position")]
    public Transform dropCenter;
    public float dropRadius = 1.5f;
    public float dropUpForce = 2f;
    public float dropForwardForce = 1.5f;

    [Header("Common Loot")]
    public GameObject[] commonLootPrefabs;

    [Header("Rare Loot")]
    public GameObject[] rareLootPrefabs;

    [Header("Epic Loot")]
    public GameObject[] epicLootPrefabs;

    [Header("Debug")]
    public bool debugMessages = true;

    private Transform player;
    private bool opened;

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
            player = playerObject.transform;
        else
            Debug.LogWarning(gameObject.name + " cannot find Player tag.");

        if (dropCenter == null)
            dropCenter = transform;

        if (chestVisual == null)
            chestVisual = gameObject;
    }

    private void Update()
    {
        if (opened)
            return;

        if (player == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > openDistance)
            return;

        if (Input.GetKeyDown(openKey))
        {
            OpenChest();
        }
    }

    private void OpenChest()
    {
        if (opened)
            return;

        opened = true;

        if (debugMessages)
            Debug.Log("Opened " + chestType + " chest.");

        DropLootByChestType();

        if (destroyChestAfterOpen)
        {
            Destroy(gameObject);
        }
        else
        {
            if (chestVisual != null)
                chestVisual.SetActive(false);
        }
    }

    private void DropLootByChestType()
    {
        if (chestType == ChestType.Bronze)
        {
            DropRandomCommonLoot(Random.Range(1, 3)); // 1 or 2 common items
            TryDropRareLoot(15);                      // 15% rare
            TryDropEpicLoot(3);                       // 3% epic
            return;
        }

        if (chestType == ChestType.Silver)
        {
            DropRandomCommonLoot(Random.Range(2, 4)); // 2 or 3 common items
            TryDropRareLoot(40);                      // 40% rare
            TryDropEpicLoot(10);                      // 10% epic
            return;
        }

        if (chestType == ChestType.Gold)
        {
            DropRandomCommonLoot(Random.Range(3, 6)); // 3, 4, or 5 common items
            TryDropRareLoot(80);                      // 80% rare
            TryDropEpicLoot(40);                      // 40% epic
            return;
        }
    }

    private void DropRandomCommonLoot(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            SpawnRandomFromArray(commonLootPrefabs, "Common");
        }
    }

    private void TryDropRareLoot(int chancePercent)
    {
        int roll = Random.Range(1, 101);

        if (roll <= chancePercent)
            SpawnRandomFromArray(rareLootPrefabs, "Rare");
        else if (debugMessages)
            Debug.Log("Rare loot failed. Roll: " + roll + " Needed: " + chancePercent);
    }

    private void TryDropEpicLoot(int chancePercent)
    {
        int roll = Random.Range(1, 101);

        if (roll <= chancePercent)
            SpawnRandomFromArray(epicLootPrefabs, "Epic");
        else if (debugMessages)
            Debug.Log("Epic loot failed. Roll: " + roll + " Needed: " + chancePercent);
    }

    private void SpawnRandomFromArray(GameObject[] lootArray, string lootName)
    {
        if (lootArray == null || lootArray.Length <= 0)
        {
            if (debugMessages)
                Debug.LogWarning("No " + lootName + " loot prefabs assigned on " + gameObject.name);

            return;
        }

        int randomIndex = Random.Range(0, lootArray.Length);
        GameObject prefab = lootArray[randomIndex];

        if (prefab == null)
        {
            Debug.LogWarning("Missing prefab in " + lootName + " loot array.");
            return;
        }

        Vector3 randomOffset = Random.insideUnitSphere * dropRadius;
        randomOffset.y = 0f;

        Vector3 spawnPosition = dropCenter.position + randomOffset + Vector3.up * 0.5f;

        GameObject droppedItem = Instantiate(prefab, spawnPosition, Quaternion.identity);
        droppedItem.SetActive(true);

        Rigidbody rb = droppedItem.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 force = Vector3.up * dropUpForce;
            force += transform.forward * dropForwardForce;
            force += Random.insideUnitSphere * 0.7f;

            rb.AddForce(force, ForceMode.Impulse);
        }

        if (debugMessages)
            Debug.Log("Dropped " + lootName + " loot: " + prefab.name);
    }
}