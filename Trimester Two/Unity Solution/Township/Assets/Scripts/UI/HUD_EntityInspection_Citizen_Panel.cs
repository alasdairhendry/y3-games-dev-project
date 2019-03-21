using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HUD_EntityInspection_Citizen_Panel : UIPanel {

    //private Character citizen;
    private GameObject target;

    [SerializeField] private Transform tabView;
    [SerializeField] private Transform leftHorizontalView;
    [SerializeField] private Transform rightHorizontalView;

    [SerializeField] private GameObject keyValueText_Prefab;
    [SerializeField] private Text header_Label;

    //[SerializeField] private Button destroyButton;
    //[SerializeField] private Button focusButton;

    private System.Action tickActions;
    private System.Action onCloseActions;

    private System.Action displayAction;
    private System.Action destroyAction;
    private System.Action focusAction;

    private string currentTabName = "Default";
    private Dictionary<string, GameObject> tabs = new Dictionary<string, GameObject> ();

    private List<KeyValueUIPair> keyValueUIPairs = new List<KeyValueUIPair> ();

    public void ShowPanel (GameObject target)
    {
        this.target = target;
        header_Label.text = "<b>" + target?.GetComponent<WorldEntity> ()?.Type.ToString() + "</b>" + " - " + target?.GetComponent<WorldEntity> ()?.EntityName;
        FindObjectOfType<HUD_EntityInspection_Panel> ().ShowPanel ( HUD_EntityInspection_Panel.InspectionType.Citizen );
    }   

    public override void Show ()
    {
        base.Show ();
        ClearPanel ();
        GameTime.RegisterGameTick ( Tick );
    }

    protected virtual void Tick(int relativeTick)
    {
        Tick_ValidityCheck ();
    }

    protected override void SetAnchoredPosition ()
    {        
        if (target == null) return;

        Vector3 citizenScreenPosition = Camera.main.WorldToScreenPoint ( target.transform.position );
        
        base.targetAnchorOffset = new Vector3 ( 32.0f, -32.0f, 0.0f );
        base.targetAnchoredPosition = citizenScreenPosition;
    }

    public void AddTab (string tabName)
    {
        if (tabs.ContainsKey ( tabName ) || tabName == "Any") return;
        GameObject go = UIController.Instance.SpawnUI ( UIController.UIEnumValue.Type.Tab, tabView );
        go.GetComponentInChildren<Text> ().text = tabName;
        go.GetComponentInChildren<Button> ().onClick.AddListener ( () => { SetActiveTab ( tabName ); } );
        tabs.Add ( tabName, go );
    }

    private void ClearTabs ()
    {
        GameObject[] gos = tabs.Values.ToArray();

        for (int i = 0; i < gos.Length; i++)
        {
            Destroy ( gos[i] );
        }

        tabs.Clear ();
    }

    public void SetActiveTab(string tabName)
    {
        currentTabName = tabName;
        ClearPanel ();
        displayAction?.Invoke ();
    }

    public void SetAction(System.Action displayAction, System.Action destroyAction, System.Action focusAction, string destroyDescription)
    {
        this.displayAction = displayAction;
        this.destroyAction = destroyAction;
        this.focusAction = focusAction;

        if (!string.IsNullOrEmpty ( destroyDescription ))
            transform.Find ( "Header_Panel" ).Find ( "Delete_Button" ).GetComponent<Tooltip> ().SetTooltip ( destroyDescription, HUD_Tooltip_Panel.Tooltip.Preset.Information );
    }

    public void AddTextData(System.Func<KeyValueUIPair, string> action, string key, string tabName)
    {
        AddTab ( tabName );

        if (currentTabName == tabName || tabName == "Any")
        {
            KeyValueUIPair pair = UIController.Instance.SpawnUI ( UIController.UIEnumValue.Type.KeyValueText, leftHorizontalView ).GetComponent<KeyValueUIPair> ();
            pair.SetData ( action, key );
            keyValueUIPairs.Add ( pair );
        }
    }

    public void AddButtonData(System.Action action, string key, string tabName)
    {
        AddTab ( tabName );

        if (currentTabName == tabName || tabName == "Any")
        {
            GameObject go = UIController.Instance.SpawnUI ( UIController.UIEnumValue.Type.LongButton, rightHorizontalView );
            go.GetComponent<Button> ().onClick.AddListener ( () => { action (); } );
            go.GetComponentInChildren<Text> ().text = key;
        }
    }

    public void AddButtonTextData (System.Action clickAction, System.Func<KeyValueUIPair, string> textAction, string key, string tabName)
    {
        AddTab ( tabName );

        if (currentTabName == tabName || tabName == "Any")
        {
            GameObject go = UIController.Instance.SpawnUI ( UIController.UIEnumValue.Type.KeyValueInput, leftHorizontalView );
            go.GetComponent<KeyValueUIPair> ().SetData ( textAction, key );
            go.GetComponentInChildren<Button> ().onClick.AddListener ( () => { clickAction (); } );
        }
    }

    public void AddDropdownData (System.Action<int, List<Dropdown.OptionData>> valueChanged, System.Func<KeyValueUIPair, string> textAction, string key, string tabName, params string[] options)
    {
        AddTab ( tabName );

        if (currentTabName == tabName || tabName == "Any")
        {
            GameObject go = UIController.Instance.SpawnUI ( UIController.UIEnumValue.Type.Dropdown, leftHorizontalView );
            go.GetComponent<KeyValueUIPair> ().SetType ( KeyValueUIPair.Type.Dropdown );
            go.GetComponent<KeyValueUIPair> ().SetData ( textAction, key );
            Dropdown dropdown = go.GetComponentInChildren<Dropdown> ();
            dropdown.options.Clear ();

            if (options.Length <= 1) Debug.Log ( "Too few options!" );

            for (int i = 0; i < options.Length; i++)
            {
                dropdown.options.Add ( new Dropdown.OptionData ( options[i] ) );
            }

            dropdown.onValueChanged.RemoveAllListeners ();
            dropdown.onValueChanged.AddListener (  (index) => { valueChanged ( index, dropdown.options); } );
        }
    }

    public void AddTickActionData(System.Action action)
    {
        if (action != null) tickActions += action;
    }

    public void AddOnCloseAction(System.Action action)
    {
        if (action != null) onCloseActions += action;
    }

    private void ClearPanel ()
    {
        for (int i = 0; i < leftHorizontalView.childCount; i++)
        {
            Destroy ( leftHorizontalView.GetChild ( i ).gameObject );
        }

        for (int i = 0; i < rightHorizontalView.childCount; i++)
        {
            Destroy ( rightHorizontalView.GetChild ( i ).gameObject );
        }

        ClearTabs ();
    }

    private void Tick_ValidityCheck ()
    {
        if (target == null) { ClearPanel (); Hide (); return; }
        if (tickActions != null) tickActions ();
    }

    public void OnClick_Delete ()
    {
        destroyAction?.Invoke ();
    }

    public void OnClick_Focus ()
    {
        focusAction?.Invoke ();
    }

    public override void Hide ()
    {
        base.Hide ();
        FindObjectOfType<HUD_EntityInspection_Panel> ().Hide ();

        displayAction = null;

        if (onCloseActions != null) onCloseActions ();
        onCloseActions = null;

        tickActions = null;
        GameTime.UnRegisterGameTick ( Tick );
    }
}
