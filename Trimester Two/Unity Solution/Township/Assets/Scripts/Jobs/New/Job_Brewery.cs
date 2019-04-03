using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Job_Brewery : Job_Profession {

    private Transform entryPoint;
    private Transform squashPoint;
    private ParticleSystem particles;
    private bool givenDestination = false;
    private bool onPlinth = false;

    public Job_Brewery (JobEntity entity, string name, bool open, float timeRequired, System.Action onComplete, Prop_Brewery prop, Transform entryPoint, Transform squashPoint, ParticleSystem particles) : base ( entity, name, open, timeRequired, onComplete, prop )
    {
        this.professionTypes.Add ( ProfessionType.Winemaker );
        this.entryPoint = entryPoint;
        this.squashPoint = squashPoint;
        this.particles = particles;
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
            targetPosition = entryPoint.transform.position;
            SetDestination ( entryPoint.gameObject );
            provideResources = false;
            return;
        }

        if (!citizenReachedPath) return;

        if (!onPlinth)
        {
            cBase.CitizenMovement.WarpSpecific ( squashPoint.transform.position );
            particles.Play ();

            onPlinth = true;
            return;
        }

        if (!provideResources) provideResources = true;

        this.cBase.CitizenGraphics.SetIsSquashing ( true );


        LookAtTarget ( squashPoint.transform.position + (squashPoint.transform.forward * 3.0f) );
    }

    public override void OnCharacterLeave (string reason, bool setOpenToTrue, KeyValuePair<bool, string> isCompletable)
    {
        this.cBase?.CitizenGraphics?.SetIsSquashing ( false );

        if (onPlinth)
        {
            this.cBase?.CitizenMovement?.WarpAgentToNavMesh ( entryPoint.transform.position );
        }

        particles.Stop ();

        givenDestination = false;
        onPlinth = false;

        base.OnCharacterLeave ( reason, setOpenToTrue, isCompletable );
    }
}
