using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop_Mine : Prop_Profession
{
    [SerializeField] private List<Transform> jobPoints = new List<Transform> ();

    protected override void OnPlaced ()
    {
        base.OnPlaced ();
    }

    protected override void SetResources ()
    {
        resourceIDToGive = 3;
        resourceIDToConsume = -1;

        giveAmount = 1;
        consumeAmount = -1;

        ProductionRequired = 150.0f;
    }

    // Called from Prop_Profession on built "MaxJobs" times
    protected override void CreateProfessionJobs (int index)
    {
        if (index >= jobPoints.Count)
        {
            Debug.LogError ( "Too many jobs for points" );
            return;
        }

        Job_Miner job = GetComponent<JobEntity> ().CreateJob_Miner ( "Mining", !HaltProduction, 5.0f, null, this, jobPoints[index] );
        professionJobs.Add ( job );
    }

    protected override void SetInspectable ()
    {
        base.SetInspectable ();
    }
}
