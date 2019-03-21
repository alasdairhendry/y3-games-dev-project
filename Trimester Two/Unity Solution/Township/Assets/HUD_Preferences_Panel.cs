using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Preferences_Panel : UIPanel {

    [Header("Display Tab")]
    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform displayPanel;

    private List<Element> elements = new List<Element> ();

    struct Element
    {
        public GameObject go;
        public System.Action update;

        public Element (GameObject go, Action update)
        {
            this.go = go;
            this.update = update;
        }
    }

    protected override void Start ()
    {
        base.Start ();

        CreateElements ();
    }

    private void CreateElements ()
    {
        CreateElement ( "Show Citizen Icons", displayPanel, (b) => { GamePreferences.Instance.ShowCitizenIcons ( b ); }, (t) => { t.isOn = GamePreferences.Instance.preferences.showCitizenIcons; } );
        CreateElement ( "Show Prop Icons", displayPanel, (b) => { GamePreferences.Instance.ShowPropIcons ( b ); }, (t) => { t.isOn = GamePreferences.Instance.preferences.showPropIcons; } );
        CreateElement ( "Show Inventory Popups", displayPanel, (b) => { GamePreferences.Instance.ShowInventoryPopups ( b ); }, (t) => { t.isOn = GamePreferences.Instance.preferences.showInventoryPopups; } );
    }

    private void CreateElement (string text, Transform panel, System.Action<bool> action, System.Action<Toggle> updateAction)
    {
        GameObject go = Instantiate ( prefab );
        go.transform.SetParent ( panel );

        go.GetComponentInChildren<Text> ().text = text;

        Toggle t = go.GetComponentInChildren<Toggle> ();
        t.onValueChanged.AddListener ( (b) => { action ( b ); } );

        elements.Add ( new Element ( go, () => { updateAction ( t ); } ) );
    }

    public override void Show ()
    {
        Debug.Log ( "Sow()" );
        base.Show ();

        UpdateUI ();
    }

    private void UpdateUI ()
    {
        for (int i = 0; i < elements.Count; i++)
        {
            elements[i].update?.Invoke ();
        }
    }

    public override void Hide ()
    {
        base.Hide ();
    }

}
