using System.Collections.Generic;
using UnityEngine;

public class JobEntity : MonoBehaviour
{
    private List<Job> currentJobs = new List<Job> ();

    public void CreateJob_Build (string name, bool open, float buildSpeed, Buildable buildableTarget)
    {
        Job_Build job = new Job_Build ( this, name, open, buildSpeed, buildableTarget );
        DisplayJobWaitingIcon ();
        job.OnCharacterChanged += CheckJobs;
        currentJobs.Add ( JobController.QueueJob ( job ) );
    }

    public void CreateJob_Haul (string name, bool open, int resourceID, float resourceQuantity, Buildable targetBuildable)
    {
        Job_Haul job = new Job_Haul ( this, name, open, resourceID, resourceQuantity, targetBuildable );
        DisplayJobWaitingIcon ();
        job.OnCharacterChanged += CheckJobs;
        currentJobs.Add ( JobController.QueueJob ( job ) );
    }

    public void CreateJob_GatherResource (string name, bool open, int resourceID, float resourceQuantity, float timeRequired, RawMaterial rawMaterial)
    {
        Job_GatherResource job = new Job_GatherResource ( this, name, open, resourceID, resourceQuantity, timeRequired, rawMaterial );
        DisplayJobWaitingIcon ();
        job.OnCharacterChanged += CheckJobs;
        currentJobs.Add ( JobController.QueueJob ( job ) );
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
            if (currentJobs[i].Character == null)
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
