using UnityEngine;

public class ArmorPickup : MonoBehaviour
{
    [Header("Armor Type")]
    public bool randomArmorType = false;
    public ArmorItemType armorType = ArmorItemType.Strong100;

    [Header("Pickup")]
    public KeyCode pickupKey = KeyCode.E;
    public float pickupDistance = 3f;
    public bool destroyAfterPickup = true;

    [Header("Animation")]
    public Transform modelToAnimate;
    public float rotateSpeed = 90f;
    public float bobSpeed = 2f;
    public float bobAmount = 0.15f;

    [Header("Debug")]
    public bool debugMessages = true;

    private Transform player;
    private PlayerArmorInventory armorInventory;
    private Vector3 startLocalPosition;
    private bool pickedUp;

    private void Start()
    {
        if (randomArmorType)
            armorType = GetRandomArmorType();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject == null)
        {
            Debug.LogWarning(gameObject.name + " cannot find Player tag.");
            return;
        }

        player = playerObject.transform;
        armorInventory = playerObject.GetComponent<PlayerArmorInventory>();

        if (armorInventory == null)
        {
            Debug.LogWarning("Player does not have PlayerArmorInventory.");
            return;
        }

        if (modelToAnimate == null)
            modelToAnimate = transform;

        startLocalPosition = modelToAnimate.localPosition;

        Debug.Log(gameObject.name + " armor pickup ready. Final type: " + armorType);
    }

    private void Update()
    {
        AnimatePickup();

        if (pickedUp)
            return;

        if (player == null || armorInventory == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > pickupDistance)
            return;

        if (Input.GetKeyDown(pickupKey))
            PickupArmor();
    }

    private void AnimatePickup()
    {
        if (modelToAnimate == null)
            return;

        modelToAnimate.Rotate(Vector3.up * rotateSpeed * Time.deltaTime, Space.World);

        Vector3 pos = startLocalPosition;
        pos.y += Mathf.Sin(Time.time * bobSpeed) * bobAmount;

        modelToAnimate.localPosition = pos;
    }

    private void PickupArmor()
    {
        bool added = armorInventory.AddArmorItem(armorType);

        if (!added)
            return;

        pickedUp = true;

        Debug.Log("PICKED ARMOR: " + armorType);

        if (destroyAfterPickup)
            Destroy(gameObject);
    }

    private ArmorItemType GetRandomArmorType()
    {
        int random = Random.Range(0, 4);

        if (random == 0)
            return ArmorItemType.Strong100;

        if (random == 1)
            return ArmorItemType.Strong50;

        if (random == 2)
            return ArmorItemType.Weak100;

        return ArmorItemType.Weak50;
    }
}