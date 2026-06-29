using System.Collections;
using UnityEngine;

public class HitFlash : MonoBehaviour
{
    [Header("Flash Settings")]
    public Color flashColor = Color.red;
    public float flashTime = 0.12f;

    private Renderer[] renderers;
    private Color[][] originalColors;
    private Coroutine flashCoroutine;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();

        originalColors = new Color[renderers.Length][];

        for (int i = 0; i < renderers.Length; i++)
        {
            Material[] materials = renderers[i].materials;
            originalColors[i] = new Color[materials.Length];

            for (int j = 0; j < materials.Length; j++)
            {
                originalColors[i][j] = materials[j].color;
            }
        }
    }

    public void Flash()
    {
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        SetColor(flashColor);

        yield return new WaitForSeconds(flashTime);

        ResetColor();

        flashCoroutine = null;
    }

    private void SetColor(Color color)
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            Material[] materials = renderers[i].materials;

            for (int j = 0; j < materials.Length; j++)
            {
                materials[j].color = color;
            }
        }
    }

    private void ResetColor()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            Material[] materials = renderers[i].materials;

            for (int j = 0; j < materials.Length; j++)
            {
                materials[j].color = originalColors[i][j];
            }
        }
    }
}