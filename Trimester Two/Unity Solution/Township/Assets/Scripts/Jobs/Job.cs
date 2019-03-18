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
    public bool BreakForEnergy { get; protected set; }

    public string AgentJobStatus { get; protected set; }
    public CitizenBase cBase { get; protected set; }
    public JobEntity JobEntity { get; protected set; }

    public System.Action OnCharacterAcceptAction;
    public System.Action OnCharacterLeaveAction;
    public System.Action OnCharacterChanged;
    public System.Action onComplete;

    protected bool isCheckingCompletable = false;

    protected bool citizenReachedPath = false;
    protected Vector3 targetPosition;

    protected bool completableListenersAdded { get; private set; }

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
        this.BreakForEnergy = false;

        Debug.Log ( "Created Job " + Name );

        //GameTime.RegisterGameTick ( Tick_CheckCompletable );
    }

    // Called from the job controller when a character accepts a job from the job queue.
    public virtual void OnCharacterAccept (CitizenBase citizen)
    {
        //GameTime.UnRegisterGameTick ( Tick_CheckCompletable );
        this.cBase = citizen;
        if (this.cBase != null)
        {
            this.cBase.CitizenMovement.onReachedPath += OnCitizenPathComplete;
        }

        Open = false;

        if (completableListenersAdded)
        {
            RemoveCompletableListeners ();       
        }

        if (OnCharacterAcceptAction != null) OnCharacterAcceptAction ();
        if (OnCharacterChanged != null) OnCharacterChanged ();
    }

    // Usually this will be called from a job when a character is unable to complete the job ( for example, we cant find any warehouses with a specific resources).
    // In this event we should probably move this job up an index in the job queue, so it doesnt keep being assigned to other characters, as we know
    // its current incompletable.
    public virtual void OnCharacterLeave (string reason, bool setOpenToTrue, KeyValuePair<bool, string> isCompletable)
    {
        //GameTime.RegisterGameTick ( Tick_CheckCompletable );
        JobController.DecreasePriority ( this );

        if (this.cBase == null)
        {
            Debug.Log ( this.Name + " has no character assigned" ); return;
        }
        else
        {
            this.cBase.CitizenMovement.ClearDestination ();
            this.cBase.CitizenJob.OnJob_Leave ();
            this.cBase.CitizenMovement.onReachedPath -= OnCitizenPathComplete;
            this.cBase = null;
        }

        if (setOpenToTrue)
        {
            this.Open = true;
            Debug.Log ( "Job " + Name + " has been set to open" );
        }
        else
        {
            Debug.Log ( "Job " + Name + " has been set to NOT open" );
        }

        this.IsCompletable = isCompletable.Key;
        this.IsCompletableReason = isCompletable.Value;

        if (!this.IsCompletable)
        {
            if (!completableListenersAdded)
            {
                AddCompletableListeners ();
            }

            Debug.Log ( "Job " + Name + " has been set to NOT completable" );
        }

        if (OnCharacterLeaveAction != null) OnCharacterLeaveAction ();
        if (OnCharacterChanged != null) OnCharacterChanged ();
    }

    // Called by the character that this job is assigned to, to invoke the core logic of this job
    public virtual void DoJob ()
    {

        cBase.CitizenNeeds.NeedsDictionary[Need.Type.Energy].DecreaseValue ();
    }

    // Called by the job itself when the core logic is finished.
    protected virtual void OnComplete ()
    {
        //GameTime.UnRegisterGameTick ( Tick_CheckCompletable );

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

    public virtual void SetOpen (bool open)
    {
        Debug.Log ( "Set job " + Name + " to open - " + open );
        Open = open;

        if (!open)
        {
            OnCharacterLeave ( "Job status changed to not open", false, Job.GetCompletableParams ( Job.CompleteIdentifier.None ) );
        }
    }

    protected virtual void SetIsCompletable ()
    {
        IsCompletable = true;
        IsCompletableReason = "";

        Debug.Log ( "Job " + Name + " has been set to completable" );


        if (completableListenersAdded)
        {
            RemoveCompletableListeners ();
        }
    }

    protected virtual void OnCitizenPathComplete (Vector3 targetDestination)
    {
        if (targetPosition == targetDestination)
        {
            citizenReachedPath = true;
        }
    }

    protected virtual void SetDestination (GameObject destinationObject)
    {
        if (cBase != null)
        {
            cBase.CitizenMovement.SetDestination ( destinationObject, targetPosition );
            citizenReachedPath = false;
        }
    }

    //protected float GetPathLength (NavMeshPath path)
    //{
    //    float length = float.MaxValue;
    //    if (path.status != NavMeshPathStatus.PathComplete) return length;

    //    for (int i = 1; i < path.corners.Length; i++)
    //    {
    //        length += Vector3.Distance ( path.corners[i - 1], path.corners[i] );
    //    }

    //    return length;
    //}

    //protected virtual void Tick_CheckCompletable(int relevantTick)
    //{
    //    if (isCheckingCompletable) return;
    //    if (relevantTick  % 20 != 0) return;
    //    if (cBase != null) return;

    //    GameObject.FindObjectOfType<GameTime> ().StartCoroutine ( CheckIsCompletable () );
    //}

    //protected virtual IEnumerator CheckIsCompletable ()
    //{
    //    yield return null;
    //}

    protected void LookAtTarget (Vector3 target)
    {
        Quaternion lookRot = Quaternion.LookRotation ( target - this.cBase.transform.position, Vector3.up );
        this.cBase.transform.rotation = Quaternion.Slerp ( this.cBase.transform.rotation, lookRot, GameTime.DeltaGameTime * 2.5f );
    }

    public enum CompleteIdentifier { None, NoWarehouses, NotEnoughResources, ResourceDestroyed, PropStorageFull, PropNoConsumables }

    public static KeyValuePair<bool, string> GetCompletableParams (CompleteIdentifier identifier, int resourceID = 0, float resourceQuantity = 0)
    {
        switch (identifier)
        {
            case CompleteIdentifier.None:
                return new KeyValuePair<bool, string> ( true, "" );

            case CompleteIdentifier.NoWarehouses:
                return new KeyValuePair<bool, string> ( false, "No Warehouses Available" );

            case CompleteIdentifier.NotEnoughResources:
                return new KeyValuePair<bool, string> ( false, "Can't find warehouse with " + resourceQuantity.ToString ( "00" ) + " " + ResourceManager.Instance.GetResourceByID ( resourceID ).name );

            case CompleteIdentifier.PropStorageFull:
                return new KeyValuePair<bool, string> ( false, "Storage is full." );

            case CompleteIdentifier.PropNoConsumables:
                return new KeyValuePair<bool, string> ( false, "Not enough " + ResourceManager.Instance.GetResourceByID ( resourceID ).name + "." );

            case CompleteIdentifier.ResourceDestroyed:
                return new KeyValuePair<bool, string> ( false, "Resource was destroyed." );

            default:
                Debug.LogError ( "No identifier found " + identifier.ToString () );
                return new KeyValuePair<bool, string> ( true, "" );
        }
    }

    protected virtual void AddCompletableListeners () { completableListenersAdded = true; }

    protected virtual void RemoveCompletableListeners () { completableListenersAdded = false; }
}
