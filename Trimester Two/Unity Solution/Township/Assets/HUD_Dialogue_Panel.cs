using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Dialogue_Panel : UIPanel {

    public static HUD_Dialogue_Panel Instance;

    [Space]

    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private TextMeshProUGUI bodyText;

    [Space]

    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform buttonParent;

    private void Awake ()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy ( this.gameObject );
    }

    public override void Show ()
    {
        base.Show ();
    }

    public override void Hide ()
    {
        base.Hide ();
    }

    public void ShowDialogue (string header, string body, params DialogueButton[] buttons)
    {
        headerText.text = header;
        bodyText.text = body;

        for (int i = 0; i < buttonParent.childCount; i++)
        {
            Destroy ( buttonParent.GetChild ( i ).gameObject );
        }

        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            DialogueButton button = buttons[index];

            GameObject go = Instantiate ( buttonPrefab );
            Button b = go.GetComponent<Button> ();

            b.colors = buttons[i].GetColourBlock ();
            b.onClick.AddListener ( () => { button.OnClick?.Invoke (); Hide (); } );

            TextMeshProUGUI text = b.GetComponentInChildren<TextMeshProUGUI> ();
            text.text = buttons[i].Name;
            go.transform.SetParent ( buttonParent );
        }

        Show ();
    }
}

public class DialogueButton
{
    public enum Preset { Okay, Close, Cancel, Yes, No }
    public string Name { get; protected set; }
    public System.Action OnClick { get; protected set; }
    public Color Color { get; protected set; }

    public DialogueButton (string name, System.Action onClick, Color color)
    {
        this.Name = name;
        this.OnClick = onClick;
        this.Color = color;
    }

    public DialogueButton (Preset preset, System.Action onClick)
    {
        Name = preset.ToString ();
        OnClick = onClick;

        if (preset == Preset.Okay || preset == Preset.Yes)
        {
            Color = ColourGroupController.Instance.GetColour ( "UI_TextGreen" );
        }
        else
        {
            Color = ColourGroupController.Instance.GetColour ( "UI_TextRed" );
        }
    }

    public ColorBlock GetColourBlock ()
    {
        ColorBlock block = new ColorBlock ();
        block.colorMultiplier = 1.0f;
        block.fadeDuration = 0.1f;
        block.normalColor = Color;
        block.highlightedColor = Color * 1.1f;
        block.pressedColor = Color * 0.9f;
        return block;
    }
}
