using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CitizenBase : MonoBehaviour {

    public CitizenMovement CitizenMovement { get; protected set; }
    public CitizenGraphics CitizenGraphics { get; protected set; }
    public CitizenJob CitizenJob { get; protected set; }
    public CitizenFamily CitizenFamily { get; protected set; }
    public CitizenHousing CitizenHousing { get; protected set; }

    private NavMeshAgent agent { get; set; }
    public ResourceInventory Inventory { get; protected set; }
    
    public bool MoveToRandomLocation { get; set; }

    public System.Action<CitizenBase> OnCitizenDied;

    private void Awake ()
    {
        CitizenMovement = GetComponent<CitizenMovement> ();
        CitizenGraphics = GetComponent<CitizenGraphics> ();
        CitizenJob = GetComponent<CitizenJob> ();
        CitizenFamily = GetComponent<CitizenFamily> ();
        CitizenHousing = GetComponent<CitizenHousing> ();

        Inventory = new ResourceInventory ();
        agent = GetComponent<NavMeshAgent> ();
    }

    void Start ()
    {
        GameTime.RegisterGameTick ( OnGameTick );

        MoveToRandomLocation = false;

        SetInspection ();
    }

    private void OnGameTick (int relativeTick)
    {        
        Tick_CheckRandomMovement ();
    }

    private void Tick_CheckRandomMovement ()
    {
        if (!MoveToRandomLocation) return;
        if (agent == null) return;
        if (!agent.isOnNavMesh) return;

        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    CitizenJob.ClearPreviousJobs ();
                    MoveToRandomLocation = false;
                }
            }
        }
    }

    private void SetInspection ()
    {
        GetComponent<Inspectable> ().SetAdditiveAction ( () =>
        {
            HUD_EntityInspection_Citizen_Panel panel = FindObjectOfType<HUD_EntityInspection_Citizen_Panel> ();

            panel.AddButtonData ( () =>
            {
                if (this == null) return;

                FindObjectOfType<CameraMovement> ().LockTo ( this.transform );

            }, "Follow Citizen", "Any" );

            panel.AddButtonData ( () =>
            {
                if (this == null) return;

                if (this.CitizenJob.GetCurrentJob == null) return;
                else this.CitizenJob.GetCurrentJob.OnCharacterLeave ( "User Left" );
            }, "Cancel Job", "Any" );

            panel.AddButtonData ( () =>
            {
                if (this == null) return;

                KillCitizen ();

            }, "Kill Citizen", "Any" );

            panel.AddTextData ( () =>
            {
                if (this == null) { return "None"; }

                if (this.CitizenJob.GetCurrentJob == null) return "None";
                else return this.CitizenJob.GetCurrentJob.Name;

            }, "Job", "Overview" );

            panel.AddTextData ( () =>
            {
                if (this == null) return "Idling";

                if (this.CitizenJob.GetCurrentJob == null) return "Idling";
                else return this.CitizenJob.GetCurrentJob.AgentJobStatus;

            }, "Status", "Overview" );           

            panel.AddTickActionData ( () =>
            {
                if (this == null) return;

                LineRenderer lr = this.GetComponentInChildren<LineRenderer> ();

                if (this.CitizenMovement.HasPath)
                {
                    NavMeshPath path = this.CitizenMovement.GetPath;
                    lr.positionCount = path.corners.Length;

                    for (int i = 0; i < path.corners.Length; i++)
                    {
                        lr.SetPosition ( i, path.corners[i] + new Vector3 ( 0.0f, 0.2f + (SnowController.Instance.TerrainSnowLevel / SnowController.Instance.MaxSnowDepth), 0.0f ) );
                    }

                }
                else lr.positionCount = 0;                
            } );

            panel.AddOnCloseAction ( () =>
            {
                if (this == null) return;

                LineRenderer lr = this.GetComponentInChildren<LineRenderer> ();
                if (lr != null) lr.positionCount = 0;
            } );
        } );
    }

    private void KillCitizen ()
    {
        if (OnCitizenDied != null) OnCitizenDied (this);
        if (this.CitizenJob.GetCurrentJob != null) this.CitizenJob.GetCurrentJob.OnCharacterLeave ( "Citizen Died" );
        Destroy ( this.gameObject );
    }

    private void OnDestroy ()
    {
        GameTime.UnRegisterGameTick ( OnGameTick );
    }
}