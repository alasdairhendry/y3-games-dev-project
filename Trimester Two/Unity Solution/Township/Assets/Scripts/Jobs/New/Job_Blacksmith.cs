using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Job_Blacksmith : Job_Profession {

    private Transform targetPoint;
    private GameObject smeltPoint;
    private GameObject anvilPoint;
    private bool givenDestination = false;

    public bool smelting { get; protected set; }

    public Job_Blacksmith (JobEntity entity, string name, bool open, float timeRequired, System.Action onComplete, Prop_Smithery prop, GameObject smeltPoint, GameObject anvilPoint) : base ( entity, name, open, timeRequired, onComplete, prop )
    {
        this.professionTypes.Add ( ProfessionType.Blacksmith );
        this.smeltPoint = smeltPoint;
        this.anvilPoint = anvilPoint;

        SetIsSmelting ( true );
        //targetPoint = smeltPoint.transform;

        BreakForEnergy = true;
    }

    public void SetIsSmelting (bool state)
    {
        smelting = state;
        this.cBase?.CitizenGraphics?.SetUsingAxe ( false, CitizenAnimation.AxeUseAnimation.Splitting );

        if (smelting)
        {
            targetPoint = smeltPoint.transform;
        }
        else
        {
            targetPoint = anvilPoint.transform;
        }

        givenDestination = false;
        provideResources = false;
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
            targetPosition = targetPoint.transform.position;
            SetDestination ( targetPoint.gameObject );
            provideResources = false;
            return;
        }

        if (smelting) base.AgentJobStatus = "Walking to ore smelter";
        else base.AgentJobStatus = "Walking to anvil";

        if (!citizenReachedPath) return;

        if (smelting) base.AgentJobStatus = "Smelting iron ore";
        else base.AgentJobStatus = "Smithing iron tools";

        if (!provideResources) provideResources = true;

        this.cBase.GetComponent<CitizenGraphics> ().SetUsingAxe ( true, CitizenAnimation.AxeUseAnimation.Splitting );

        LookAtTarget ( targetPoint.transform.position + (targetPoint.transform.forward * 3.0f) );
    }

    public void CheckMaterials ()
    {
        if (targetInventory.CheckIsFull ( targetProp.resourceIDToGive )) return;
        if (targetProp.consumeAmount > 0)
        {
            if (targetInventory.CheckIsEmpty ( targetProp.resourceIDToConsume )) return;
        }

        base.SetIsCompletable ();
    }

    public override void OnCharacterLeave (string reason, bool setOpenToTrue, KeyValuePair<bool, string> isCompletable)
    {
        this.cBase?.CitizenGraphics?.SetUsingAxe ( false, CitizenAnimation.AxeUseAnimation.Splitting );
        givenDestination = false;

        base.OnCharacterLeave ( reason, setOpenToTrue, isCompletable );
    }

}
