﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CitizenBase : MonoBehaviour {

    public int ID { get; protected set; }
    public WorldEntity WorldEntity { get; protected set; }
    public CitizenMovement CitizenMovement { get; protected set; }
    public CitizenAnimation CitizenAnimation { get; protected set; }
    public CitizenGraphics CitizenGraphics { get; protected set; }
    public CitizenJob CitizenJob { get; protected set; }
    public CitizenIdleJob CitizenIdleJob { get; protected set; }
    public CitizenNeeds CitizenNeeds { get; protected set; }
    public CitizenAge CitizenAge { get; protected set; }
    public CitizenFamily CitizenFamily { get; protected set; }
    public CitizenHousing CitizenHousing { get; protected set; }
    public CitizenFootsteps CitizenFootsteps { get; protected set; }
    public CitizenTaxing CitizenTaxing { get; protected set; }
    public Inspectable Inspectable { get; protected set; }

    private NavMeshAgent agent { get; set; }
    public ResourceInventory Inventory { get; protected set; }

    [SerializeField] private LayerMask layerMask;
    
    public System.Action<CitizenBase> OnCitizenDied;

    private void Awake ()
    {
        WorldEntity = GetComponent<WorldEntity> ();
        CitizenMovement = GetComponent<CitizenMovement> ();
        CitizenAnimation = GetComponent<CitizenAnimation> ();
        CitizenGraphics = GetComponent<CitizenGraphics> ();
        CitizenJob = GetComponent<CitizenJob> ();
        CitizenIdleJob = GetComponent<CitizenIdleJob> ();
        CitizenNeeds = GetComponent<CitizenNeeds> ();
        CitizenAge = GetComponent<CitizenAge> ();
        CitizenFamily = GetComponent<CitizenFamily> ();
        CitizenHousing = GetComponent<CitizenHousing> ();
        CitizenFootsteps = FindObjectOfType<CitizenFootsteps> ();
        CitizenTaxing = FindObjectOfType<CitizenTaxing> ();
        Inspectable = GetComponent<Inspectable> ();
        
        Inventory = new ResourceInventory ( float.MaxValue );
        agent = GetComponent<NavMeshAgent> ();
        EntityManager.Instance.OnEntityCreated ( this.gameObject, this.GetType () );

        CitizenController.Instance.currentCitizensCount++;
        OnCitizenDied += (cBase) => { CitizenController.Instance.currentCitizensCount--; };
    }

    private void Start ()
    {
        SetInspection ();
    }

    private void SetInspection ()
    {
        Inspectable.SetDestroyAction ( () => { KillCitizen ( "Murder By User" ); }, false, "Kill" );
        Inspectable.SetFocusAction ( () => { Inspectable.InspectAndLockCamera (); }, false );

        Inspectable.SetAdditiveAction ( () =>
        {
            HUD_EntityInspection_Citizen_Panel panel = FindObjectOfType<HUD_EntityInspection_Citizen_Panel> ();

            #region Buttons
            panel.AddButtonData ( () =>
            {
                if (this == null) return;

                CitizenNeeds.NeedsDictionary[Need.Type.Energy].SetBase ( 0 );

            }, "Set Energy 0", "Needs" );            

            panel.AddButtonData ( () =>
            {
                if (this == null) return;

                if (this.CitizenJob.GetCurrentJob == null) return;
                else this.CitizenJob.GetCurrentJob.OnCharacterLeave ( "Citizen Left", true, Job.GetCompletableParams ( Job.CompleteIdentifier.None ) );
            }, "Cancel Job", "Any" );

            panel.AddButtonData ( () =>
            {
                if (this == null) return;

                KillCitizen ( "Murder By User" );

            }, "Kill Citizen", "Any" );
            #endregion

            panel.AddTextData ( (pair) =>
            {
                if (CitizenFamily.thisMember == null) return "None";
                string s = "";
                if (!string.IsNullOrEmpty ( CitizenFamily.thisMember.firstName )) s += CitizenFamily.thisMember.firstName;
                if (!string.IsNullOrEmpty ( CitizenFamily.familyName )) s += " " + CitizenFamily.familyName;
                if (string.IsNullOrEmpty ( s )) s = "None";
                return s;
            }, "Name", "Overview" );

            if (CitizenJob.profession == ProfessionType.None || CitizenJob.profession == ProfessionType.Student)
            {
                panel.AddTextData ( (pair) =>
                {
                    return CitizenJob.profession.ToString ();
                }, "Profession", "Overview" );
            }
            else
            {
                panel.AddDropdownData ( (index, options) =>
                {
                    ProfessionController.Instance.SetProfession ( CitizenJob, options[index].text );
                }, (pair) =>
                {
                    return CitizenJob.profession.ToString ();
                }, "Profession", "Overview",
                ProfessionController.Instance.GetProfessionsAreStringList ( 2 ) );
            }

            panel.AddButtonTextData ( () =>
            {
               // Open tax band panel
            }, (pair) =>
            {
                return TaxController.Instance.GetRequiredTax ( CitizenJob.profession ).ToString ( "0.0" );

            }, "Daily Tax", "Overview" );

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

            CitizenFootsteps.SetState ( true, this);

            panel.AddOnCloseAction ( () =>
             {
                 if (this == null) return;

                 CitizenFootsteps.SetState ( false, this );
             } );
        } );
    }

    public void SetID(int id)
    {
        this.ID = id;
    }

    public void SetInventory(List<int> ids, List<float> quantities)
    {
        Inventory = new ResourceInventory ( float.MaxValue );
        
        for (int i = 0; i < ids.Count; i++)
        {
            WarehouseController.Instance.Inventory.AddItemQuantity ( ids[i], quantities[i] );
        }
    }

    [ContextMenu ( "Kill" )]
    public void KillCitizen (string reason)
    {
        HUD_Notification_Panel.Instance.AddNotification ( CitizenFamily.thisMember.fullName + " has died at age " + CitizenAge.Age + " of " + reason, HUD_Notification_Panel.NotificationSprite.Warning, null );

        OnCitizenDied?.Invoke ( this );
        if (this.CitizenJob.GetCurrentJob != null) this.CitizenJob.GetCurrentJob.OnCharacterLeave ( "Citizen Died", true, Job.GetCompletableParams ( Job.CompleteIdentifier.None ) );
        ProfessionController.Instance.SetProfessionExplicit ( this.CitizenJob );
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

    public void EnterProp(Prop prop)
    {
        Inspectable.OverrideLockTo ( prop.Inspectable );
        if (Inspectable.isInspected) Inspectable.InspectAndLockCamera ();
    }

    public void ExitProp(Prop prop)
    {
        Inspectable.OverrideLockTo ( null );
        if (Inspectable.isInspected) Inspectable.InspectAndLockCamera ();
    }

    private void OnDestroy ()
    {
        EntityManager.Instance.OnEntityDestroyed ( this.gameObject, this.GetType () );
    }
}