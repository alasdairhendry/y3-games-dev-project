using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Job_Vintner : Job_Profession {

    private List<Transform> waypoints;
    private bool givenDestination = false;

    private int waypointIndex = 0;
    private float waitDelay = 0;
    private bool reachedFirstWaypoint = false;

    public Job_Vintner (JobEntity entity, string name, bool open, float timeRequired, System.Action onComplete, Prop_Orchard prop, List<Transform> waypoints) : base ( entity, name, open, timeRequired, onComplete, prop )
    {
        this.professionTypes.Add ( ProfessionType.Vintner );
        this.waypoints = waypoints;
        this.provideResources = false;

        BreakForEnergy = true;
    }

    public override void DoJob ()
    {
        base.DoJob ();

        if (targetInventory == null)
        {
            Debug.Log ( "Target Inventory Is null" );
            this.targetInventory = targetProp.inventory;
            return;
        }

        if (targetInventory.CheckIsFull ( targetProp.resourceIDToGive ))
        {
            OnCharacterLeave ( "Storage is full. Send a market cart!", true, GetCompletableParams ( CompleteIdentifier.PropStorageFull ) );
            return;
        }

        if (cBase == null) return;
        if (cBase.gameObject == null) return;

        DoSpecificJob ();
    }

    private void DoSpecificJob ()
    {
        if (!givenDestination)
        {
            givenDestination = true;
            targetPosition = waypoints[waypointIndex].transform.position;
            SetDestination ( waypoints[waypointIndex].gameObject );

            waitDelay = Random.Range ( 2.0f, 6.0f );
            waypointIndex++;
            if (waypointIndex >= waypoints.Count) waypointIndex = 0;
            return;
        }

        if (!citizenReachedPath) return;

        if (!reachedFirstWaypoint)
        {
            reachedFirstWaypoint = true;

            if (!provideResources) { provideResources = true; }
        }

        LookAtTarget ( waypoints[waypointIndex].position + (waypoints[waypointIndex].forward * 2.0f) );

        waitDelay -= GameTime.DeltaGameTime;

        if(waitDelay <= 0.0f)
        {
            givenDestination = false;
            return;
        }
    }

    public override void OnCharacterLeave (string reason, bool setOpenToTrue, KeyValuePair<bool, string> isCompletable)
    {
        if (this.cBase != null)
        {
            //this.cBase.CitizenGraphics.SetUsingAxe ( false, CitizenAnimation.AxeUseAnimation.Chopping );
            //this.cBase.CitizenGraphics.SetUsingAxe ( false, CitizenAnimation.AxeUseAnimation.Splitting );
            //this.cBase.CitizenGraphics.SetUsingRod ( false );
        }
        givenDestination = false;
        waitDelay = 0.0f;
        reachedFirstWaypoint = false;
        provideResources = false;

        base.OnCharacterLeave ( reason, setOpenToTrue, isCompletable );
    }
}
