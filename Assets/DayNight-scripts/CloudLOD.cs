using System.Collections;
using UnityEngine;

public class CloudLOD : MonoBehaviour
{
    [Header("Cloud Particle Systems")]
    public ParticleSystem cloudBody;
    public ParticleSystem cloudBottom;

    [Header("Camera")]
    public Transform cameraTransform;

    [Header("Distances")]
    [Tooltip("Inside this distance, show the complete cloud.")]
    public float fullDetailDistance = 180f;

    [Tooltip("Outside this distance, hide the complete cloud.")]
    public float maximumVisibleDistance = 450f;

    [Header("Performance")]
    [Tooltip("How often the script checks the distance.")]
    public float checkInterval = 0.5f;

    private ParticleSystemRenderer bodyRenderer;
    private ParticleSystemRenderer bottomRenderer;

    private void Awake()
    {
        FindReferences();

        if (cloudBody != null)
        {
            bodyRenderer =
                cloudBody.GetComponent
                    <ParticleSystemRenderer>();
        }

        if (cloudBottom != null)
        {
            bottomRenderer =
                cloudBottom.GetComponent
                    <ParticleSystemRenderer>();
        }
    }

    private void Start()
    {
        StartCoroutine(
            CheckDistanceRoutine()
        );
    }

    private void FindReferences()
    {
        if (cameraTransform == null &&
            Camera.main != null)
        {
            cameraTransform =
                Camera.main.transform;
        }

        if (cloudBody == null)
        {
            Transform bodyTransform =
                transform.Find("CloudBody");

            if (bodyTransform != null)
            {
                cloudBody =
                    bodyTransform.GetComponent
                        <ParticleSystem>();
            }
        }

        if (cloudBottom == null)
        {
            Transform bottomTransform =
                transform.Find("CloudBottom");

            if (bottomTransform != null)
            {
                cloudBottom =
                    bottomTransform.GetComponent
                        <ParticleSystem>();
            }
        }
    }

    private IEnumerator CheckDistanceRoutine()
    {
        WaitForSeconds wait =
            new WaitForSeconds(
                Mathf.Max(0.1f, checkInterval)
            );

        while (true)
        {
            UpdateCloudDetail();
            yield return wait;
        }
    }

    private void UpdateCloudDetail()
    {
        if (cameraTransform == null)
        {
            FindReferences();
            return;
        }

        float distanceSquared =
            (transform.position -
             cameraTransform.position)
            .sqrMagnitude;

        float fullDetailSquared =
            fullDetailDistance *
            fullDetailDistance;

        float visibleDistanceSquared =
            maximumVisibleDistance *
            maximumVisibleDistance;

        bool bodyVisible =
            distanceSquared <=
            visibleDistanceSquared;

        bool bottomVisible =
            distanceSquared <=
            fullDetailSquared;

        SetParticleVisibility(
            cloudBody,
            bodyRenderer,
            bodyVisible
        );

        SetParticleVisibility(
            cloudBottom,
            bottomRenderer,
            bottomVisible
        );
    }

    private void SetParticleVisibility(
        ParticleSystem particleSystem,
        ParticleSystemRenderer particleRenderer,
        bool visible
    )
    {
        if (particleSystem == null ||
            particleRenderer == null)
        {
            return;
        }

        particleRenderer.enabled = visible;

        if (visible)
        {
            if (particleSystem.isPaused)
            {
                particleSystem.Play();
            }
        }
        else
        {
            if (particleSystem.isPlaying)
            {
                particleSystem.Pause();
            }
        }
    }
}