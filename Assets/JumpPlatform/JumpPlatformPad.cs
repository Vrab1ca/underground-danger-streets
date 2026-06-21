using UnityEngine;

public class JumpPlatformPad : MonoBehaviour
{
    [Header("Jump")]
    public float jumpHeight = 25f;
    public float cooldown = 1f;

    [Header("Detection Box")]
    public Vector3 detectionCenter = new Vector3(0f, 0.6f, 0f);
    public Vector3 detectionSize = new Vector3(3f, 1.2f, 3f);

    [Header("Options")]
    public bool destroyAfterUse = false;

    private float nextJumpTime;

    private void Update()
    {
        if (Time.time < nextJumpTime)
            return;

        Vector3 worldCenter = transform.TransformPoint(detectionCenter);

        Collider[] hits = Physics.OverlapBox(
            worldCenter,
            detectionSize * 0.5f,
            transform.rotation,
            ~0,
            QueryTriggerInteraction.Ignore
        );

        foreach (Collider hit in hits)
        {
            PlayerJumpBoostReceiver receiver = hit.GetComponentInParent<PlayerJumpBoostReceiver>();

            if (receiver == null)
                continue;

            nextJumpTime = Time.time + cooldown;

            receiver.BoostJump(jumpHeight);

            Debug.Log("Jump platform activated by: " + hit.name);

            if (destroyAfterUse)
                Destroy(gameObject, 0.1f);

            return;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(
            transform.TransformPoint(detectionCenter),
            transform.rotation,
            Vector3.one
        );

        Gizmos.DrawWireCube(Vector3.zero, detectionSize);

        Gizmos.matrix = oldMatrix;
    }
}