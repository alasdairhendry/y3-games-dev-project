using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Job_Profession : Job {

    protected bool provideResources = false;

    protected Prop_Profession targetProp;
    protected ResourceInventory targetInventory;

    public Job_Profession (JobEntity entity, string name, bool open, float timeRequired, System.Action onComplete, Prop_Profession targetProp) : base ( entity, name, open, timeRequired, onComplete )
    {
        this.targetProp = targetProp;
        this.targetInventory = targetProp.inventory;
    }

    public override void DoJob (float deltaGameTime)
    {
        if (!provideResources) return;
        if (targetProp == null) return;
        if (targetProp.HaltProduction) return;

        targetProp.AddProduction ();       
    }

    public override void OnCharacterLeave (string reason, bool setOpenToTrue)
    {
        GameTime.RegisterGameTick ( Tick_CheckCompletable );
        if (this.cBase == null)
        {
            if (setOpenToTrue) this.Open = true;
            Debug.Log ( this.Name + " has no character assigned" ); return;            
        }
        else
        {
            this.cBase.CitizenMovement.ClearDestination ();
            this.cBase.CitizenJob.OnJob_Leave ();
            this.cBase = null;
        }

        if (setOpenToTrue)
            this.Open = true;
        if (OnCharacterLeaveAction != null) OnCharacterLeaveAction ();
        if (OnCharacterChanged != null) OnCharacterChanged ();
    }
}
