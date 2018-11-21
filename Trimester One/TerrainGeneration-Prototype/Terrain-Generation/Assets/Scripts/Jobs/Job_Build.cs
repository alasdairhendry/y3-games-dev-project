using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Job_Build : Job {

    private Buildable buildableTarget;
    private float buildSpeed = 0.0f;

    public Job_Build(string name, bool open, float buildSpeed, Buildable buildableTarget)
    {
        this.Name = name;
        this.Open = open;
        this.buildSpeed = buildSpeed;
        this.buildableTarget = buildableTarget;
    }

    public override void DoJob (float deltaGameTime)
    {
        if (buildableTarget.IsComplete)
        {
            base.OnComplete ();
            return;
        }

        buildableTarget.AddConstructionPercentage ( deltaGameTime * buildSpeed );
    }

}
