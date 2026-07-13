using UnityEngine;

public class CloudMover : MonoBehaviour
{
    [Header("Movement")]
    public Vector3 moveDirection =
        new Vector3(1f, 0f, 0f);

    [Min(0f)]
    public float moveSpeed = 1.5f;

    [Header("World Wrapping")]
    public float minimumX = -200f;
    public float maximumX = 200f;

    private void Update()
    {
        if (moveSpeed <= 0f)
            return;

        if (moveDirection.sqrMagnitude <= 0.001f)
            return;

        Vector3 direction =
            moveDirection.normalized;

        transform.position +=
            direction *
            moveSpeed *
            Time.deltaTime;

        Vector3 position =
            transform.position;

        if (position.x > maximumX)
        {
            position.x = minimumX;
            transform.position = position;
        }
        else if (position.x < minimumX)
        {
            position.x = maximumX;
            transform.position = position;
        }
    }
}