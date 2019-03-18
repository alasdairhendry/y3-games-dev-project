using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Job_Fisherman : Job_Profession {

    public enum JobState { Fishing, Chopping }
    private JobState jobState = JobState.Fishing;

    private GameObject target;
    private bool givenDestination = false;

    public Job_Fisherman (JobEntity entity, string name, bool open, float timeRequired, System.Action onComplete, Prop_FishingHut prop, GameObject target, JobState state) : base ( entity, name, open, timeRequired, onComplete, prop )
    {
        this.professionTypes.Add ( ProfessionType.Fisherman );
        this.target = target;
        this.jobState = state;
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
            targetPosition = target.transform.position;
            SetDestination ( target );
            provideResources = false;
            return;
        }

        if (!citizenReachedPath) return;

        if (!provideResources) provideResources = true;

        //this.cBase.GetComponent<CitizenGraphics> ().SetUsingAxe ( true, CitizenAnimation.AxeUseAnimation.Splitting );
        this.cBase.CitizenGraphics.SetUsingRod ( true );

        LookAtTarget ( target.transform.position + (target.transform.forward * 2.0f) );
    }

    public override void OnCharacterLeave (string reason, bool setOpenToTrue, KeyValuePair<bool, string> isCompletable)
    {
        if (this.cBase != null)
        {
            //this.cBase.CitizenGraphics.SetUsingAxe ( false, CitizenAnimation.AxeUseAnimation.Chopping );
            //this.cBase.CitizenGraphics.SetUsingAxe ( false, CitizenAnimation.AxeUseAnimation.Splitting );
            this.cBase.CitizenGraphics.SetUsingRod ( false );
        }
        givenDestination = false;

        base.OnCharacterLeave ( reason, setOpenToTrue, isCompletable );
    }
}
