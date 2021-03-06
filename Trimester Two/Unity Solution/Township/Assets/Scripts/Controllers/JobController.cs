﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class JobController
{
    private static List<Job> jobs = new List<Job> ();
    private static List<Job> previousJobs = new List<Job> ();

    private static int idCount = 0;

    public static System.Action<List<Job>, List<Job>> onJobsChanged;

    public static Job QueueJob (Job job)
    {
        job.OnCharacterLeaveAction += OnCharacterLeavesJob;
        jobs.Add ( job );
        onJobsChanged?.Invoke ( jobs, previousJobs );
        return job;
    }

    public static void OnCharacterLeavesJob ()
    {
        onJobsChanged?.Invoke ( jobs, previousJobs );
    }

    // Obtain the next job in the queue, if any.
    public static Job GetNext (CitizenBase cBase, List<Job> previouslyAttemptedJobs)
    {
        // Loop through available jobs
        for (int i = 0; i < jobs.Count; i++)
        {
            // Ensure job was not attempted within recent job checks
            if (previouslyAttemptedJobs.Contains ( jobs[i] )) { continue; }

            // Ensure the job meets the profession of the citizen
            if (!jobs[i].professionTypes.Contains ( cBase.CitizenJob.profession ) && jobs[i].professionTypes.Count > 0) continue;

            // Ensure job is not classed as "Idle" (i.e, dancing, wandering about)
            if (jobs[i].IdleJob) continue;

            // Ensure job is open and can be completed. Completion checked locally by job.
            if (jobs[i].Open && jobs[i].IsCompletable)
            {
                // Job is compatible with citizen, assign it.
                Job job = jobs[i];
                job.OnCharacterAccept ( cBase );
                if (onJobsChanged != null) onJobsChanged ( jobs, previousJobs );

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
            job.OnCharacterLeave ( "Job destroyed, the object the job belonged to may have been destroyed." , false, Job.GetCompletableParams ( Job.CompleteIdentifier.None ) );
        }

        job.JobEntity.OnJobRemovedFromQueue ( job );

        previousJobs.Add ( job );
        jobs.Remove ( job );
        onJobsChanged?.Invoke ( jobs, previousJobs );
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
                targetJobs[i].OnCharacterLeave ( "Job destroyed, the object the job belonged to may have been destroyed.", false, Job.GetCompletableParams ( Job.CompleteIdentifier.None ) );
            }

            targetJobs[i].JobEntity.OnJobRemovedFromQueue ( targetJobs[i] );

            previousJobs.Add ( targetJobs[i] );
            jobs.Remove ( targetJobs[i] );
        }

        onJobsChanged?.Invoke ( jobs, previousJobs );
    }

    public static void IncreasePriority(Job job)
    {
        if (!jobs.Contains ( job )) return;

        int currentIndex = jobs.IndexOf ( job );

        if (jobs.Count == 1)
        {
            Debug.LogError ( "this is the only job we have, so we cant do anything" );
        }
        else if(currentIndex == 0)
        {
            Debug.LogError ( "This job is the highest priority, so don't do anything" );
        }
        else
        {
            Debug.LogError ( "Increase the current jobs index by one, and move the next job below us" );
            jobs.RemoveAt ( currentIndex );
            jobs.Insert ( currentIndex - 1, job );
            onJobsChanged?.Invoke ( jobs, previousJobs );
            //Job nextJob = jobs[currentIndex + 1];
            //jobs[currentIndex] = nextJob;
            //jobs[currentIndex + 1] = job;
        }
    }

    public static void DecreasePriority (Job job)
    {
        if (!jobs.Contains ( job )) return;

        int currentIndex = jobs.IndexOf ( job );

        if (jobs.Count == 1)
        {
            //Debug.LogError ( "this is the only job we have, so we cant do anything" );
        }
        else if (currentIndex >= jobs.Count - 1)
        {
            //Debug.LogError ( "This job is the lowest priority, so don't do anything" );
        }
        else
        {
            //Debug.LogError ( "Increase the current jobs index by one, and move the next job below us" );
            jobs.RemoveAt ( currentIndex );
            jobs.Insert ( currentIndex + 1, job );
            onJobsChanged?.Invoke ( jobs, previousJobs );
        }
    }

    public static void TopPriority (Job job)
    {
        if (!jobs.Contains ( job )) return;

        int currentIndex = jobs.IndexOf ( job );

        if (jobs.Count == 1)
        {
            Debug.LogError ( "this is the only job we have, so we cant do anything" );
        }
        else if (currentIndex == 0)
        {
            Debug.LogError ( "This job is the lowest priority, so don't do anything" );
        }
        else
        {
            Debug.LogError ( "Increase the current jobs index by one, and move the next job below us" );
            jobs.RemoveAt ( currentIndex );
            jobs.Insert ( 0, job );
            onJobsChanged?.Invoke ( jobs, previousJobs );
        }
    }

    public static void LowestPriority (Job job)
    {
        if (!jobs.Contains ( job )) return;

        int currentIndex = jobs.IndexOf ( job );

        if (jobs.Count == 1)
        {
            Debug.LogError ( "this is the only job we have, so we cant do anything" );
        }
        else if (currentIndex >= jobs.Count - 1)
        {
            Debug.LogError ( "This job is the lowest priority, so don't do anything" );
        }
        else
        {
            Debug.LogError ( "Increase the current jobs index by one, and move the next job below us" );
            jobs.RemoveAt ( currentIndex );
            jobs.Insert ( jobs.Count - 1, job );
            onJobsChanged?.Invoke ( jobs, previousJobs );
        }
    }


}
