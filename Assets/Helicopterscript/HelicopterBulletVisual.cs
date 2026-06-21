using UnityEngine;

public class HelicopterBulletVisual : MonoBehaviour
{
    public float speed = 150f;
    public float lifeTime = 2f;

    private Vector3 targetPosition;
    private bool hasTarget;

    public void Init(Vector3 target, float bulletSpeed, float destroyTime)
    {
        targetPosition = target;
        speed = bulletSpeed;
        lifeTime = destroyTime;
        hasTarget = true;

        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        if (!hasTarget)
            return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );

        Vector3 direction = targetPosition - transform.position;

        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);

        if (Vector3.Distance(transform.position, targetPosition) < 0.15f)
        {
            Destroy(gameObject);
        }
    }
}