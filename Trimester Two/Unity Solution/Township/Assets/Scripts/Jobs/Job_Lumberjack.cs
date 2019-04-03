using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Job_Lumberjack : Job_Profession
{
    private bool givenDestination = false;

    private GameObject stump;
    private List<GameObject> trees = new List<GameObject> ();

    public enum JobState { Chopping, Splitting }
    private JobState jobState = JobState.Splitting;

    public Job_Lumberjack (JobEntity entity, string name, bool open, float timeRequired, System.Action onComplete, Prop_LumberjackHut prop, List<GameObject> trees, GameObject stump) : base ( entity, name, open, timeRequired, onComplete, prop )
    {
        this.professionTypes.Add ( ProfessionType.Lumberjack );
        this.trees = trees;
        this.stump = stump;        
    }

    public override void DoJob ()
    {
        base.DoJob ();

        if(targetInventory == null)
        {
            Debug.Log ( "Target Inventory Is null" );
            this.targetInventory = targetProp.inventory;
            return;
        }

        if (targetInventory.CheckIsFull ( targetProp.resourceIDToGive ))
        {
            OnCharacterLeave ( "Storage is full. Send a market cart!", true, GetCompletableParams(CompleteIdentifier.PropStorageFull) );
            return;
        }

        if (cBase == null) return;
        if (cBase.gameObject == null) return;

        switch (jobState)
        {
            case JobState.Chopping:
                DoJob_Chopping ();
                break;
            case JobState.Splitting:
                DoJob_Splitting ();
                break;
        }
    }

    private void DoJob_Chopping ()
    {
    
    }

    private void DoJob_Splitting ()
    {
        if (!givenDestination)
        {
            givenDestination = true;
            targetPosition = stump.transform.GetChild ( 0 ).transform.position + (stump.transform.GetChild ( 0 ).transform.right * 2.0f);
            SetDestination ( stump );
            provideResources = false;
            return;
        }

        if (!citizenReachedPath) return;

        if (!provideResources) provideResources = true;

        this.cBase.GetComponent<CitizenGraphics> ().SetUsingAxe ( true, CitizenAnimation.AxeUseAnimation.Splitting );

        LookAtTarget ( stump.transform.GetChild ( 0 ).transform.position );
    }

    public override void OnCharacterLeave (string reason, bool setOpenToTrue, KeyValuePair<bool, string> isCompletable)
    {
        if (this.cBase != null)
        {
            this.cBase.CitizenGraphics.SetUsingAxe ( false, CitizenAnimation.AxeUseAnimation.Chopping );
            this.cBase.CitizenGraphics.SetUsingAxe ( false, CitizenAnimation.AxeUseAnimation.Splitting );
        }
        givenDestination = false;

        base.OnCharacterLeave ( reason, setOpenToTrue, isCompletable );
    }

    //protected override IEnumerator CheckIsCompletable ()
    //{
    //    if (targetProp.HaltProduction) { IsCompletable = false; yield break; }

    //    if (targetInventory.CheckIsFull ( targetProp.resourceIDToGive ))
    //    {
    //        IsCompletable = false;
    //        yield break;
    //    }
    //    else
    //    {
    //        IsCompletable = true;
    //        yield break;
    //    }
    //}
}
