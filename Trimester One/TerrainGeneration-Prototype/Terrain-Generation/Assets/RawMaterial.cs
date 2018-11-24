using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RawMaterial : MonoBehaviour {

    [SerializeField] private int resourceProvided;
    [SerializeField] private float quantityProvided;

    [SerializeField] Job_GatherResource gatherJob;

    [ContextMenu( "CreateRemovalJob" )]
    public void CreateRemovalJob ()
    {        
        if (!gatherJob.IsNull ()) return;

        Job_GatherResource job = new Job_GatherResource ( "Gather Resource", true, resourceProvided, quantityProvided, 10.0f, this );
        gatherJob = job;
        JobController.QueueJob ( job );
    }

    public virtual void Gather ()
    {        
        Destroy ( this.gameObject );
    }

    public virtual void Remove ()
    {
        if (gatherJob != null) JobController.RemoveJob ( gatherJob );

        Destroy ( this.gameObject );
    }
}
