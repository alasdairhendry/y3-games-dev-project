using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Job_Miner : Job_Profession {

    private Transform jobPoint;
    private bool givenDestination = false;
    private bool insideMine = false;

    public Job_Miner (JobEntity entity, string name, bool open, float timeRequired, System.Action onComplete, Prop_Mine prop, Transform jobPoint) : base ( entity, name, open, timeRequired, onComplete, prop )
    {
        this.professionTypes.Add ( ProfessionType.Miner );
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

        if (cBase == null) return;
        if (cBase.gameObject == null) return;

        DoJob_Specific ();
    }

    private void DoJob_Specific ()
    {
        if (!givenDestination)
        {
            givenDestination = true;
            targetPosition = targetProp.CitizenInteractionPointGlobal;
            SetDestination ( targetProp.gameObject );
            provideResources = false;
            return;
        }

        if (!citizenReachedPath) return;

        if (!insideMine)
        {
            this.cBase.EnterProp ( targetProp );
            this.cBase.CitizenMovement.WarpSpecific ( jobPoint.position );
            insideMine = true;
            return;
        }

        if (!provideResources) provideResources = true;

        this.cBase.CitizenGraphics.SetUsingPickaxe ( true );

        LookAtTarget ( jobPoint.transform.position + (jobPoint.transform.forward * 3.0f) );
    }

    public override void OnCharacterLeave (string reason, bool setOpenToTrue, KeyValuePair<bool, string> isCompletable)
    {
        this.cBase?.CitizenGraphics?.SetUsingPickaxe ( false );

        if (insideMine)
        {
            this.cBase?.ExitProp ( targetProp );
            this.cBase?.CitizenMovement?.WarpAgentToNavMesh ( targetProp.CitizenInteractionPointGlobal );
        }

        givenDestination = false;
        insideMine = false;

        base.OnCharacterLeave ( reason, setOpenToTrue, isCompletable );
    }

}
