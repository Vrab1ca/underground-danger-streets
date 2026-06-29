using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class AdvancedSaveSystem
{
    private const string SaveListKey = "AdvancedSave_List";
    private const string LoadSaveIdKey = "AdvancedSave_LoadId";
    private const string ShouldLoadKey = "AdvancedSave_ShouldLoad";
    private const string LastSaveIdKey = "AdvancedSave_LastId";

    [Serializable]
    public class SaveData
    {
        public string id;
        public string saveName;
        public string sceneName;
        public string dateTime;

        public float x;
        public float y;
        public float z;

        public float rotY;
    }

    [Serializable]
    private class SaveList
    {
        public List<SaveData> saves = new List<SaveData>();
    }

    public static List<SaveData> GetSaves()
    {
        SaveList list = LoadList();
        return list.saves;
    }

    public static bool HasAnySave()
    {
        return GetSaves().Count > 0;
    }

    public static SaveData GetSaveById(string id)
    {
        List<SaveData> saves = GetSaves();

        foreach (SaveData save in saves)
        {
            if (save.id == id)
                return save;
        }

        return null;
    }

    public static void CreateNewSave(Transform player, string customName = "")
    {
        if (player == null)
        {
            Debug.LogWarning("Cannot save: Player is missing.");
            return;
        }

        SaveList list = LoadList();

        string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        SaveData data = new SaveData();
        data.id = DateTime.Now.Ticks.ToString();
        data.sceneName = SceneManager.GetActiveScene().name;
        data.dateTime = date;

        if (string.IsNullOrWhiteSpace(customName))
            data.saveName = "Save " + date;
        else
            data.saveName = customName;

        data.x = player.position.x;
        data.y = player.position.y;
        data.z = player.position.z;

        data.rotY = player.eulerAngles.y;

        // newest save goes first
        list.saves.Insert(0, data);

        SaveListToPrefs(list);

        PlayerPrefs.SetString(LastSaveIdKey, data.id);
        PlayerPrefs.Save();

        Debug.Log("Created save: " + data.saveName);
    }

    public static void LoadSave(string id)
    {
        SaveData save = GetSaveById(id);

        if (save == null)
        {
            Debug.LogWarning("Save not found.");
            return;
        }

        PlayerPrefs.SetString(LoadSaveIdKey, id);
        PlayerPrefs.SetInt(ShouldLoadKey, 1);
        PlayerPrefs.SetString(LastSaveIdKey, id);
        PlayerPrefs.Save();

        Time.timeScale = 1f;

        SceneManager.LoadScene(save.sceneName);
    }

    public static void LoadLatestSave()
    {
        List<SaveData> saves = GetSaves();

        if (saves.Count <= 0)
        {
            Debug.Log("No saves found.");
            return;
        }

        string lastId = PlayerPrefs.GetString(LastSaveIdKey, "");

        if (!string.IsNullOrEmpty(lastId) && GetSaveById(lastId) != null)
        {
            LoadSave(lastId);
            return;
        }

        LoadSave(saves[0].id);
    }

    public static void DeleteSave(string id)
    {
        SaveList list = LoadList();

        for (int i = list.saves.Count - 1; i >= 0; i--)
        {
            if (list.saves[i].id == id)
            {
                Debug.Log("Deleted save: " + list.saves[i].saveName);
                list.saves.RemoveAt(i);
                break;
            }
        }

        SaveListToPrefs(list);

        string lastId = PlayerPrefs.GetString(LastSaveIdKey, "");

        if (lastId == id)
            PlayerPrefs.DeleteKey(LastSaveIdKey);

        PlayerPrefs.Save();
    }

    public static bool TryApplyLoadedSaveToPlayer(Transform player)
    {
        if (player == null)
            return false;

        bool shouldLoad = PlayerPrefs.GetInt(ShouldLoadKey, 0) == 1;

        if (!shouldLoad)
            return false;

        string id = PlayerPrefs.GetString(LoadSaveIdKey, "");
        SaveData save = GetSaveById(id);

        if (save == null)
        {
            Debug.LogWarning("Could not apply save. Save missing.");
            PlayerPrefs.SetInt(ShouldLoadKey, 0);
            PlayerPrefs.Save();
            return false;
        }

        CharacterController controller = player.GetComponent<CharacterController>();

        if (controller != null)
            controller.enabled = false;

        player.position = new Vector3(save.x, save.y, save.z);
        player.rotation = Quaternion.Euler(0f, save.rotY, 0f);

        if (controller != null)
            controller.enabled = true;

        PlayerPrefs.SetInt(ShouldLoadKey, 0);
        PlayerPrefs.Save();

        Debug.Log("Loaded player from save: " + save.saveName);

        return true;
    }

    private static SaveList LoadList()
    {
        string json = PlayerPrefs.GetString(SaveListKey, "");

        if (string.IsNullOrEmpty(json))
            return new SaveList();

        SaveList list = JsonUtility.FromJson<SaveList>(json);

        if (list == null || list.saves == null)
            return new SaveList();

        return list;
    }

    private static void SaveListToPrefs(SaveList list)
    {
        string json = JsonUtility.ToJson(list);
        PlayerPrefs.SetString(SaveListKey, json);
        PlayerPrefs.Save();
    }
}