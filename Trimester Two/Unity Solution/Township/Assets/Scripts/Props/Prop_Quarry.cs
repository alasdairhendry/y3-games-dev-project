using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Prop_Quarry : Prop_Profession {

    [SerializeField] private List<GameObject> rocks = new List<GameObject> ();
    [SerializeField] private List<GameObject> wellPoints = new List<GameObject> ();

    protected override void OnPlaced ()
    {
        base.OnPlaced ();

        FindObjectOfType<World> ().DEBUG_UpdateNavMesh ();
    }

    protected override void SetResources ()
    {
        resourceIDToGive = 1;
        resourceIDToConsume = -1;

        giveAmount = 1;
        consumeAmount = 0;

        ProductionRequired = 60.0f;
    }

    // Called from Prop_Profession on built "MaxJobs" times
    protected override void CreateProfessionJobs (int index)
    {
        Job_QuarryWorker job = GetComponent<JobEntity> ().CreateJob_QuarryWorker ( "Quarry Work", !HaltProduction, 5.0f, null, this, rocks[index], wellPoints[index] );
        professionJobs.Add ( job );
        JobController.QueueJob ( job );
    }

    protected override void SetInspectable ()
    {
        base.SetInspectable ();
    }

}
