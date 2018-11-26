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
            this.character.GetComponent<CharacterGraphics> ().OnUseAxeAction ( false );
            base.OnComplete ();            
            return;
        }

        if (!assignedCharacterDestination)
        {
            assignedCharacterDestination = true;
            this.character.agent.SetDestination ( buildableTarget.GetPropData.CitizenInteractionPointGlobal );
        }

        if (!ReachedPath ()) return;

        this.character.GetComponent<CharacterGraphics> ().OnUseAxeAction ( true );
        buildableTarget.AddConstructionPercentage ( deltaGameTime * buildSpeed );
    }

    public override void OnCharacterLeave (string reason)
    {
        assignedCharacterDestination = false;
        this.character.GetComponent<CharacterGraphics> ().OnUseAxeAction ( false );

        base.OnCharacterLeave ( reason );
    }

}
