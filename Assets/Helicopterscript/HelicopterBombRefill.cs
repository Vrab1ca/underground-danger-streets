using UnityEngine;

public class HelicopterBombRefill : MonoBehaviour
{
    public int bombsToAdd = 5;

    private void OnTriggerEnter(Collider other)
    {
        HelicopterBombDropper dropper = other.GetComponentInParent<HelicopterBombDropper>();

        if (dropper == null)
            return;

        dropper.AddBombs(bombsToAdd);

        Destroy(gameObject);
    }
}