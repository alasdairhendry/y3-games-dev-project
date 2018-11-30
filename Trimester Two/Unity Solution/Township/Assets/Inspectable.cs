using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inspectable : MonoBehaviour {

    protected System.Action action;

    protected virtual void Start () { }

	public virtual void Inspect ()
    {
        if (action == null) return;
        FindObjectOfType<HUD_EntityInspection_Citizen_Panel> ().ShowPanel ( this.gameObject );
        action ();
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
