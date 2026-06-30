using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeybindButton : MonoBehaviour
{
    [Header("Action")]
    public GameAction action;
    public string actionDisplayName;

    [Header("UI")]
    public TMP_Text actionText;
    public TMP_Text keyText;
    public Button button;

    private bool waitingForKey;
    private int startFrame;

    private void Start()
    {
        AutoFindReferences();

        if (button != null)
        {
            button.onClick.RemoveListener(StartRebind);
            button.onClick.AddListener(StartRebind);
        }
        else
        {
            Debug.LogWarning(gameObject.name + " has no Button component.");
        }

        Refresh();
    }

    private void OnEnable()
    {
        AutoFindReferences();
        Refresh();
    }

    private void Update()
    {
        if (!waitingForKey)
            return;

        if (Time.frameCount == startFrame)
            return;

        KeyCode pressedKey = GetPressedKey();

        if (pressedKey == KeyCode.None)
            return;

        GameKeybinds.SetKey(action, pressedKey);

        waitingForKey = false;
        Refresh();

        Debug.Log("Changed key: " + action + " = " + pressedKey);
    }

    private void AutoFindReferences()
    {
        if (button == null)
            button = GetComponent<Button>();

        if (button == null)
            button = GetComponentInChildren<Button>(true);

        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);

        foreach (TMP_Text text in texts)
        {
            if (text.name == "ActionText")
                actionText = text;

            if (text.name == "KeyText")
                keyText = text;
        }
    }

    public void StartRebind()
    {
        waitingForKey = true;
        startFrame = Time.frameCount;

        if (keyText != null)
            keyText.text = "PRESS KEY...";

        Debug.Log("Waiting for new key for: " + action);
    }

    public void Refresh()
    {
        if (actionText != null)
        {
            if (string.IsNullOrEmpty(actionDisplayName))
                actionText.text = action.ToString();
            else
                actionText.text = actionDisplayName;
        }

        if (keyText != null)
            keyText.text = GameKeybinds.GetKeyName(action);
    }

    private KeyCode GetPressedKey()
    {
        // Mouse buttons
        if (Input.GetKeyDown(KeyCode.Mouse0))
            return KeyCode.Mouse0;

        if (Input.GetKeyDown(KeyCode.Mouse1))
            return KeyCode.Mouse1;

        if (Input.GetKeyDown(KeyCode.Mouse2))
            return KeyCode.Mouse2;

        // Keyboard
        foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (keyCode == KeyCode.Mouse0 || keyCode == KeyCode.Mouse1 || keyCode == KeyCode.Mouse2)
                continue;

            if (Input.GetKeyDown(keyCode))
                return keyCode;
        }

        return KeyCode.None;
    }
}