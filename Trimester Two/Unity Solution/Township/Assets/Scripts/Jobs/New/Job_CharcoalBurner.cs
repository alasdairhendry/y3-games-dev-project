using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Job_CharcoalBurner : Job_Profession {

    private Transform jobPoint;
    private bool givenDestination = false;

    public Job_CharcoalBurner (JobEntity entity, string name, bool open, float timeRequired, System.Action onComplete, Prop_CharcoalBurnerHut prop, Transform jobPoint) : base ( entity, name, open, timeRequired, onComplete, prop )
    {
        this.professionTypes.Add ( ProfessionType.Charcoal_Burner );
        this.jobPoint = jobPoint;
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

        if (!targetInventory.CheckCanHold ( targetProp.resourceIDToGive, targetProp.giveAmount ))
        {
            OnCharacterLeave ( "Storage is full. Send a market cart!", true, GetCompletableParams ( CompleteIdentifier.PropStorageFull ) );
            return;
        }

        if (!targetInventory.CheckHasQuantityAvailable ( targetProp.resourceIDToConsume, targetProp.consumeAmount ))
        {
            OnCharacterLeave ( "Stock is empty. Send a market cart!", true, GetCompletableParams ( CompleteIdentifier.PropNoConsumables, targetProp.resourceIDToConsume, targetProp.consumeAmount ) );
            return;
        }

        if (cBase == null) return;
        if (cBase.gameObject == null) return;

        DoJob_Specific ();
    }

    private void DoJob_Specific ()
    {
        if (!givenDestination)
        {
            givenDestination = true;
            targetPosition = jobPoint.position;
            SetDestination ( jobPoint.gameObject );
            provideResources = false;
            return;
        }

        if (!citizenReachedPath) return;

        if (!provideResources) provideResources = true;

        this.cBase.CitizenGraphics.SetUsingAxe ( true, CitizenAnimation.AxeUseAnimation.Splitting );

        LookAtTarget ( jobPoint.transform.position + (jobPoint.transform.forward * 3.0f) );
    }

    public override void OnCharacterLeave (string reason, bool setOpenToTrue, KeyValuePair<bool, string> isCompletable)
    {
        this.cBase?.CitizenGraphics?.SetUsingAxe ( false, CitizenAnimation.AxeUseAnimation.Splitting );

        givenDestination = false;
        base.OnCharacterLeave ( reason, setOpenToTrue, isCompletable );
    }
}
