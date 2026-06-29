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

    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();

        if (button != null)
            button.onClick.AddListener(StartRebind);
    }

    private void OnEnable()
    {
        Refresh();
    }

    private void Update()
    {
        if (!waitingForKey)
            return;

        // This prevents the button click from instantly binding Mouse0
        if (Time.frameCount == startFrame)
            return;

        if (!Input.anyKeyDown)
            return;

        KeyCode pressedKey = GetPressedKey();

        if (pressedKey == KeyCode.None)
            return;

        GameKeybinds.SetKey(action, pressedKey);

        waitingForKey = false;
        Refresh();
    }

    public void StartRebind()
    {
        waitingForKey = true;
        startFrame = Time.frameCount;

        if (keyText != null)
            keyText.text = "Press key...";
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
        foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(keyCode))
                return keyCode;
        }

        return KeyCode.None;
    }
}