using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Job_Profession : Job {

    protected float provideResourceDelay = 20.0f;
    protected float provideResourceCounter = 0.0f;

    protected int resourceIDToConsume = -1;
    protected int resourceIDToGive = -1;

    protected int consumeAmount = 1;
    protected int giveAmount = 1;

    protected bool provideResources = false;

    protected Prop_Profession targetProp;
    protected ResourceInventory targetInventory;

    public Job_Profession (JobEntity entity, string name, bool open, float timeRequired, System.Action onComplete) : base ( entity, name, open, timeRequired, onComplete ) { }

    public override void DoJob (float deltaGameTime)
    {
        if (!provideResources) return;
        if (targetProp.HaltProduction) return;
        
        provideResourceCounter += GameTime.DeltaGameTime;

        if(provideResourceCounter >= provideResourceDelay)
        {
            provideResourceCounter = 0.0f;
            OnResourceDelayActive ();
        }

        CheckJobs ();
    }

    protected virtual void CheckJobs ()
    {
        if (resourceIDToConsume < 0 && resourceIDToGive < 0)
        {
            Debug.LogError ( "Job: " + Name + " does not give or take any resources" );
            return;
        }

        if (resourceIDToConsume >= 0)
        {
            if (targetInventory.CheckIsEmpty ( resourceIDToConsume ))
            {
                if (!targetProp.createdSupplyJob)
                {
                    targetProp.GetComponent<JobEntity> ().CreateJob_MarketCart ( "Supply " + resourceIDToConsume + " to " + targetProp.name, true,
                        5.0f, () => { targetProp.createdSupplyJob = false; }, targetProp, targetInventory, resourceIDToConsume, 5, true );
                    targetProp.createdSupplyJob = true;
                }
            }
        }

        if (resourceIDToGive >= 0)
        {
            if (targetInventory.CheckIsFull ( resourceIDToGive ))
            {
                if (!targetProp.createdCollectJob)
                {
                    targetProp.GetComponent<JobEntity> ().CreateJob_MarketCart ( "Collect " + resourceIDToGive + " from " + targetProp.name, true,
                        5.0f, () => { targetProp.createdCollectJob = false; }, targetProp, targetInventory, resourceIDToGive, 5, false );
                    targetProp.createdCollectJob = true;
                }
            }
        }
    }

    protected virtual void OnResourceDelayActive ()
    {
        if(resourceIDToConsume <0 && resourceIDToGive < 0)
        {
            Debug.LogError ( "Job: " + Name + " does not give or take any resources" );
            return;
        }

        if(resourceIDToConsume < 0)
        {
            // TODO: - If this returns something, we are full, alert the user
            targetInventory.AddItemQuantity ( resourceIDToGive, giveAmount );
        }
        else
        {
            if(targetInventory.CheckHasQuantityAvailable(resourceIDToConsume, consumeAmount ))
            {
                targetInventory.RemoveItemQuantity ( resourceIDToConsume, consumeAmount );
                targetInventory.AddItemQuantity ( resourceIDToGive, giveAmount );
            }
            else
            {
                Debug.Log ( "Job: " + Name + " does not have enough consumer resources" );
            }
        }
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
        //this.onComplete = null;
        if (OnCharacterLeaveAction != null) OnCharacterLeaveAction ();
        if (OnCharacterChanged != null) OnCharacterChanged ();
    }
}
