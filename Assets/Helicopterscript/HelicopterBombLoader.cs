using UnityEngine;

public class HelicopterBombLoader : MonoBehaviour
{
    [Header("References")]
    public HelicopterBombDropper bombDropper;

    [Header("Load Settings")]
    public float loadDistance = 5f;

    public bool TryLoadBox(CarryableBombRefillBox box, Transform player)
    {
        if (box == null)
            return false;

        if (bombDropper == null)
        {
            Debug.LogWarning("Bomb Dropper is missing on helicopter.");
            return false;
        }

        if (player == null)
            return false;

        float distance = Vector3.Distance(player.position, transform.position);

        if (distance > loadDistance)
        {
            Debug.Log("Too far from helicopter to load bomb box.");
            return false;
        }

        if (bombDropper.currentBombs >= bombDropper.maxBombs)
        {
            Debug.Log("Helicopter bombs are already full.");
            return false;
        }

        bombDropper.AddBombs(box.bombsToAdd);

        Debug.Log("Bomb refill box loaded into helicopter. Added bombs: " + box.bombsToAdd);

        box.RemoveBox();

        return true;
    }
}