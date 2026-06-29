using UnityEngine;

public class LevelSaveLoader : MonoBehaviour
{
    private void Start()
    {
        AdvancedSaveSystem.TryApplyLoadedSaveToPlayer(transform);
    }
}