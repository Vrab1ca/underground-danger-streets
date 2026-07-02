using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeybindButton : MonoBehaviour
{
    [Header("Action")]
    public GameAction action;

    [Header("UI")]
    public TMP_Text actionText;
    public TMP_Text keyText;
    public Button button;

    private bool waitingForKey;
    private int startFrame;

    private void Awake()
    {
        AutoFind();

        if (button != null)
        {
            button.onClick.RemoveListener(StartRebind);
            button.onClick.AddListener(StartRebind);
        }

        Refresh();
    }

    private void OnEnable()
    {
        AutoFind();
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
    }

    private void AutoFind()
    {
        if (button == null)
            button = GetComponent<Button>();

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

        Debug.Log("Waiting for key: " + action);
    }

    public void Refresh()
    {
        if (actionText != null)
            actionText.text = GameKeybinds.GetActionName(action);

        if (keyText != null)
            keyText.text = GameKeybinds.GetKeyName(action);
    }

    private KeyCode GetPressedKey()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
            return KeyCode.Mouse0;

        if (Input.GetKeyDown(KeyCode.Mouse1))
            return KeyCode.Mouse1;

        if (Input.GetKeyDown(KeyCode.Mouse2))
            return KeyCode.Mouse2;

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