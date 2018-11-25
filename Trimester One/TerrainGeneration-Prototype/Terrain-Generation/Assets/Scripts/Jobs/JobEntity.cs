using System.Collections.Generic;
using UnityEngine;

public class JobEntity : MonoBehaviour
{
    private List<Job> currentJobs = new List<Job> ();

    public void CreateJob_Build (string name, bool open, float buildSpeed, Buildable buildableTarget)
    {
        Job_Build job = new Job_Build ( this, name, open, buildSpeed, buildableTarget );
        currentJobs.Add ( JobController.QueueJob ( job ) );
    }

    public void CreateJob_Haul (string name, bool open, int resourceID, float resourceQuantity, Buildable targetBuildable)
    {
        Job_Haul job = new Job_Haul ( this, name, open, resourceID, resourceQuantity, targetBuildable );
        currentJobs.Add ( JobController.QueueJob ( job ) );
    }

    public void CreateJob_GatherResource (string name, bool open, int resourceID, float resourceQuantity, float timeRequired, RawMaterial rawMaterial)
    {
        Job_GatherResource job = new Job_GatherResource ( this, name, open, resourceID, resourceQuantity, timeRequired, rawMaterial );
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
        Debug.Log ( "DestroyJobs Count: " + currentJobs.Count );
        //for (int i = 0; i < currentJobs.Count; i++)
        //{
        //    Debug.Log ( "Destroying Job: " + currentJobs[i].Name );
        //    JobController.DestroyJob ( currentJobs[i] );
        //}
        JobController.DestroyJobs ( currentJobs );
    }

    public void OnJobRemovedFromQueue (Job job)
    {
        if (currentJobs.Contains ( job ))
        {
            currentJobs.RemoveAt ( currentJobs.IndexOf ( job ) );
        }
    }
}
