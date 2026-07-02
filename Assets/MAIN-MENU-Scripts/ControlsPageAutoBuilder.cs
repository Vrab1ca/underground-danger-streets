using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControlsPageAutoBuilder : MonoBehaviour
{
    [Header("Settings")]
    public bool buildOnStart = true;
    public bool clearOldRows = true;

    [Header("Row Style")]
    public float rowHeight = 55f;
    public Color rowColor = new Color(0f, 0f, 0f, 0.45f);
    public Color rowHoverColor = new Color(0.25f, 0.25f, 0.25f, 0.8f);
    public Color keyBoxColor = new Color(0f, 0f, 0f, 0.55f);
    public Color textColor = Color.white;
    public Color headerColor = Color.yellow;

    [Header("Optional Reset Button")]
    public Button resetButton;

    private void Start()
    {
        if (buildOnStart)
            BuildControls();

        if (resetButton != null)
        {
            resetButton.onClick.RemoveListener(ResetControls);
            resetButton.onClick.AddListener(ResetControls);
        }
    }

    private void OnEnable()
    {
        RefreshButtons();
    }

    public void BuildControls()
    {
        if (clearOldRows)
            ClearContent();

        CreateHeader("COMBAT");
        CreateRow(GameAction.Fire);
        CreateRow(GameAction.AimScope);
        CreateRow(GameAction.Reload);

        CreateHeader("QUICK SLOTS");
        CreateRow(GameAction.Slot1);
        CreateRow(GameAction.Slot2);
        CreateRow(GameAction.Slot3);
        CreateRow(GameAction.Slot4);
        CreateRow(GameAction.Slot5);

        CreateHeader("INTERACTION");
        CreateRow(GameAction.Pickup);
        CreateRow(GameAction.DropWeapon);
        CreateRow(GameAction.EnterExitVehicle);

        CreateHeader("FUEL CAN");
        CreateRow(GameAction.FuelCanPickup);
        CreateRow(GameAction.FuelCanRefuel);
        CreateRow(GameAction.FuelCanDrop);

        CreateHeader("BOMB BOX");
        CreateRow(GameAction.BombBoxTakeLoad);
        CreateRow(GameAction.BombBoxDrop);

        CreateHeader("MOVEMENT");
        CreateRow(GameAction.Sprint);
        CreateRow(GameAction.Crouch);
        CreateRow(GameAction.Jump);

        CreateHeader("HELICOPTER");
        CreateRow(GameAction.HelicopterBomb);

        CreateHeader("SYSTEM");
        CreateRow(GameAction.SaveGame);
        CreateRow(GameAction.LoadGame);
        CreateRow(GameAction.PauseMenu);

        RefreshButtons();
    }

    private void ClearContent()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    private void CreateHeader(string title)
    {
        GameObject headerObject = new GameObject(title + "_Header");
        headerObject.transform.SetParent(transform, false);

        RectTransform rect = headerObject.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(0f, 40f);

        LayoutElement layout = headerObject.AddComponent<LayoutElement>();
        layout.minHeight = 40f;
        layout.preferredHeight = 40f;

        TMP_Text text = headerObject.AddComponent<TextMeshProUGUI>();
        text.text = title;
        text.fontSize = 24f;
        text.color = headerColor;
        text.alignment = TextAlignmentOptions.MidlineLeft;
        text.raycastTarget = false;
    }

    private void CreateRow(GameAction action)
    {
        GameObject row = new GameObject(action + "_Row");
        row.transform.SetParent(transform, false);

        RectTransform rowRect = row.AddComponent<RectTransform>();
        rowRect.sizeDelta = new Vector2(0f, rowHeight);

        LayoutElement layout = row.AddComponent<LayoutElement>();
        layout.minHeight = rowHeight;
        layout.preferredHeight = rowHeight;

        Image rowImage = row.AddComponent<Image>();
        rowImage.color = rowColor;
        rowImage.raycastTarget = true;

        Button rowButton = row.AddComponent<Button>();
        rowButton.targetGraphic = rowImage;

        ColorBlock colors = rowButton.colors;
        colors.normalColor = rowColor;
        colors.highlightedColor = rowHoverColor;
        colors.pressedColor = Color.gray;
        colors.selectedColor = rowHoverColor;
        rowButton.colors = colors;

        KeybindButton keybindButton = row.AddComponent<KeybindButton>();
        keybindButton.action = action;
        keybindButton.button = rowButton;

        TMP_Text actionText = CreateText("ActionText", row.transform);
        actionText.text = GameKeybinds.GetActionName(action);
        actionText.fontSize = 22f;
        actionText.color = textColor;
        actionText.alignment = TextAlignmentOptions.MidlineLeft;

        RectTransform actionRect = actionText.GetComponent<RectTransform>();
        actionRect.anchorMin = new Vector2(0f, 0f);
        actionRect.anchorMax = new Vector2(0.65f, 1f);
        actionRect.offsetMin = new Vector2(20f, 0f);
        actionRect.offsetMax = new Vector2(-10f, 0f);

        GameObject keyBox = new GameObject("KeyBox");
        keyBox.transform.SetParent(row.transform, false);

        RectTransform keyBoxRect = keyBox.AddComponent<RectTransform>();
        keyBoxRect.anchorMin = new Vector2(0.70f, 0.15f);
        keyBoxRect.anchorMax = new Vector2(0.98f, 0.85f);
        keyBoxRect.offsetMin = Vector2.zero;
        keyBoxRect.offsetMax = Vector2.zero;

        Image keyBoxImage = keyBox.AddComponent<Image>();
        keyBoxImage.color = keyBoxColor;
        keyBoxImage.raycastTarget = false;

        TMP_Text keyText = CreateText("KeyText", keyBox.transform);
        keyText.text = GameKeybinds.GetKeyName(action);
        keyText.fontSize = 22f;
        keyText.color = textColor;
        keyText.alignment = TextAlignmentOptions.Center;

        RectTransform keyTextRect = keyText.GetComponent<RectTransform>();
        keyTextRect.anchorMin = Vector2.zero;
        keyTextRect.anchorMax = Vector2.one;
        keyTextRect.offsetMin = Vector2.zero;
        keyTextRect.offsetMax = Vector2.zero;

        keybindButton.actionText = actionText;
        keybindButton.keyText = keyText;
    }

    private TMP_Text CreateText(string name, Transform parent)
    {
        GameObject textObject = new GameObject(name);
        textObject.transform.SetParent(parent, false);

        RectTransform rect = textObject.AddComponent<RectTransform>();
        rect.localScale = Vector3.one;

        TMP_Text text = textObject.AddComponent<TextMeshProUGUI>();
        text.raycastTarget = false;

        return text;
    }

    public void RefreshButtons()
    {
        KeybindButton[] buttons = GetComponentsInChildren<KeybindButton>(true);

        foreach (KeybindButton button in buttons)
        {
            if (button != null)
                button.Refresh();
        }
    }

    public void ResetControls()
    {
        GameKeybinds.ResetAll();
        RefreshButtons();
    }
}