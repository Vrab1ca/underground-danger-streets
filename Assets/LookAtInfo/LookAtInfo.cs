using UnityEngine;

public class LookAtInfo : MonoBehaviour
{
    [Header("Info")]
    public string objectName = "Object";

    [TextArea(2, 4)]
    public string description = "Description here.";

    [Header("Settings")]
    public float showDistance = 4f;
}