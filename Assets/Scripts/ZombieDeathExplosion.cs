using UnityEngine;

public class ZombieDeathExplosion : MonoBehaviour
{
    [Header("Explosion Pieces")]
    public GameObject piecePrefab;
    public int pieceCount = 18;

    [Header("Piece Settings")]
    public float pieceSize = 0.25f;
    public float pieceLifeTime = 4f;

    [Header("Explosion Force")]
    public float explosionForce = 7f;
    public float explosionRadius = 3f;
    public float upwardForce = 2f;

    [Header("Spawn Area")]
    public float spawnHeight = 1.2f;
    public float spawnWidth = 0.5f;

    [Header("Optional")]
    public Material pieceMaterial;

    private bool exploded;

    public void Explode()
    {
        if (exploded)
            return;

        exploded = true;

        Vector3 explosionCenter = transform.position + Vector3.up * 1f;

        for (int i = 0; i < pieceCount; i++)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-spawnWidth, spawnWidth),
                Random.Range(0f, spawnHeight),
                Random.Range(-spawnWidth, spawnWidth)
            );

            Vector3 spawnPosition = transform.position + randomOffset;

            GameObject piece;

            if (piecePrefab != null)
            {
                piece = Instantiate(piecePrefab, spawnPosition, Random.rotation);
            }
            else
            {
                piece = GameObject.CreatePrimitive(PrimitiveType.Cube);
                piece.transform.position = spawnPosition;
                piece.transform.rotation = Random.rotation;
                piece.transform.localScale = Vector3.one * pieceSize;
            }

            if (pieceMaterial != null)
            {
                Renderer renderer = piece.GetComponent<Renderer>();

                if (renderer != null)
                    renderer.material = pieceMaterial;
            }

            Rigidbody rb = piece.GetComponent<Rigidbody>();

            if (rb == null)
                rb = piece.AddComponent<Rigidbody>();

            rb.mass = 0.2f;

            rb.AddExplosionForce(
                explosionForce,
                explosionCenter,
                explosionRadius,
                upwardForce,
                ForceMode.Impulse
            );

            Destroy(piece, pieceLifeTime);
        }
    }
}