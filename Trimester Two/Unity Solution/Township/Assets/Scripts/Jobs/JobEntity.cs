using System.Collections.Generic;
using UnityEngine;

public class JobEntity : MonoBehaviour
{
    private List<Job> currentJobs = new List<Job> ();

    public void CreateJob_Build (string name, bool open, float timeRequired, System.Action onComplete, float buildSpeed, Buildable buildableTarget)
    {
        Job_Build job = new Job_Build ( this, name, open, timeRequired, onComplete, buildSpeed, buildableTarget );
        DisplayJobWaitingIcon ();
        job.OnCharacterChanged += CheckJobs;
        currentJobs.Add ( JobController.QueueJob ( job ) );
    }

    public void CreateJob_Haul (string name, bool open, float timeRequired, System.Action onComplete, int resourceID, float resourceQuantity, Prop targetProp, ResourceInventory targetInventory)
    {
        Job_Haul job = new Job_Haul ( this, name, open, timeRequired, onComplete, resourceID, resourceQuantity, targetProp, targetInventory );
        DisplayJobWaitingIcon ();
        job.OnCharacterChanged += CheckJobs;
        currentJobs.Add ( JobController.QueueJob ( job ) );
    }

    public void CreateJob_GatherResource (string name, bool open, float timeRequired, System.Action onComplete, int resourceID, float resourceQuantity, RawMaterial rawMaterial)
    {
        Job_GatherResource job = new Job_GatherResource ( this, name, open, timeRequired, onComplete, resourceID, resourceQuantity, rawMaterial );
        DisplayJobWaitingIcon ();
        job.OnCharacterChanged += CheckJobs;
        currentJobs.Add ( JobController.QueueJob ( job ) );
    }

    public Job_Lumberjack CreateJob_Lumberjack(string name, bool open, float timeRequired, System.Action onComplete, Prop_LumberjackHut prop, List<GameObject> trees, GameObject stump)
    {
        Job_Lumberjack job = new Job_Lumberjack ( this, name, open, timeRequired, onComplete, prop ,trees, stump );
        DisplayJobWaitingIcon ();
        job.OnCharacterChanged += CheckJobs;
        currentJobs.Add ( JobController.QueueJob ( job ) );
        return job;
    }

    public Job_QuarryWorker CreateJob_QuarryWorker (string name, bool open, float timeRequired, System.Action onComplete, Prop_Quarry prop, GameObject rock, GameObject wellPoint)
    {
        Job_QuarryWorker job = new Job_QuarryWorker ( this, name, open, timeRequired, onComplete, prop, rock, wellPoint );
        DisplayJobWaitingIcon ();
        job.OnCharacterChanged += CheckJobs;
        currentJobs.Add ( JobController.QueueJob ( job ) );
        return job;
    }

    public Job_MarketCart CreateJob_MarketCart(string name, bool open, float timeRequired, System.Action onComplete, Prop targetProp, ResourceInventory propInventory, int resourceID, int maxSupplyQuantity, bool supply)
    {
        Job_MarketCart job = new Job_MarketCart ( this, name, open, timeRequired, onComplete, targetProp, propInventory, resourceID, maxSupplyQuantity, supply );
        DisplayJobWaitingIcon ();
        job.OnCharacterChanged += CheckJobs;
        currentJobs.Add ( JobController.QueueJob ( job ) );
        return job;
    }  

    public bool HasNonNullJob ()
    {
        for (int i = 0; i < currentJobs.Count; i++)
        {
            if (!currentJobs[i].IsNull ())
            {
                return true;
            }
        }

        return false;
    }

    public void DestroyJobs ()
    {
        for (int i = 0; i < currentJobs.Count; i++)
        {
            currentJobs[i].OnCharacterChanged -= CheckJobs;
        }

        JobController.DestroyJobs ( currentJobs );
    }

    public void OnJobRemovedFromQueue (Job job)
    {
        if (currentJobs.Contains ( job ))
        {
            job.OnCharacterChanged -= CheckJobs;
            currentJobs.RemoveAt ( currentJobs.IndexOf ( job ) );
        }
    }

    private void CheckJobs ()
    {
        bool foundEmptyJob = false;

        for (int i = 0; i < currentJobs.Count; i++)
        {
            if (currentJobs[i].cBase == null)
            {
                foundEmptyJob = true;
            }
        }

        if (foundEmptyJob)
        {
            DisplayJobWaitingIcon ();
        }
        else
        {
            RemoveJobWaitingIcon ();
        }
    }

    private void DisplayJobWaitingIcon ()
    {
        if (this == null) return;
        GetComponent<IconDisplayer> ().AddIcon ( IconDisplayer.IconType.JobWaiting );
    }

    private void RemoveJobWaitingIcon ()
    {
        if (this == null) return;
        GetComponent<IconDisplayer> ().RemoveIcon ( IconDisplayer.IconType.JobWaiting );
    }
}
