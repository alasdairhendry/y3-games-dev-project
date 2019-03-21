using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop_Orchard : Prop_Profession {

    [SerializeField] private List<Transform> waypointsOne;
    [SerializeField] private List<Transform> waypointsTwo;

    private int jobCounter = 0;

    protected override void OnPlaced ()
    {
        base.OnPlaced ();
    }

    protected override void SetResources ()
    {
        resourceIDToGive = 6;
        resourceIDToConsume = -1;

        giveAmount = 1;
        consumeAmount = 0;

        ProductionRequired = 20.0f;
    }

    // Called from Prop_Profession on built "MaxJobs" times
    protected override void CreateProfessionJobs (int index)
    {
        if (index == 0)
        {
            Job_Vintner job = GetComponent<JobEntity> ().CreateJob_Vintner ( "Vineyard Work", !HaltProduction, 5.0f, null, this, waypointsOne );
            professionJobs.Add ( job );
        }
        else
        {
            Job_Vintner job = GetComponent<JobEntity> ().CreateJob_Vintner ( "Vineyard Work", !HaltProduction, 5.0f, null, this, waypointsTwo );
            professionJobs.Add ( job );

        }
    }

    protected override void SetInspectable ()
    {
        base.SetInspectable ();
    }

}
