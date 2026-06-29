using UnityEngine;

public class VehicleSeatState : MonoBehaviour
{
    public bool playerInside;

    public void SetPlayerInside(bool inside)
    {
        playerInside = inside;
    }
}