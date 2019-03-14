using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CitizenBase : MonoBehaviour {

    public int ID { get; protected set; }
    public CitizenMovement CitizenMovement { get; protected set; }
    public CitizenAnimation CitizenAnimation { get; protected set; }
    public CitizenGraphics CitizenGraphics { get; protected set; }
    public CitizenJob CitizenJob { get; protected set; }
    public CitizenIdleJob CitizenIdleJob { get; protected set; }
    public CitizenNeeds CitizenNeeds { get; protected set; }
    public CitizenAge CitizenAge { get; protected set; }
    public CitizenFamily CitizenFamily { get; protected set; }
    public CitizenHousing CitizenHousing { get; protected set; }

    private NavMeshAgent agent { get; set; }
    public ResourceInventory Inventory { get; protected set; }

    [SerializeField] private LayerMask layerMask;
    
    public System.Action<CitizenBase> OnCitizenDied;

    private void Awake ()
    {
        CitizenMovement = GetComponent<CitizenMovement> ();
        CitizenAnimation = GetComponent<CitizenAnimation> ();
        CitizenGraphics = GetComponent<CitizenGraphics> ();
        CitizenJob = GetComponent<CitizenJob> ();
        CitizenIdleJob = GetComponent<CitizenIdleJob> ();
        CitizenNeeds = GetComponent<CitizenNeeds> ();
        CitizenAge = GetComponent<CitizenAge> ();
        CitizenFamily = GetComponent<CitizenFamily> ();
        CitizenHousing = GetComponent<CitizenHousing> ();

        Inventory = new ResourceInventory ();
        agent = GetComponent<NavMeshAgent> ();
        EntityManager.Instance.OnEntityCreated ( this.gameObject, this.GetType () );
    }

    private void Start ()
    {
        SetInspection ();
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
                else this.CitizenJob.GetCurrentJob.OnCharacterLeave ( "User Left", true );
            }, "Cancel Job", "Any" );

            panel.AddButtonData ( () =>
            {
                if (this == null) return;

                KillCitizen ();

            }, "Kill Citizen", "Any" );

            panel.AddTextData ( (pair) =>
            {
                
                if (this == null) { return "None"; }

                if (this.CitizenJob.GetCurrentJob == null) return "None";
                else return this.CitizenJob.GetCurrentJob.Name;

            }, "Job", "Overview" );

            panel.AddTextData ( (pair) =>
            {
                if (this == null) return "Idling";

                if (this.CitizenJob.GetCurrentJob == null) return "Idling";
                else return this.CitizenJob.GetCurrentJob.AgentJobStatus;

            }, "Status", "Overview" );

            panel.AddTextData ( (pair) =>
            {
                if (this == null) return "-1.0f";
                if (this.CitizenNeeds == null) return "-1.0";
                return CitizenNeeds.Needs[0].currentValue.ToString ( "0.00" );

            }, CitizenNeeds.Needs[0].type.ToString (), "Needs" );

            panel.AddTextData ( (pair) =>
            {
                if (this == null) return "-1.0f";
                if (this.CitizenNeeds == null) return "-1.0";
                return CitizenNeeds.Needs[1].currentValue.ToString ( "0.00" );

            }, CitizenNeeds.Needs[1].type.ToString (), "Needs" );

            panel.AddTextData ( (pair) =>
            {
                if (this == null) return "-1.0f";
                if (this.CitizenNeeds == null) return "-1.0";
                return CitizenNeeds.Needs[2].currentValue.ToString ( "0.00" );

            }, CitizenNeeds.Needs[2].type.ToString (), "Needs" );

            panel.AddTextData ( (pair) =>
            {
                if (this == null) return "-1.0f";
                if (this.CitizenNeeds == null) return "-1.0";
                return CitizenNeeds.Needs[3].currentValue.ToString ( "0.00" );

            }, CitizenNeeds.Needs[3].type.ToString (), "Needs" );


            panel.AddTickActionData ( () =>
            {
                if (this == null) return;

                LineRenderer lr = this.GetComponentInChildren<LineRenderer> ();

                if (this.CitizenMovement.HasPath)
                {
                    NavMeshPath path = this.CitizenMovement.GetAgentPath;
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

    public void SetID(int id)
    {
        this.ID = id;
    }

    private void KillCitizen ()
    {
        if (OnCitizenDied != null) OnCitizenDied (this);
        if (this.CitizenJob.GetCurrentJob != null) this.CitizenJob.GetCurrentJob.OnCharacterLeave ( "Citizen Died", true );
        Destroy ( this.gameObject );
    }

    public KeyValuePair<bool, Vector3> GetNavMeshPoint(float x, float z)
    {
        Ray ray = new Ray ( new Vector3(x, 150.0f, z), Vector3.down );
        RaycastHit hit;

        if (Physics.Raycast ( ray, out hit, 1000, layerMask ))
        {
            if (SampleNavMesh ( hit.point, 1 )) return new KeyValuePair<bool, Vector3> ( true, hit.point );
            else return new KeyValuePair<bool, Vector3> ( false, Vector3.zero );
        }
        else
        {
            return new KeyValuePair<bool, Vector3> ( false, Vector3.zero );
        }
    }

    public Vector3 GetRandomNavMeshCirclePosition (Vector3 from, float dist)
    {
        Vector3 newPosition = new Vector3 ( from.x, from.y, from.z );

        Vector3 direction = new Vector3 ();

        if (Random.value > 0.50f)
            direction.x += Random.Range ( 0.0f, 1.0f );
        else direction.x += Random.Range ( -1.0f, 0.0f );

        if (Random.value > 0.50f)
            direction.z += Random.Range ( 0.0f, 1.0f );
        else direction.z += Random.Range ( -1.0f, 0.0f );

        direction.y = 0.0f;

        newPosition += direction.normalized * dist;
        newPosition.y = 150.0f;

        Ray ray = new Ray ( newPosition, Vector3.down );
        RaycastHit hit;

        if (Physics.Raycast ( ray, out hit, 1000, layerMask ))
        {
            if (SampleNavMesh ( hit.point, 1 )) return hit.point;
            else return from;
        }
        else
        {
            return from;
        }
    }

    private bool SampleNavMesh (Vector3 point, int mask)
    {
        NavMeshHit hit;

        if (NavMesh.SamplePosition ( point, out hit, 1.0f, NavMesh.AllAreas ))
        {
            if (hit.mask == mask)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    private void OnDestroy ()
    {
        EntityManager.Instance.OnEntityDestroyed ( this.gameObject, this.GetType () );
    }
}