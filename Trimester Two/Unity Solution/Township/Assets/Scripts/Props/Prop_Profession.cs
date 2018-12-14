using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop_Profession : Prop {

    [SerializeField] protected int MaxJobs = 2;
    [SerializeField] protected int UserDefinedNumberJobs = 2;
    public bool HaltProduction { get; protected set; }
    public List<Job> professionJobs { get; protected set; }

    public bool createdSupplyJob = false;
    public bool createdCollectJob = false;

    public override void OnBuilt ()
    {
        professionJobs = new List<Job> ();

        for (int i = 0; i < MaxJobs; i++)
        {
            CreateProfessionJobs (i);
        }
    }

    protected virtual void CreateProfessionJobs(int index)
    {
        
    }

    public virtual void ToggleProduction()
    {
        HaltProduction = !HaltProduction;
        Debug.Log ( "production = " + !HaltProduction );
        OnProductionStatusChanged ();
    }

    public virtual void OnProductionStatusChanged ()
    {
        for (int i = 0; i < professionJobs.Count; i++)
        {
            professionJobs[i].SetOpen ( !HaltProduction );
        }
    }
}
