﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class Job {

    //public string Name { get; protected set; }
    //public bool Open { get; protected set; }
    //public bool Complete { get; protected set; }
    //public float TimeRequired { get; protected set; }

    protected int id = -1;
    public int ID { get { return id; } }

    public string Name;
    public bool Complete;
    public float TimeRequired;
    public bool Open { get; protected set; }
    public enum WorkerType { Builder, Gatherer, Farmer }

    protected string agentJobStatus = "";
    public string AgentJobStatus { get { return agentJobStatus; } }

    protected Character character;
    public Character Character { get { return character; } }
    
    public System.Action onComplete;

    public Job () { }

    public Job (string name, bool open, float timeRequired, System.Action onComplete)
    {
        this.id = JobController.GetNewJobID ();
        this.Name = name;
        this.Open = open;
        this.TimeRequired = timeRequired;
        this.onComplete = onComplete;
    }

    // Called from the job controller when a character accepts a job from the job queue.
    public virtual void OnCharacterAccept (Character character)
    {
        this.character = character;
        Debug.Log ( "Character " + this.character.name + " Accepted Job " + Name );
        character.GetComponent<NavMeshAgent> ().ResetPath ();
        Open = false;
    }

    // Usually this will be called from a job when a character is unable to complete the job ( for example, we cant find any warehouses with a specific resources).
    // In this event we should probably move this job up an index in the job queue, so it doesnt keep being assigned to other characters, as we know
    // its current incompletable.
    public virtual void OnCharacterLeave (string reason)
    {
        Debug.Log ( "Character Left Job " + Name + ": " + reason );
        JobController.DecreasePriority ( this );

        if(this.character == null)
        {
            Debug.Log ( this.Name + " has no character assigned" );return;
        }
        this.character.OnJob_Leave ();
        this.character = null;
        this.Open = true;
        this.onComplete = null;
    }

    // Called by the character that this job is assigned to, to invoke the core logic of this job
    public virtual void DoJob (float deltaGameTime) { }    

    // Called by the job itself when the core logic is finished.
    protected virtual void OnComplete ()
    {
        Debug.Log ( "Job Completed " + Name );

        this.character.OnJob_Complete ();
        this.character = null;

        Open = false;
        Complete = true;

        JobController.RemoveJob ( this );
        if (onComplete != null) onComplete ();
    }

    protected float GetPathLength (NavMeshPath path)
    {
        float length = float.MaxValue;
        if (path.status != NavMeshPathStatus.PathComplete) return length;

        for (int i = 1; i < path.corners.Length; i++)
        {
            length += Vector3.Distance ( path.corners[i - 1], path.corners[i] );
        }

        return length;
    }

    protected bool ReachedPath ()
    {
        if (character.agent.pathPending) { return false; }
        if (character.agent.remainingDistance > character.agent.stoppingDistance) { return false; }
        if (character.agent.hasPath) { return false; }
        return true;
    }
}
