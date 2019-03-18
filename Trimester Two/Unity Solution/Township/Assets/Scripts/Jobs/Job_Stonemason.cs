using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Job_Stonemason : Job_Profession {

    private bool givenDestination = false;

    private GameObject plinth;
    private GameObject plinthPoint;

    public Job_Stonemason (JobEntity entity, string name, bool open, float timeRequired, System.Action onComplete, Prop_StonemasonHut prop, GameObject plinth, GameObject plinthPoint) : base ( entity, name, open, timeRequired, onComplete, prop )
    {
        this.professionTypes.Add ( ProfessionType.Stonemason );
        this.plinth = plinth;
        this.plinthPoint = plinthPoint;
    }

    public override void DoJob ()
    {
        base.DoJob ();

        if (targetInventory == null)
        {
            Debug.Log ( "Target Inventory Is null" );
            return;
        }

        if (targetInventory.CheckIsFull ( targetProp.resourceIDToGive ))
        {
            OnCharacterLeave ( "Storage is full. Send a market cart!", true, GetCompletableParams ( CompleteIdentifier.PropStorageFull ) );
            //IsCompletable = false;
            return;
        }

        if (targetInventory.CheckIsEmpty ( targetProp.resourceIDToConsume ))
        {
            OnCharacterLeave ( "Stock is empty. Send a market cart!", true, GetCompletableParams ( CompleteIdentifier.PropNoConsumables, targetProp.resourceIDToConsume, targetProp.consumeAmount ) );
            //IsCompletable = false;
            return;
        }

        if (cBase == null) return;
        if (cBase.gameObject == null) return;

        if (!givenDestination)
        {
            givenDestination = true;
            targetPosition = plinthPoint.transform.position;
            SetDestination ( plinth );
            provideResources = false;
            return;
        }

        if (!citizenReachedPath) return;

        if (!provideResources) provideResources = true;

        this.cBase.GetComponent<CitizenGraphics> ().SetUsingAxe ( true, CitizenAnimation.AxeUseAnimation.Splitting );

        LookAtTarget ( plinth.transform.position );
    }

    public override void OnCharacterLeave (string reason, bool setOpenToTrue, KeyValuePair<bool, string> isCompletable)
    {
        if (this.cBase != null)
        {
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
    //    else if (targetInventory.CheckIsEmpty ( targetProp.resourceIDToConsume ))
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
