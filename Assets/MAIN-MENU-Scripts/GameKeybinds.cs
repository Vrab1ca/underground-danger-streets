using System;
using System.Collections.Generic;
using UnityEngine;

public enum GameAction
{
    Fire,
    AimScope,
    Reload,

    Slot1,
    Slot2,
    Slot3,
    Slot4,
    Slot5,

    Pickup,
    DropWeapon,
    EnterExitVehicle,

    FuelCanPickup,
    FuelCanRefuel,
    FuelCanDrop,

    BombBoxTakeLoad,
    BombBoxDrop,

    Sprint,
    Crouch,
    Jump,

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

        { GameAction.Slot1, KeyCode.Alpha1 },
        { GameAction.Slot2, KeyCode.Alpha2 },
        { GameAction.Slot3, KeyCode.Alpha3 },
        { GameAction.Slot4, KeyCode.Alpha4 },
        { GameAction.Slot5, KeyCode.Alpha5 },

        { GameAction.Pickup, KeyCode.F },
        { GameAction.DropWeapon, KeyCode.G },
        { GameAction.EnterExitVehicle, KeyCode.E },

        { GameAction.FuelCanPickup, KeyCode.F },
        { GameAction.FuelCanRefuel, KeyCode.F },
        { GameAction.FuelCanDrop, KeyCode.X },

        { GameAction.BombBoxTakeLoad, KeyCode.E },
        { GameAction.BombBoxDrop, KeyCode.G },

        { GameAction.Sprint, KeyCode.LeftShift },
        { GameAction.Crouch, KeyCode.LeftControl },
        { GameAction.Jump, KeyCode.Space },

        { GameAction.HelicopterBomb, KeyCode.B },

        { GameAction.SaveGame, KeyCode.F5 },
        { GameAction.LoadGame, KeyCode.F9 },
        { GameAction.PauseMenu, KeyCode.Escape },
    };

    public static KeyCode GetKeyCode(GameAction action)
    {
        string savedKey = PlayerPrefs.GetString(GetPrefsKey(action), "");

        if (!string.IsNullOrEmpty(savedKey))
        {
            if (Enum.TryParse(savedKey, out KeyCode key))
                return key;
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

        Debug.Log("Changed key: " + action + " = " + keyCode);
    }

    public static void ResetAll()
    {
        foreach (GameAction action in Enum.GetValues(typeof(GameAction)))
        {
            PlayerPrefs.DeleteKey(GetPrefsKey(action));
        }

        PlayerPrefs.Save();

        Debug.Log("All controls reset.");
    }

    public static string GetKeyName(GameAction action)
    {
        return PrettyKeyName(GetKeyCode(action));
    }

    public static string GetActionName(GameAction action)
    {
        switch (action)
        {
            case GameAction.Fire: return "Fire";
            case GameAction.AimScope: return "Aim / Scope";
            case GameAction.Reload: return "Reload";

            case GameAction.Slot1: return "Slot 1 - Weapon 1";
            case GameAction.Slot2: return "Slot 2 - Weapon 2";
            case GameAction.Slot3: return "Slot 3 - Grenade";
            case GameAction.Slot4: return "Slot 4 - Molotov";
            case GameAction.Slot5: return "Slot 5 - Jump Platform";

            case GameAction.Pickup: return "Pick Up";
            case GameAction.DropWeapon: return "Drop Weapon";
            case GameAction.EnterExitVehicle: return "Enter / Exit Vehicle";

            case GameAction.FuelCanPickup: return "Fuel Can Pick Up";
            case GameAction.FuelCanRefuel: return "Fuel Can Refuel";
            case GameAction.FuelCanDrop: return "Fuel Can Drop";

            case GameAction.BombBoxTakeLoad: return "Bomb Box Take / Load";
            case GameAction.BombBoxDrop: return "Bomb Box Drop";

            case GameAction.Sprint: return "Sprint";
            case GameAction.Crouch: return "Crouch";
            case GameAction.Jump: return "Jump";

            case GameAction.HelicopterBomb: return "Helicopter Bomb";

            case GameAction.SaveGame: return "Save Game";
            case GameAction.LoadGame: return "Load Game";
            case GameAction.PauseMenu: return "Pause Menu";
        }

        return action.ToString();
    }

    private static string GetPrefsKey(GameAction action)
    {
        return "Keybind_" + action;
    }

    private static string PrettyKeyName(KeyCode key)
    {
        if (key == KeyCode.Mouse0) return "Left Mouse";
        if (key == KeyCode.Mouse1) return "Right Mouse";
        if (key == KeyCode.Mouse2) return "Middle Mouse";

        if (key == KeyCode.LeftShift) return "Left Shift";
        if (key == KeyCode.RightShift) return "Right Shift";

        if (key == KeyCode.LeftControl) return "Left Ctrl";
        if (key == KeyCode.RightControl) return "Right Ctrl";

        if (key == KeyCode.Alpha1) return "1";
        if (key == KeyCode.Alpha2) return "2";
        if (key == KeyCode.Alpha3) return "3";
        if (key == KeyCode.Alpha4) return "4";
        if (key == KeyCode.Alpha5) return "5";

        if (key == KeyCode.Space) return "Space";
        if (key == KeyCode.Escape) return "Escape";

        return key.ToString();
    }
}