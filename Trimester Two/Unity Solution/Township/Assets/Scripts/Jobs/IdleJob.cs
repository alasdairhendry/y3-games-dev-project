using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IdleJob : Job
{
    protected float averageIdleTime = 7.5f;
    protected float targetIdleTime = 7.5f;
    protected float currentIdleTime = 0.0f;

    public override void DoJob (float deltaGameTime)
    {
        currentIdleTime += deltaGameTime;

        if(currentIdleTime >= targetIdleTime)
        {
            OnCharacterLeave ( "Spent too much time idling", true );
        }
    }

    public override void OnCharacterAccept (CitizenBase character)
    {
        base.OnCharacterAccept ( character );
        currentIdleTime = 0.0f;
        targetIdleTime = averageIdleTime * Random.Range ( 0.8f, 1.2f );
    }

    public override void OnCharacterLeave (string reason, bool setOpenToTrue)
    {
        if (this.cBase == null)
        {
            Debug.Log ( this.Name + " has no character assigned" );
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
