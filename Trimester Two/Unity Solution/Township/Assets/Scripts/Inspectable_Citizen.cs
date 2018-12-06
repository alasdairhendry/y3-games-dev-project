using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inspectable_Citizen : Inspectable {

    private Character citizen;
	
	protected override void Start () {
        citizen = GetComponent<Character> ();

        base.action = () =>
        {
            HUD_EntityInspection_Citizen_Panel panel = FindObjectOfType<HUD_EntityInspection_Citizen_Panel> ();            
            //panel.ShowPanel ( citizen.gameObject );

            panel.AddTextData ( () =>
            {
                if (citizen == null) { Debug.Log ( "Citizen == null" ); return "None"; }

                if (citizen.GetCurrentJob == null) return "None";
                else return citizen.GetCurrentJob.Name;

            }, "Job" );

            panel.AddTextData ( () =>
            {
                if (citizen == null) return "Idling";

                if (citizen.GetCurrentJob == null) return "Idling";
                else return citizen.GetCurrentJob.AgentJobStatus;

            }, "Status" );

            panel.AddButtonData ( () =>
            {
                if (citizen == null) return;

                if (citizen.GetCurrentJob == null) return;
                else citizen.GetCurrentJob.OnCharacterLeave ( "User Left" );
            }, "Cancel Job" );

            panel.AddButtonData ( () =>
            {
                if (citizen == null) return;

                if (citizen.GetCurrentJob != null) citizen.GetCurrentJob.OnCharacterLeave ( "Citizen Died" );

                Destroy ( citizen.gameObject );

            }, "Kill Citizen" );

            panel.AddButtonData ( () =>
            {
                if (citizen == null) return;

                FindObjectOfType<CameraMovement> ().LockTo ( citizen.transform );

            }, "Follow Citizen" );
        };
	}
	
}
