using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIPanel : MonoBehaviour {

    [Header ( "Panel Group" )]
    [SerializeField] protected int groupID;
    [SerializeField] protected bool isDefault;
    public KeyValuePair<int, bool> GroupDetails { get { return new KeyValuePair<int, bool> ( groupID, isDefault ); } }

    [Header("Options")]
    [SerializeField] private bool showOnAwake;
    [SerializeField] private bool blockRaycasts;

    //protected PanelGroup panelGroup;
    protected CanvasGroup cGroup;
    protected bool active = false;

    protected virtual void Start ()
    {
        cGroup = GetComponent<CanvasGroup> ();
        //panelGroup = GetComponentInParent<PanelGroup> ();
        if (showOnAwake) Show (); else Hide ();
    }

	public virtual void Show ()
    {        
        active = true;
        if (cGroup == null) { Debug.LogError ( "Error getting canvas group", this ); return; }
        cGroup.alpha = 1;

        if (blockRaycasts)
            cGroup.blocksRaycasts = true;
        else cGroup.blocksRaycasts = false;


        //panelGroup.SetActivePanel ( this );
    }

    public virtual void Toggle ()
    {
        active = !active;

        if (active) Show (); else Hide ();
    }

    public virtual void Hide ()
    {
        //Debug.Log ( gameObject.name );
        active = false;
        cGroup.alpha = 0;
        cGroup.blocksRaycasts = false;

        //if (!isDefault)
        //    panelGroup.RemoveActivePanel ( this );
        //return;
    }
}
