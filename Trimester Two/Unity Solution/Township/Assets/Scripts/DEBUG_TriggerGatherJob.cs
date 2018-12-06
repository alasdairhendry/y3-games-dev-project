using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DEBUG_TriggerGatherJob : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
     
	}

    private void Show_Citizen (GameObject target)
    {
        
    }

    private void Show_RawMaterial (GameObject target)
    {
        //HUD_EntityInspection_Citizen_Panel panel = FindObjectOfType<HUD_EntityInspection_Citizen_Panel> ();
        //RawMaterial rawMaterial = target.GetComponentInParent<RawMaterial> ();

        //panel.ShowPanel ( target );

        //panel.AddButtonData ( () =>
        //{
        //    if (rawMaterial == null) return;

        //    rawMaterial.CreateRemovalJob ();
        //    panel.Hide ();

        //}, "Cut Down" );
    }
}
