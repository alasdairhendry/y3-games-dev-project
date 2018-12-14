using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleJob_Dance : IdleJob {

    private bool toldToDance = false;

    public IdleJob_Dance (JobEntity entity, string name)
    {
        this.ID = JobController.GetNewJobID ();
        this.JobEntity = entity;
        this.Name = name;
        this.Open = true;
        this.IdleJob = true;
        this.professionTypes = new List<ProfessionType> ();
    }

    public override void DoJob (float deltaGameTime)
    {
        base.DoJob ( deltaGameTime );

        if (!toldToDance)
        {
            if (cBase == null) return;
            cBase.CitizenAnimation.animator.SetBool ( "Dance", true );
            AgentJobStatus = "Dancing";
        }
    }

    public override void OnCharacterAccept (CitizenBase character)
    {
        base.OnCharacterAccept ( character );
    }

    public override void OnCharacterLeave (string reason, bool setOpenToTrue)
    {
        cBase.CitizenAnimation.animator.SetBool ( "Dance", false );
        toldToDance = false;

        base.OnCharacterLeave ( reason, setOpenToTrue );
    }
}
