using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop_StonemasonHut : Prop_Profession {

    [SerializeField] private GameObject plinth;
    [SerializeField] private GameObject plinthPoint;

    protected override void OnPlaced ()
    {
        base.OnPlaced ();
    }

    protected override void SetResources ()
    {
        resourceIDToGive = 2;
        resourceIDToConsume = 1;

        giveAmount = 1;
        consumeAmount = 2;

        ProductionRequired = 20.0f;
    }

    // Called from Prop_Profession on built "MaxJobs" times
    protected override void CreateProfessionJobs (int index)
    {
        Job_Stonemason job = GetComponent<JobEntity> ().CreateJob_StoneMason ( "Stonemason", !HaltProduction, 5.0f, null, this, plinth, plinthPoint);
        professionJobs.Add ( job );
        JobController.QueueJob ( job );
    }
}
