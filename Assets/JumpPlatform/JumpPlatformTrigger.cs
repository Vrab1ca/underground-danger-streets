using UnityEngine;

public class JumpPlatformTrigger : MonoBehaviour
{
    [Header("Jump Settings")]
    public float jumpHeight = 18f;
    public float cooldown = 1f;

    [Header("Options")]
    public bool destroyAfterUse = false;

    private float nextJumpTime;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Jump platform touched by: " + other.name);
        TryJump(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryJump(other);
    }

    private void TryJump(Collider other)
    {
        if (Time.time < nextJumpTime)
            return;

        PlayerJumpBoostReceiver jumpReceiver = other.GetComponentInParent<PlayerJumpBoostReceiver>();

        if (jumpReceiver == null)
            jumpReceiver = other.GetComponentInChildren<PlayerJumpBoostReceiver>();

        if (jumpReceiver == null)
            return;

        nextJumpTime = Time.time + cooldown;

        jumpReceiver.BoostJump(jumpHeight);

        Debug.Log("Jump platform activated.");

        if (destroyAfterUse)
            Destroy(transform.root.gameObject, 0.1f);
    }
}