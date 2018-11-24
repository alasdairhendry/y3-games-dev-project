using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class JobController {

    private static List<Job> jobs = new List<Job> ();
    private static int idCount = 0;

    public static Job QueueJob (Job job)
    {
        jobs.Add ( job );        
        Debug.Log ( "Queued Job " + job.ID );
        return job;
    }

    public static Job GetNext (Character character, List<Job> previouslyAttemptedJobs)
    {
        for (int i = 0; i < jobs.Count; i++)
        {
            if (previouslyAttemptedJobs.Contains ( jobs[i] )) { continue; }

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

    public static int GetNewJobID ()
    {
        idCount++;
        return idCount;
    }

    public static void RemoveJob (Job job)
    {
        if (!jobs.Contains ( job )) return;

        if(job.Character != null && !job.Complete)
        {
            job.OnCharacterLeave ( "Job destroyed" );
        }

        jobs.Remove ( job );
    }

    public static void DecreasePriority(Job job)
    {
        int currentIndex = jobs.IndexOf ( job );

        if(jobs.Count == 1)
        {
            // this is the only job we have, so we cant do anything
        }
        else if(currentIndex >= jobs.Count - 1)
        {
            // This job is the lowest priority, so don't do anything.
        }
        else
        {
            // Increase the current jobs index by one, and move the next job below us.
            Job nextJob = jobs[currentIndex + 1];
            jobs[currentIndex] = nextJob;
            jobs[currentIndex + 1] = job;
        }
    }
}
