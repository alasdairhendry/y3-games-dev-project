﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Job_QuarryWorker : Job_Profession {

    private bool givenDestination = false;

    private GameObject rock;

    public enum JobState { Mining, Welling }
    private JobState jobState = JobState.Mining;

    public Job_QuarryWorker (JobEntity entity, string name, bool open, float timeRequired, System.Action onComplete, Prop_Quarry prop, GameObject rock, GameObject wellPoint) : base ( entity, name, open, timeRequired, onComplete, prop )
    {
        this.professionTypes.Add ( ProfessionType.Quarryman );
        this.rock = rock;
    }

    public override void DoJob ()
    {
        base.DoJob ();

        if (targetInventory.CheckIsFull ( targetProp.resourceIDToGive ))
        {
            OnCharacterLeave ( "Storage is full. Send a market cart!", true, GetCompletableParams ( CompleteIdentifier.PropStorageFull ) );
            //IsCompletable = false;
            return;
        }

        if (cBase == null) return;
        if (cBase.gameObject == null) return;

        switch (jobState)
        {
            case JobState.Mining:
                DoJob_Mining ();
                break;
            case JobState.Welling:
                DoJob_Welling ();
                break;
        }
    }

    private void DoJob_Mining ()
    {
        if (!givenDestination)
        {
            givenDestination = true;
            targetPosition = rock.transform.GetChild ( 0 ).transform.position;
            SetDestination ( rock );
            provideResources = false;
        }

        if (!citizenReachedPath) return;

        if (!provideResources) provideResources = true;

        this.cBase.GetComponent<CitizenGraphics> ().SetUsingPickaxe ( true );

        LookAtTarget ( rock.transform.position );
    }

    private void DoJob_Welling ()
    {

    }

    public override void OnCharacterLeave (string reason, bool setOpenToTrue, KeyValuePair<bool, string> isCompletable)
    {
        if (this.cBase != null)
        {
            this.cBase.CitizenGraphics.SetUsingPickaxe ( false );
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
