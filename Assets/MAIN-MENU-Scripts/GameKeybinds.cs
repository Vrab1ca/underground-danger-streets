using System;
using System.Collections.Generic;
using UnityEngine;

public enum GameAction
{
    Fire,
    AimScope,
    Reload,

    Pickup,
    DropWeapon,
    EnterExitVehicle,
    Refuel,

    Sprint,
    Crouch,
    Jump,

    ThrowGrenade,
    SwitchGrenade,
    PlaceJumpPlatform,

    HelicopterBomb,

    SaveGame,
    LoadGame,
    PauseMenu
}

public static class GameKeybinds
{
    private static Dictionary<GameAction, KeyCode> defaultKeys = new Dictionary<GameAction, KeyCode>()
    {
        { GameAction.Fire, KeyCode.Mouse0 },
        { GameAction.AimScope, KeyCode.Mouse1 },
        { GameAction.Reload, KeyCode.R },

        { GameAction.Pickup, KeyCode.F },
        { GameAction.DropWeapon, KeyCode.G },
        { GameAction.EnterExitVehicle, KeyCode.E },
        { GameAction.Refuel, KeyCode.F },

        { GameAction.Sprint, KeyCode.LeftShift },
        { GameAction.Crouch, KeyCode.LeftControl },
        { GameAction.Jump, KeyCode.Space },

        { GameAction.ThrowGrenade, KeyCode.Q },
        { GameAction.SwitchGrenade, KeyCode.Z },
        { GameAction.PlaceJumpPlatform, KeyCode.T },

        { GameAction.HelicopterBomb, KeyCode.B },

        { GameAction.SaveGame, KeyCode.F5 },
        { GameAction.LoadGame, KeyCode.F9 },
        { GameAction.PauseMenu, KeyCode.Escape },
    };

    public static KeyCode GetKeyCode(GameAction action)
    {
        string key = PlayerPrefs.GetString(GetPrefsKey(action), "");

        if (!string.IsNullOrEmpty(key))
        {
            if (Enum.TryParse(key, out KeyCode savedKey))
                return savedKey;
        }

        return defaultKeys[action];
    }

    public static bool GetKey(GameAction action)
    {
        return Input.GetKey(GetKeyCode(action));
    }

    public static bool GetKeyDown(GameAction action)
    {
        return Input.GetKeyDown(GetKeyCode(action));
    }

    public static bool GetKeyUp(GameAction action)
    {
        return Input.GetKeyUp(GetKeyCode(action));
    }

    public static void SetKey(GameAction action, KeyCode keyCode)
    {
        PlayerPrefs.SetString(GetPrefsKey(action), keyCode.ToString());
        PlayerPrefs.Save();

        Debug.Log(action + " changed to " + keyCode);
    }

    public static void ResetKey(GameAction action)
    {
        PlayerPrefs.DeleteKey(GetPrefsKey(action));
        PlayerPrefs.Save();
    }

    public static void ResetAll()
    {
        foreach (GameAction action in Enum.GetValues(typeof(GameAction)))
        {
            PlayerPrefs.DeleteKey(GetPrefsKey(action));
        }

        PlayerPrefs.Save();

        Debug.Log("All keybinds reset.");
    }

    public static string GetKeyName(GameAction action)
    {
        KeyCode key = GetKeyCode(action);
        return MakeKeyNamePretty(key);
    }

    private static string GetPrefsKey(GameAction action)
    {
        return "Keybind_" + action;
    }

    private static string MakeKeyNamePretty(KeyCode key)
    {
        if (key == KeyCode.Mouse0)
            return "Left Mouse";

        if (key == KeyCode.Mouse1)
            return "Right Mouse";

        if (key == KeyCode.Mouse2)
            return "Middle Mouse";

        if (key == KeyCode.LeftShift)
            return "Left Shift";

        if (key == KeyCode.LeftControl)
            return "Left Ctrl";

        if (key == KeyCode.Space)
            return "Space";

        return key.ToString();
    }
}