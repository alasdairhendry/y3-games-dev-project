using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Job_Build : Job {

    private Buildable buildableTarget;
    private float buildSpeed = 0.0f;

    private bool assignedCharacterDestination = false;

    public Job_Build(JobEntity entity, string name, bool open, float timeRequired, System.Action onComplete, float buildSpeed, Buildable buildableTarget) : base (entity, name, open, timeRequired, onComplete)
    {
        this.buildSpeed = buildSpeed;
        this.buildableTarget = buildableTarget;
        this.professionTypes.Add ( ProfessionType.Worker );
        BreakForEnergy = false;
    }

    public override void DoJob ()
    {
        base.DoJob ();

        if (buildableTarget.IsComplete)
        {
            this.cBase.GetComponent<CitizenGraphics> ().SetUsingAxe ( false, CitizenAnimation.AxeUseAnimation.Chopping );
            base.OnComplete ();            
            return;
        }

        if (!assignedCharacterDestination)
        {
            assignedCharacterDestination = true;
            targetPosition = buildableTarget.Prop.CitizenInteractionPointGlobal;
            SetDestination ( buildableTarget.gameObject );
        }

        if (!citizenReachedPath) return;        

        this.cBase.GetComponent<CitizenGraphics> ().SetUsingAxe ( true, CitizenAnimation.AxeUseAnimation.Chopping );
        buildableTarget.AddConstructionPercentage ( GameTime.DeltaGameTime * buildSpeed );

        Quaternion lookRot = Quaternion.LookRotation ( this.buildableTarget.transform.position - this.cBase.transform.position, Vector3.up );
        this.cBase.transform.rotation = Quaternion.Slerp ( this.cBase.transform.rotation, lookRot, GameTime.DeltaGameTime * 2.5f );
    }

    public override void OnCharacterLeave (string reason, bool setOpenToTrue, KeyValuePair<bool, string> isCompletable)
    {
        assignedCharacterDestination = false;
        this.cBase.GetComponent<CitizenGraphics> ().SetUsingAxe ( false, CitizenAnimation.AxeUseAnimation.Chopping );

        base.OnCharacterLeave ( reason, setOpenToTrue, isCompletable );
    }
}
