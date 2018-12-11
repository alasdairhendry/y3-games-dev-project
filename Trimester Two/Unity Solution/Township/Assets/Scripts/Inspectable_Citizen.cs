using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Inspectable_Citizen : Inspectable {

    private Character citizen;
    [SerializeField] private float groundOffset = 0.2f;
	
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

            panel.AddTickActionData ( () =>
              {
                  if (citizen == null) return;

                  LineRenderer lr = citizen.GetComponentInChildren<LineRenderer> ();

                  //if(citizen.CharacterMovement.DestinationObject == null)
                  //{
                  //    lr.positionCount = 0;
                  //    return;
                  //}

                  if (citizen.CharacterMovement.HasPath)
                  {
                      NavMeshPath path = citizen.CharacterMovement.GetPath;
                      lr.positionCount = path.corners.Length;

                      for (int i = 0; i < path.corners.Length; i++)
                      {
                          lr.SetPosition ( i, path.corners[i] + new Vector3 ( 0.0f, groundOffset + (SnowController.Instance.TerrainSnowLevel / SnowController.Instance.MaxSnowDepth), 0.0f ) );
                      }

                  }
                  else lr.positionCount = 0;

                  //if (citizen == null) return;

                  ////if(citizen.CharacterMovement.DestinationObject == null)
                  ////{
                  ////    FindObjectOfType<FootstepCreator> ().Clear ();
                  ////    return;
                  ////}

                  //if (citizen.CharacterMovement.HasPath)
                  //{
                  //    NavMeshPath path = citizen.CharacterMovement.GetPath;
                  //    FindObjectOfType<FootstepCreator> ().Create ( path.corners );

                  //}
                  //else FindObjectOfType<FootstepCreator> ().Clear ();

              } );
        };
	}
	
}
