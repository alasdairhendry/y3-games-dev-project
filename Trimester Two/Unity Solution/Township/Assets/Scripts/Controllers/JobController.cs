﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class JobController
{
    private static List<Job> jobs = new List<Job> ();
    private static int idCount = 0;

    public static Job QueueJob (Job job)
    {
        jobs.Add ( job );
        return job;
    }

    public static Job GetNext (CitizenBase cBase, List<Job> previouslyAttemptedJobs)
    {
        for (int i = 0; i < jobs.Count; i++)
        {
            if (previouslyAttemptedJobs.Contains ( jobs[i] )) { continue; }
            if (!jobs[i].professionTypes.Contains ( cBase.CitizenJob.profession ) && jobs[i].professionTypes.Count > 0) continue;
            if (jobs[i].IdleJob) continue;

            if (jobs[i].Open && jobs[i].IsCompletable)
            {
                Job job = jobs[i];
                job.OnCharacterAccept ( cBase );

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

    public static void DestroyJob (Job job)
    {       
        if (!jobs.Contains ( job )) return;
        
        if (job.cBase != null && !job.Complete)
        {
            job.OnCharacterLeave ( "Job destroyed, the object the job belonged to may have been destroyed." , false);
        }

        job.JobEntity.OnJobRemovedFromQueue ( job );

        jobs.Remove ( job );
    }

    public static void DestroyJobs(List<Job> givenJobs)
    {
        List<Job> targetJobs = new List<Job> ();

        for (int i = 0; i < givenJobs.Count; i++)
        {
            targetJobs.Add ( givenJobs[i] );
        }

        for (int i = 0; i < targetJobs.Count; i++)
        {
            if (!jobs.Contains ( targetJobs[i] )) continue;

            if (targetJobs[i].cBase != null && !targetJobs[i].Complete)
            {
                targetJobs[i].OnCharacterLeave ( "Job destroyed, the object the job belonged to may have been destroyed.", false );
            }

            targetJobs[i].JobEntity.OnJobRemovedFromQueue ( targetJobs[i] );

            jobs.Remove ( targetJobs[i] );
        }
    }

    public static void DecreasePriority (Job job)
    {
        int currentIndex = jobs.IndexOf ( job );

        if (jobs.Count == 1)
        {
            // this is the only job we have, so we cant do anything
        }
        else if (currentIndex >= jobs.Count - 1)
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