using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop_FishingHut : Prop_Profession {

    [SerializeField] private GameObject fishingSpot;
    [SerializeField] private GameObject choppingSpot;
    int jobIndex = 0;

    protected override void OnPlaced ()
    {
        base.OnPlaced ();

        FindObjectOfType<World> ().DEBUG_UpdateNavMesh ();
    }

    protected override void SetResources ()
    {
        resourceIDToGive = 10;
        resourceIDToConsume = -1;

        giveAmount = 1;
        consumeAmount = 0;

        ProductionRequired = 20.0f;        
    }

    // Called from Prop_Profession on built "MaxJobs" times
    protected override void CreateProfessionJobs (int index)
    {
        if (jobIndex == 0)
        {
            Job_Fisherman job = GetComponent<JobEntity> ().CreateJob_Fisherman ( "Fishing", !HaltProduction, 5.0f, null, this, fishingSpot, Job_Fisherman.JobState.Fishing );
            professionJobs.Add ( job );
            JobController.QueueJob ( job );
        }
        else
        {
            Job_Fisherman job = GetComponent<JobEntity> ().CreateJob_Fisherman ( "Fishing", !HaltProduction, 5.0f, null, this, choppingSpot, Job_Fisherman.JobState.Fishing );
            professionJobs.Add ( job );
            JobController.QueueJob ( job );
        }

        jobIndex++;
    }

    protected override void SetInspectable ()
    {
        base.SetInspectable ();
    }

}
