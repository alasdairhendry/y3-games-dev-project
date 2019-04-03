using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop_CharcoalBurnerHut : Prop_Profession {

    [SerializeField] private Transform jobPoint;

    protected override void OnPlaced ()
    {
        base.OnPlaced ();
    }

    protected override void SetResources ()
    {
        resourceIDToGive = 5;
        resourceIDToConsume = 0;

        giveAmount = 2;
        consumeAmount = 1;

        ProductionRequired = 30.0f;
    }

    // Called from Prop_Profession on built "MaxJobs" times
    protected override void CreateProfessionJobs (int index)
    {
        if (index == 0)
        {
            Job_CharcoalBurner job = GetComponent<JobEntity> ().CreateJob_CharcoalBurner ( "Charcoal Burning", !HaltProduction, 5.0f, null, this, jobPoint );
            professionJobs.Add ( job );
        }
    }

    protected override void SetInspectable ()
    {
        base.SetInspectable ();
    }
}
