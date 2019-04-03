using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Preferences_Panel : UIPanel {

    [Header("Display Tab")]
    [SerializeField] private GameObject togglePrefab;
    [SerializeField] private GameObject valuePrefab;
    [SerializeField] private Transform displayPanel;
    [SerializeField] private Transform notificationsPanel;
    [SerializeField] private Transform conditionsPanel;
    [SerializeField] private Transform hotkeysPanel;

    private List<ToggleElement> toggleElements = new List<ToggleElement> ();
    private List<ConditionElement> startingConditionElements = new List<ConditionElement> ();

    struct ToggleElement
    {
        public GameObject go;
        public System.Action update;

        public ToggleElement (GameObject go, Action update)
        {
            this.go = go;
            this.update = update;
        }
    }

    struct ConditionElement
    {
        public GameObject go;
        public System.Action<TMP_Text> update;

        public ConditionElement (GameObject go, Action<TMP_Text> update)
        {
            this.go = go;
            this.update = update;
        }
    }

    protected override void Start ()
    {
        base.Start ();

        CreateToggleElements ();
        CreateStartingConditionsElements ();
        CreateHotkeys ();
    }

    private void CreateToggleElements ()
    {
        CreateToggleElement ( "Show Citizen Icons", displayPanel, (b) => { GamePreferences.Instance.ShowCitizenIcons ( b ); }, (t) => { t.isOn = GamePreferences.Instance.preferences.showCitizenIcons; } );
        CreateToggleElement ( "Show Prop Icons", displayPanel, (b) => { GamePreferences.Instance.ShowPropIcons ( b ); }, (t) => { t.isOn = GamePreferences.Instance.preferences.showPropIcons; } );
        CreateToggleElement ( "Show Inventory Popups", displayPanel, (b) => { GamePreferences.Instance.ShowInventoryPopups ( b ); }, (t) => { t.isOn = GamePreferences.Instance.preferences.showInventoryPopups; } );

        CreateToggleElement ( "Show Birthday Notifications", notificationsPanel, (b) => { GamePreferences.Instance.showNotification_Birthday ( b ); }, (t) => { t.isOn = GamePreferences.Instance.preferences.showNotification_Birthday; } );
        CreateToggleElement ( "Show Special Birthday Notifications", notificationsPanel, (b) => { GamePreferences.Instance.showNotification_SpecialBirthday ( b ); }, (t) => { t.isOn = GamePreferences.Instance.preferences.showNotification_SpecialBirthday; } );
        CreateToggleElement ( "Show Pregnancy Notifications", notificationsPanel, (b) => { GamePreferences.Instance.showNotification_Pregnancy ( b ); }, (t) => { t.isOn = GamePreferences.Instance.preferences.showNotification_Pregnancy; } );
        CreateToggleElement ( "Show Birth Notifications", notificationsPanel, (b) => { GamePreferences.Instance.showNotification_Birth ( b ); }, (t) => { t.isOn = GamePreferences.Instance.preferences.showNotification_Birth; } );
    }

    private void CreateStartingConditionsElements ()
    {
        CreateStartingConditionElement ( "Game Difficulty", conditionsPanel, GameData.Instance.startingConditions.identifier, (obj) => { obj.text = GameData.Instance.startingConditions.identifier; } );
        CreateStartingConditionElement ( "Starting Money", conditionsPanel, MoneyController.BeautifyMoney ( GameData.Instance.startingConditions.startingMoney ), (obj) => { obj.text = MoneyController.BeautifyMoney ( GameData.Instance.startingConditions.startingMoney ); } );
        CreateStartingConditionElement ( "Starting Season", conditionsPanel, GameData.Instance.startingConditions.startingSeason.ToString (), (obj) => { obj.text = GameData.Instance.startingConditions.startingSeason.ToString (); } );
        CreateStartingConditionElement ( "Starting Resources", conditionsPanel, GameData.Instance.startingConditions.startingResourcesRichness.ToString (), (obj) => { obj.text = GameData.Instance.startingConditions.startingResourcesRichness.ToString (); } );
        CreateStartingConditionElement ( "Starting Families", conditionsPanel, GameData.Instance.startingConditions.startingFamilies.ToString ( "0" ), (obj) => { obj.text = GameData.Instance.startingConditions.startingFamilies.ToString ( "0" ); } );

        CreateStartingConditionElement ( "Max Children", conditionsPanel, GameData.Instance.startingConditions.maximumChildren.ToString ( "0" ), (obj) => { obj.text = GameData.Instance.startingConditions.maximumChildren.ToString ( "0" ); } );
        CreateStartingConditionElement ( "Fertility Rate", conditionsPanel, GameData.Instance.startingConditions.fertilityModifier.ToString ( "0.0" ) + "x", (obj) => { obj.text = GameData.Instance.startingConditions.fertilityModifier.ToString ( "0.0" ) + "x"; } );
        CreateStartingConditionElement ( "Mortality Rate", conditionsPanel, GameData.Instance.startingConditions.mortalityRate.ToString ( "0.0" ) + "x", (obj) => { obj.text = GameData.Instance.startingConditions.mortalityRate.ToString ( "0.0" ) + "x"; } );
        CreateStartingConditionElement ( "Pilgrim Rate", conditionsPanel, GameData.Instance.startingConditions.pilgrimChance.ToString ( "0.0" ) + "x", (obj) => { obj.text = GameData.Instance.startingConditions.pilgrimChance.ToString ( "0.0" ) + "x"; } );
    }

    private void CreateHotkeys ()
    {
        foreach (KeyValuePair<Hotkey.Function, Hotkey.HotkeyData> item in Hotkey.hotkeys)
        {
            CreateHotkey ( item.Value.Description, hotkeysPanel, item.Value.GetCommandString () );
        }
    }

    private void CreateToggleElement (string text, Transform panel, System.Action<bool> action, System.Action<Toggle> updateAction, bool interactive = true)
    {
        GameObject go = Instantiate ( togglePrefab );
        go.transform.SetParent ( panel );

        go.GetComponentInChildren<TextMeshProUGUI> ().text = text;

        Toggle t = go.GetComponentInChildren<Toggle> ();
        t.onValueChanged.AddListener ( (b) => { action ( b ); } );
        t.interactable = interactive;

        toggleElements.Add ( new ToggleElement ( go, () => { updateAction ( t ); } ) );
    }

    private void CreateStartingConditionElement (string text, Transform panel, string value, System.Action<TMP_Text> updateAction)
    {
        GameObject go = Instantiate ( valuePrefab );
        go.transform.SetParent ( panel );

        go.GetComponentsInChildren<TextMeshProUGUI> ()[0].text = value;
        go.GetComponentsInChildren<TextMeshProUGUI> ()[1].text = text;

        startingConditionElements.Add ( new ConditionElement ( go, updateAction ) );
    }

    private void CreateHotkey(string text, Transform panel, string value)
    {
        GameObject go = Instantiate ( valuePrefab );
        go.transform.SetParent ( panel );

        go.GetComponentsInChildren<TextMeshProUGUI> ()[0].text = value;
        go.GetComponentsInChildren<TextMeshProUGUI> ()[1].text = text;
    }

    public override void Show ()
    {
        base.Show ();

        UpdateUI ();
    }

    private void UpdateUI ()
    {
        for (int i = 0; i < toggleElements.Count; i++)
        {
            toggleElements[i].update?.Invoke ();
        }

        for (int i = 0; i < startingConditionElements.Count; i++)
        {
            startingConditionElements[i].update?.Invoke ( startingConditionElements[i].go.GetComponentsInChildren<TextMeshProUGUI> ()[0] );
        }
    }

    public override void Hide ()
    {
        base.Hide ();
    }

}
