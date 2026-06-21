using UnityEngine;

public class PlayerCarryBombBox : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public Transform carryPoint;

    [Header("Keys")]
    public KeyCode pickOrLoadKey = KeyCode.E;
    public KeyCode dropKey = KeyCode.X;

    [Header("Settings")]
    public float pickupDistance = 3f;
    public float loadDistance = 5f;
    public float dropForwardForce = 2f;
    public float dropUpForce = 1f;

    private CarryableBombRefillBox carriedBox;

    private void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    private void Update()
    {
        if (carriedBox == null)
        {
            if (Input.GetKeyDown(pickOrLoadKey))
            {
                TryPickUpBox();
            }
        }
        else
        {
            if (Input.GetKeyDown(pickOrLoadKey))
            {
                TryLoadBoxIntoHelicopter();
            }

            if (Input.GetKeyDown(dropKey))
            {
                DropBox();
            }
        }
    }

    private void TryPickUpBox()
    {
        CarryableBombRefillBox box = FindBoxInFront();

        if (box == null)
        {
            box = FindNearestBox();
        }

        if (box == null)
        {
            Debug.Log("No bomb refill box nearby.");
            return;
        }

        carriedBox = box;
        carriedBox.PickUp(carryPoint);
    }

    private CarryableBombRefillBox FindBoxInFront()
    {
        if (playerCamera == null)
            return null;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, pickupDistance, ~0, QueryTriggerInteraction.Ignore))
        {
            return hit.collider.GetComponentInParent<CarryableBombRefillBox>();
        }

        return null;
    }

    private CarryableBombRefillBox FindNearestBox()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, pickupDistance);

        CarryableBombRefillBox nearestBox = null;
        float nearestDistance = Mathf.Infinity;

        foreach (Collider hit in hits)
        {
            CarryableBombRefillBox box = hit.GetComponentInParent<CarryableBombRefillBox>();

            if (box == null)
                continue;

            if (box.isHeld)
                continue;

            float distance = Vector3.Distance(transform.position, box.transform.position);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestBox = box;
            }
        }

        return nearestBox;
    }

    private void TryLoadBoxIntoHelicopter()
    {
        HelicopterBombLoader loader = FindHelicopterLoader();

        if (loader == null)
        {
            Debug.Log("No helicopter nearby to load the box.");
            return;
        }

        bool loaded = loader.TryLoadBox(carriedBox, transform);

        if (loaded)
        {
            carriedBox = null;
        }
    }

    private HelicopterBombLoader FindHelicopterLoader()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, loadDistance);

        foreach (Collider hit in hits)
        {
            HelicopterBombLoader loader = hit.GetComponentInParent<HelicopterBombLoader>();

            if (loader != null)
                return loader;
        }

        return null;
    }

    private void DropBox()
    {
        if (carriedBox == null)
            return;

        Vector3 dropVelocity = Vector3.zero;

        if (playerCamera != null)
        {
            dropVelocity = playerCamera.transform.forward * dropForwardForce;
            dropVelocity += Vector3.up * dropUpForce;
        }

        carriedBox.Drop(dropVelocity);
        carriedBox = null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupDistance);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, loadDistance);
    }
}