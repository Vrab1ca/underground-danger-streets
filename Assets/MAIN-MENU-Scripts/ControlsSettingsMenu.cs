using UnityEngine;

public class ControlsSettingsMenu : MonoBehaviour
{
    public KeybindButton[] keybindButtons;

    private void OnEnable()
    {
        RefreshAll();
    }

    public void RefreshAll()
    {
        foreach (KeybindButton button in keybindButtons)
        {
            if (button != null)
                button.Refresh();
        }
    }

    public void ResetAllKeybinds()
    {
        GameKeybinds.ResetAll();
        RefreshAll();
    }
}