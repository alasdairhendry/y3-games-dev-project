using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Build_Panel : UIPanel {

    [SerializeField] private GameObject propOptionPrefab;
    [SerializeField] private Text header;
    //private BuildMode buildMode;
    private HUD_Toolbar_Panel toolbarPanel;
    //private PropManager propManager;
    private Transform optionsContent;
    private string targetCategory = "";
    private int targetOverallProp = 0;
    public bool bypassUnlocks = false;

    protected override void Start ()
    {
        //buildMode = FindObjectOfType<BuildMode> ();
        //propManager = FindObjectOfType<PropManager> ();
        optionsContent = transform.Find ( "Content_Blurred" ).Find( "Content_Tint" ).Find ( "ScrollRect" ).Find ( "Options" );
        toolbarPanel = FindObjectOfType<HUD_Toolbar_Panel> ();
        base.Start ();
    }

    public void Refresh ()
    {
        //if (string.IsNullOrEmpty ( targetCategory )) return;
        //SetCategory ( category, targetOverallProp );
        DisplayAllTarget ( targetOverallProp );
        //if (targetCategory == "All") DisplayAll ();
        //else DisplayCategory ( (int)(PropCategory)System.Enum.Parse ( typeof ( PropCategory ), targetCategory ) );
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

    private void DisplayAllTarget(int target)
    {
        List<PropData> propsOfCategory = PropManager.Instance.propData;

        if(propsOfCategory[target].locked && !bypassUnlocks)
        {
            DisplayAll ();
            Debug.LogError ( "Couldnt get this target" );
            return;
        }
        else
        {
            CreateOptions ( propsOfCategory, "All" );
            SetPropData ( target );
        }
    }

    public void DisplayAll ()
    {
        List<PropData> propsOfCategory = PropManager.Instance.propData;

        int unlockedIndex = -1;

        for (int i = 0; i < propsOfCategory.Count; i++)
        {
            if (propsOfCategory[i].locked && !bypassUnlocks) continue;

            unlockedIndex = i;
            break;
        }

        if (unlockedIndex == -1)
        {
            Debug.LogError ( "No props unlocked." );
            return;
        }
        else
        {
            CreateOptions ( propsOfCategory, "All" );
            SetPropData ( unlockedIndex );
        }
    }

    public void DisplayCategory (int categoryIndex)
    {
        PropCategory category = (PropCategory)categoryIndex;
        List<PropData> propsOfCategory = PropManager.Instance.GetPropsByCategory ( category );

        int unlockedIndex = -1;

        for (int i = 0; i < propsOfCategory.Count; i++)
        {
            if (propsOfCategory[i].locked && !bypassUnlocks) continue;

            unlockedIndex = i;
            break;
        }

        if(unlockedIndex == -1)
        {
            DisplayAll ();
            return;
        }
        else
        {
            CreateOptions ( propsOfCategory, category.ToString () );
            SetPropData ( PropManager.Instance.propData.IndexOf ( propsOfCategory[unlockedIndex] ) );
        }
    }

    //public void SetCategory(string c, int index)
    //{
    //    category = c;

    //    if(c == "All")
    //    {
    //        List<PropData> propAll = PropManager.Instance.propData;
    //        //BuildMode.Instance.SetPropData ( propAll[0] );
    //        CreateOptions ( propAll, "All", 0 );
    //        return;
    //    }

    //    List<PropData> props = propManager.GetPropsByCategory ( EnumCollection.propCategory[c] );

    //    if(props == null) { Debug.LogError ( "ERROR" ); return; }
    //    if (props.Count <= 0) { Debug.LogError ( "Props for category " + c + " are 0" ); return; }

    //    //BuildMode.Instance.SetPropData ( props[0] );
    //    CreateOptions ( props, c, 0 );
    //}

    private void CreateOptions (List<PropData> props, string header)
    {
        targetCategory = header;

        this.header.text = header;
        DestroyChildren ( optionsContent );

        for (int i = 0; i < props.Count; i++)
        {
            if (props[i].locked && !bypassUnlocks) continue;

            //givenPropsLocked = false;
            PropData pd = props[i];
            GameObject go = Instantiate ( propOptionPrefab );
            go.transform.SetParent ( optionsContent );
            go.GetComponentsInChildren<Image> ()[2].sprite = props[i].buildModeSprite;
            Text[] texts = go.GetComponentsInChildren<Text> ();
            texts[0].text = props[i].name;
            texts[1].text = props[i].description;
            go.GetComponent<Button> ().onClick.AddListener ( () => { SetPropData ( PropManager.Instance.propData.IndexOf ( pd ) ); /*FindObjectOfType<BuildMode> ().SetPropData ( pd ); */} );
        }

        //bool givenPropsLocked = true;

        //if (props[target].locked && !bypassUnlocks)
        //{
        //    givenPropsLocked = true;
        //}
        //else
        //{

        //}

        //if (givenPropsLocked)
        //{
        //    bool allLocked = true;
        //    int index = 0;

        //    for (int i = 0; i < PropManager.Instance.propData.Count; i++)
        //    {
        //        if (PropManager.Instance.propData[i].locked == false)
        //        {
        //            allLocked = false;
        //            index = i;
        //            break;
        //        }
        //    }

        //    if (allLocked)
        //    {
        //        Debug.LogError ( "All props are locked. This is an issue." );
        //    }
        //    else
        //    {
        //        //BuildMode.Instance.SetPropData ( PropManager.Instance.propData[index] );
        //        SetPropData ( index );
        //        CreateOptions ( PropManager.Instance.propData, "All", index );
        //    }
        //}
        //else
        //{
        //    SetPropData ( PropManager.Instance.propData.IndexOf ( props[target] ) );
        //    //BuildMode.Instance.SetPropData ( props[target] );
        //}
    }

    private void SetPropData(int overallIndex)
    {
        targetOverallProp = overallIndex;
        BuildMode.Instance.SetPropData ( PropManager.Instance.propData[overallIndex] );
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
