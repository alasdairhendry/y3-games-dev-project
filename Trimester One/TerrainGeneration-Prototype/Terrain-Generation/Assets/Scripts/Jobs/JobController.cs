using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class JobController {

    private static List<Job> jobs = new List<Job> ();

    public static Job QueueJob (Job job)
    {
        jobs.Add ( job );
        Debug.Log ( "Queued Job " + job.Name );
        return job;
    }

    public static Job GetNext (Character character)
    {
        for (int i = 0; i < jobs.Count; i++)
        {
            if (jobs[i].Open)
            {
                Job job = jobs[i];
                job.OnCharacterAccept ( character );

                //jobs.RemoveAt ( i );
                return job;
            }
        }

        return null;
    }

    public static void RemoveJob (Job job)
    {
        jobs.Remove ( job );
    }

}
