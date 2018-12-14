using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class Job {
    
    public string Name { get; protected set; }
    public int ID { get; protected set; }
    public bool IdleJob { get; protected set; }
    public bool Open { get; protected set; } 
    public bool IsCompletable { get; protected set; }
    public string IsCompletableReason { get; protected set; }
    public float TimeRequired { get; protected set; }
    public bool Complete { get; protected set; }
    public List<ProfessionType> professionTypes { get; protected set; }

    public string AgentJobStatus { get; protected set; }
    public CitizenBase cBase { get; protected set; }
    public JobEntity JobEntity { get; protected set; }

    public System.Action OnCharacterAcceptAction;
    public System.Action OnCharacterLeaveAction;
    public System.Action OnCharacterChanged;
    public System.Action onComplete;

    protected bool isCheckingCompletable = false;

    public Job () { }

    public Job (JobEntity entity, string name, bool open, float timeRequired, System.Action onComplete)
    {
        this.ID = JobController.GetNewJobID ();
        this.JobEntity = entity;
        this.Name = name;
        this.Open = open;
        this.TimeRequired = timeRequired;
        this.onComplete = onComplete;
        this.IsCompletableReason = "";
        this.IsCompletable = true;
        this.professionTypes = new List<ProfessionType> ();

        GameTime.RegisterGameTick ( Tick_CheckCompletable );
    }

    // Called from the job controller when a character accepts a job from the job queue.
    public virtual void OnCharacterAccept (CitizenBase character)
    {
        GameTime.UnRegisterGameTick ( Tick_CheckCompletable );
        this.cBase = character;
        if (this.cBase != null)
            if (character.GetComponent<NavMeshAgent> ().isOnNavMesh)
                character.GetComponent<NavMeshAgent> ().ResetPath ();

        Open = false;
        if (OnCharacterAcceptAction != null) OnCharacterAcceptAction ();
        if (OnCharacterChanged != null) OnCharacterChanged ();
    }

    // Usually this will be called from a job when a character is unable to complete the job ( for example, we cant find any warehouses with a specific resources).
    // In this event we should probably move this job up an index in the job queue, so it doesnt keep being assigned to other characters, as we know
    // its current incompletable.
    public virtual void OnCharacterLeave (string reason, bool setOpenToTrue)
    {
        GameTime.RegisterGameTick ( Tick_CheckCompletable );
        JobController.DecreasePriority ( this );

        if(this.cBase == null)
        {
            Debug.Log ( this.Name + " has no character assigned" );return;
        }
        else 
        {
            this.cBase.CitizenMovement.ClearDestination ();
            this.cBase.CitizenJob.OnJob_Leave ();
            this.cBase = null;
        }

        if (setOpenToTrue)
            this.Open = true;
        
        if (OnCharacterLeaveAction != null) OnCharacterLeaveAction ();
        if (OnCharacterChanged != null) OnCharacterChanged ();
    }

    // Called by the character that this job is assigned to, to invoke the core logic of this job
    public virtual void DoJob (float deltaGameTime) { }    

    // Called by the job itself when the core logic is finished.
    protected virtual void OnComplete ()
    {
        GameTime.UnRegisterGameTick ( Tick_CheckCompletable );

        if (this.cBase != null)
        {
            this.cBase.CitizenMovement.ClearDestination ();
            this.cBase.CitizenJob.OnJob_Complete ();
            this.cBase = null;
        }

        Open = false;
        Complete = true;

        if (onComplete != null) onComplete ();
        JobController.DestroyJob ( this );
    }

    public virtual void SetOpen(bool open)
    {
        Debug.Log ( "Set job " + Name + " to open - " + open );
        Open = open;

        if (!open)
        {
            OnCharacterLeave ( "Job status changed to not open" , false);
        }
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

    protected virtual void Tick_CheckCompletable(int relevantTick)
    {
        if (isCheckingCompletable) return;
        if (relevantTick  % 20 != 0) return;
        if (cBase != null) return;

        GameObject.FindObjectOfType<GameTime> ().StartCoroutine ( CheckIsCompletable () );
    }

    protected virtual IEnumerator CheckIsCompletable ()
    {
        yield return null;
    }
}
