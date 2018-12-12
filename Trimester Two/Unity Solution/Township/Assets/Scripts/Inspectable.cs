using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inspectable : MonoBehaviour {

    protected System.Action action;
    [SerializeField] protected string defaultTab = "Overview";    

    protected virtual void Start () { }

	public virtual void Inspect ()
    {
        if (action == null) return;
        FindObjectOfType<HUD_EntityInspection_Citizen_Panel> ().ShowPanel ( this.gameObject );
        FindObjectOfType<HUD_EntityInspection_Citizen_Panel> ().SetAction ( action );        
        FindObjectOfType<HUD_EntityInspection_Citizen_Panel> ().SetActiveTab ( defaultTab );
    }

    public void SetAction (System.Action action)
    {
        if (action != null) this.action = action;
    }

    public void SetAdditiveAction (System.Action action)
    {
        if (action != null) this.action += action;
    }
}
