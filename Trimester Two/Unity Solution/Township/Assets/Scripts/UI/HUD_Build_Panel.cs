using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Build_Panel : UIPanel {

    [SerializeField] private GameObject propOptionPrefab;
    //private BuildMode buildMode;
    private HUD_Toolbar_Panel toolbarPanel;
    private PropManager propManager;
    private Transform optionsContent;

    protected override void Start ()
    {
        //buildMode = FindObjectOfType<BuildMode> ();
        propManager = FindObjectOfType<PropManager> ();
        optionsContent = transform.Find ( "Content_Blurred" ).Find( "Content_Tint" ).Find ( "ScrollRect" ).Find ( "Options" );
        toolbarPanel = FindObjectOfType<HUD_Toolbar_Panel> ();
        base.Start ();
    }

    public override void Show ()
    {
        base.Show ();
        //buildMode.SetActive ( true );

        toolbarPanel.Hide ();
    }

    public override void Toggle ()
    {
        base.Toggle ();
    }

    public override void Hide ()
    {
        base.Hide ();

        toolbarPanel.Show ();
    }

    public void SetCategory(string c)
    {
        if(c == "All")
        {
            List<PropData> propAll = PropManager.Instance.propData;
            BuildMode.Instance.SetPropData ( propAll[0] );
            CreateOptions ( propAll );
            return;
        }

        List<PropData> props = propManager.GetPropsByCategory ( EnumCollection.propCategory[c] );

        if(props == null) { Debug.LogError ( "ERROR" ); return; }
        if (props.Count <= 0) { Debug.LogError ( "Props for category " + c + " are 0" ); return; }

        BuildMode.Instance.SetPropData ( props[0] );
        CreateOptions ( props );
    }

    private void CreateOptions (List<PropData> props)
    {
        DestroyChildren ( optionsContent );

        for (int i = 0; i < props.Count; i++)
        {
            PropData pd = props[i];
            GameObject go = Instantiate ( propOptionPrefab );
            go.transform.SetParent ( optionsContent );
            go.GetComponentsInChildren<Image> ()[2].sprite = props[i].buildModeSprite;
            go.GetComponentInChildren<Text> ().text = props[i].name;
            go.GetComponent<Button> ().onClick.AddListener ( () => { FindObjectOfType<BuildMode> ().SetPropData ( pd ); } );
        }
    }

    private void DestroyChildren(Transform t)
    {
        int c = t.childCount;

        for (int i = 0; i < c; i++)
        {
            Destroy ( t.GetChild ( i ).gameObject );
        }
    }
}
