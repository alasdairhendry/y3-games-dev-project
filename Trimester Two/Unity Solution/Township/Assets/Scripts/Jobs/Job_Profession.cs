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
        BreakForEnergy = true;
    }

    public override void DoJob ()
    {
        base.DoJob ();

        if (!provideResources) return;
        if (targetProp == null) return;
        if (targetProp.HaltProduction) return;

        targetProp.AddProduction ();       
    }

    public override void OnCharacterLeave (string reason, bool setOpenToTrue, KeyValuePair<bool, string> isCompletable)
    {
        if (this.cBase == null)
        {
            if (setOpenToTrue) this.Open = true;
            return;            
        }
        else
        {
            this.cBase.CitizenMovement.ClearDestination ();
            this.cBase.CitizenJob.OnJob_Leave ();
            this.cBase.CitizenMovement.onReachedPath -= OnCitizenPathComplete;
            this.cBase = null;
        }

        if (setOpenToTrue)
        {
            this.Open = true;
        }        

        base.IsCompletable = isCompletable.Key;
        base.IsCompletableReason = isCompletable.Value;

        if (!this.IsCompletable)
        {
            if (!completableListenersAdded)
            {
                AddCompletableListeners ();
            }
        }

        OnCharacterLeaveAction?.Invoke ();
        OnCharacterChanged?.Invoke ();
    }

    protected override void AddCompletableListeners ()
    {
        targetInventory.RegisterOnInventoryChanged ( OnInventoryChanged );

        base.AddCompletableListeners ();
    }

    private void OnInventoryChanged (ResourceInventory inv)
    {
        if (inv.CheckIsFull ( targetProp.resourceIDToGive )) return;
        if (targetProp.consumeAmount > 0)
        {
            if (inv.CheckIsEmpty ( targetProp.resourceIDToConsume )) return;
        }

        base.SetIsCompletable ();
    }

    protected override void RemoveCompletableListeners ()
    {
        targetInventory.UnregisterOnInventoryChanged ( OnInventoryChanged );

        base.RemoveCompletableListeners ();
    }
}
