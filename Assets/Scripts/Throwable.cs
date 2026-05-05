using UnityEngine;

public class Throwable : MonoBehaviour
{
    public GameObject throwablePrefab;
    public Transform throwPoint;
    public float throwForce = 14f;
    public int amount = 3;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
            Throw();
    }

    void Throw()
    {
        if (amount <= 0 || throwablePrefab == null || throwPoint == null) return;

        amount--;
        GameObject obj = Instantiate(throwablePrefab, throwPoint.position, throwPoint.rotation);

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
            rb.AddForce(throwPoint.forward * throwForce, ForceMode.VelocityChange);
    }
}
