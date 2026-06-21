using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarryableBombRefillBox : MonoBehaviour
{
    [Header("Bomb Refill")]
    public int bombsToAdd = 5;

    [Header("State")]
    public bool isHeld;

    private Rigidbody rb;
    private Collider[] colliders;
    private Transform originalParent;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>();
        originalParent = transform.parent;
    }

    public void PickUp(Transform carryPoint)
    {
        if (carryPoint == null)
            return;

        isHeld = true;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.useGravity = false;
        rb.isKinematic = true;

        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

        transform.SetParent(carryPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        Debug.Log("Picked up bomb refill box.");
    }

    public void Drop(Vector3 dropVelocity)
    {
        isHeld = false;

        transform.SetParent(originalParent);

        foreach (Collider col in colliders)
        {
            col.enabled = true;
        }

        rb.isKinematic = false;
        rb.useGravity = true;
        rb.linearVelocity = dropVelocity;

        Debug.Log("Dropped bomb refill box.");
    }

    public void RemoveBox()
    {
        Destroy(gameObject);
    }
}