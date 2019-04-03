using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop_Brewery : Prop_Profession {

    [SerializeField] private Transform[] squasherPoints;
    [SerializeField] private Transform[] squasherEntryPoints;
    [SerializeField] private ParticleSystem[] squasherParticles;

    protected override void OnPlaced ()
    {
        base.OnPlaced ();
    }

    protected override void SetResources ()
    {
        resourceIDToGive = 7;
        resourceIDToConsume = 6;

        giveAmount = 1;
        consumeAmount = 3;

        ProductionRequired = 60.0f;
    }

    // Called from Prop_Profession on built "MaxJobs" times
    protected override void CreateProfessionJobs (int index)
    {
        if (index == 0)
        {
            Job_Brewery job = GetComponent<JobEntity> ().CreateJob_Brewery ( "Winemaking", !HaltProduction, 5.0f, null, this, squasherEntryPoints[index], squasherPoints[index], squasherParticles[index] );
            professionJobs.Add ( job );
        }
        else if (index == 1)
        {
            Job_Brewery job = GetComponent<JobEntity> ().CreateJob_Brewery ( "Winemaking", !HaltProduction, 5.0f, null, this, squasherEntryPoints[index], squasherPoints[index], squasherParticles[index] );
            professionJobs.Add ( job );
        }
        else
        {
            Debug.LogError ( "What's going on" );
        }
    }

    protected override void SetInspectable ()
    {
        base.SetInspectable ();
    }
}
