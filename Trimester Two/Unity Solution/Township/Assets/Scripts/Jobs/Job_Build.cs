using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Job_Build : Job {

    private Buildable buildableTarget;
    private float buildSpeed = 0.0f;

    private bool assignedCharacterDestination = false;

    public Job_Build(JobEntity entity, string name, bool open, float buildSpeed, Buildable buildableTarget)
    {
        this.id = JobController.GetNewJobID ();
        this.jobEntity = entity;
        this.Name = name;
        this.Open = open;
        
        this.buildSpeed = buildSpeed;
        this.buildableTarget = buildableTarget;
    }

    public override void DoJob (float deltaGameTime)
    {
        if (buildableTarget.IsComplete)
        {
            this.character.GetComponent<CitizenGraphics> ().OnUseAxeAction ( false );
            base.OnComplete ();            
            return;
        }

        if (!assignedCharacterDestination)
        {
            assignedCharacterDestination = true;
            this.character.CitizenMovement.SetDestination ( buildableTarget.gameObject, buildableTarget.GetPropData.CitizenInteractionPointGlobal );
        }

        if (!this.character.CitizenMovement.ReachedPath ()) return;

        this.character.GetComponent<CitizenGraphics> ().OnUseAxeAction ( true );
        buildableTarget.AddConstructionPercentage ( deltaGameTime * buildSpeed );

        Quaternion lookRot = Quaternion.LookRotation ( this.buildableTarget.transform.position - this.character.transform.position, Vector3.up );
        this.character.transform.rotation = Quaternion.Slerp ( this.character.transform.rotation, lookRot, GameTime.DeltaGameTime * 2.5f );
    }

    public override void OnCharacterLeave (string reason)
    {
        assignedCharacterDestination = false;
        this.character.GetComponent<CitizenGraphics> ().OnUseAxeAction ( false );

        base.OnCharacterLeave ( reason );
    }

    public override bool IsCompletable ()
    {
        return true;
    }
}
