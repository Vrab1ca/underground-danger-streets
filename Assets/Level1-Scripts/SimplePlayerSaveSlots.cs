using UnityEngine;
using UnityEngine.SceneManagement;

public class SimplePlayerSaveSlots : MonoBehaviour
{
    public KeyCode saveKey = KeyCode.F5;
    public KeyCode loadKey = KeyCode.F9;
    public KeyCode mainMenuKey = KeyCode.Escape;

    public string mainMenuSceneName = "MainMenu";

    private void Start()
    {
        bool shouldLoad = PlayerPrefs.GetInt("LoadSavedGame", 0) == 1;

        if (shouldLoad)
        {
            LoadPlayer();
            PlayerPrefs.SetInt("LoadSavedGame", 0);
            PlayerPrefs.Save();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(saveKey))
        {
            SavePlayer();
        }

        if (Input.GetKeyDown(loadKey))
        {
            LoadPlayer();
        }

        if (Input.GetKeyDown(mainMenuKey))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            SceneManager.LoadScene(mainMenuSceneName);
        }
    }

    public void SavePlayer()
    {
        int slot = PlayerPrefs.GetInt("CurrentSlot", 1);

        string key = "SaveSlot" + slot + "_";

        PlayerPrefs.SetInt(key + "HasSave", 1);
        PlayerPrefs.SetString(key + "Scene", SceneManager.GetActiveScene().name);

        PlayerPrefs.SetFloat(key + "X", transform.position.x);
        PlayerPrefs.SetFloat(key + "Y", transform.position.y);
        PlayerPrefs.SetFloat(key + "Z", transform.position.z);

        PlayerPrefs.SetFloat(key + "RotY", transform.eulerAngles.y);

        PlayerPrefs.SetInt("LastSlot", slot);

        PlayerPrefs.Save();

        Debug.Log("Game saved in slot " + slot);
    }

    public void LoadPlayer()
    {
        int slot = PlayerPrefs.GetInt("CurrentSlot", 1);

        string key = "SaveSlot" + slot + "_";

        bool hasSave = PlayerPrefs.GetInt(key + "HasSave", 0) == 1;

        if (!hasSave)
        {
            Debug.Log("No save in slot " + slot);
            return;
        }

        CharacterController controller = GetComponent<CharacterController>();

        if (controller != null)
            controller.enabled = false;

        float x = PlayerPrefs.GetFloat(key + "X", transform.position.x);
        float y = PlayerPrefs.GetFloat(key + "Y", transform.position.y);
        float z = PlayerPrefs.GetFloat(key + "Z", transform.position.z);

        float rotY = PlayerPrefs.GetFloat(key + "RotY", transform.eulerAngles.y);

        transform.position = new Vector3(x, y, z);
        transform.rotation = Quaternion.Euler(0f, rotY, 0f);

        if (controller != null)
            controller.enabled = true;

        Debug.Log("Game loaded from slot " + slot);
    }
}