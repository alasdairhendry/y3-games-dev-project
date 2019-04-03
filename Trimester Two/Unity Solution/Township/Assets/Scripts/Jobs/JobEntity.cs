using System.Collections.Generic;
using UnityEngine;

public class JobEntity : MonoBehaviour
{
    private List<Job> currentJobs = new List<Job> ();
    public WorldEntity worldEntity { get; protected set; }

    private void Awake()
    {
        this.worldEntity = GetComponent<WorldEntity> ();
    }

    public void CreateJob_Build (string name, bool open, float timeRequired, System.Action onComplete, float buildSpeed, Buildable buildableTarget)
    {
        Job_Build job = new Job_Build ( this, name, open, timeRequired, onComplete, buildSpeed, buildableTarget );
        //DisplayJobWaitingIcon ();
        job.OnCharacterChanged += CheckJobs;
        job.onComplete += CheckJobs;
        currentJobs.Add ( JobController.QueueJob ( job ) );
        CheckJobs ();
    }

    public void CreateJob_Haul (string name, bool open, float timeRequired, System.Action onComplete, int resourceID, float resourceQuantity, Prop targetProp, ResourceInventory targetInventory)
    {
        Job_Haul job = new Job_Haul ( this, name, open, timeRequired, onComplete, resourceID, resourceQuantity, targetProp, targetInventory );
        //DisplayJobWaitingIcon ();
        job.OnCharacterChanged += CheckJobs;
        job.onComplete += CheckJobs;
        currentJobs.Add ( JobController.QueueJob ( job ) );
        CheckJobs ();
    }

    public void CreateJob_GatherResource (string name, bool open, float timeRequired, System.Action onComplete, int resourceID, float resourceQuantity, RawMaterial rawMaterial)
    {
        Job_GatherResource job = new Job_GatherResource ( this, name, open, timeRequired, onComplete, resourceID, resourceQuantity, rawMaterial );
        //DisplayJobWaitingIcon ();
        job.OnCharacterChanged += CheckJobs;
        job.onComplete += CheckJobs;
        currentJobs.Add ( JobController.QueueJob ( job ) );
        CheckJobs ();
    }

    public Job_Lumberjack CreateJob_Lumberjack (string name, bool open, float timeRequired, System.Action onComplete, Prop_LumberjackHut prop, List<GameObject> trees, GameObject stump)
    {
        Job_Lumberjack job = new Job_Lumberjack ( this, name, open, timeRequired, onComplete, prop, trees, stump );
        //DisplayJobWaitingIcon ();
        job.OnCharacterChanged += CheckJobs;
        job.onComplete += CheckJobs;
        currentJobs.Add ( JobController.QueueJob ( job ) );
        CheckJobs ();
        return job;
    }

    public Job_Fisherman CreateJob_Fisherman (string name, bool open, float timeRequired, System.Action onComplete, Prop_FishingHut prop, GameObject target, Job_Fisherman.JobState state)
    {
        Job_Fisherman job = new Job_Fisherman ( this, name, open, timeRequired, onComplete, prop, target, state );
        //DisplayJobWaitingIcon ();
        job.OnCharacterChanged += CheckJobs;
        job.onComplete += CheckJobs;
        currentJobs.Add ( JobController.QueueJob ( job ) );
        CheckJobs ();
        return job;
    }

    public Job_Vintner CreateJob_Vintner (string name, bool open, float timeRequired, System.Action onComplete, Prop_Orchard prop, List<Transform> waypoints)
    {
        Job_Vintner job = new Job_Vintner ( this, name, open, timeRequired, onComplete, prop, waypoints );
        //DisplayJobWaitingIcon ();
        job.OnCharacterChanged += CheckJobs;
        job.onComplete += CheckJobs;
        currentJobs.Add ( JobController.QueueJob ( job ) );
        CheckJobs ();
        return job;
    }

    public Job_QuarryWorker CreateJob_QuarryWorker (string name, bool open, float timeRequired, System.Action onComplete, Prop_Quarry prop, GameObject rock, GameObject wellPoint)
    {
        Job_QuarryWorker job = new Job_QuarryWorker ( this, name, open, timeRequired, onComplete, prop, rock, wellPoint );
        //DisplayJobWaitingIcon ();
        job.OnCharacterChanged += CheckJobs;
        job.onComplete += CheckJobs;
        currentJobs.Add ( JobController.QueueJob ( job ) );
        CheckJobs ();
        return job;
    }

    public Job_Stonemason CreateJob_StoneMason(string name, bool open, float timeRequired, System.Action onComplete, Prop_StonemasonHut prop, GameObject plinth, GameObject plinthPoint)
    {
        Job_Stonemason job = new Job_Stonemason ( this, name, open, timeRequired, onComplete, prop, plinth, plinthPoint );
        //DisplayJobWaitingIcon ();
        job.OnCharacterChanged += CheckJobs;
        job.onComplete += CheckJobs;
        currentJobs.Add ( JobController.QueueJob ( job ) );
        CheckJobs ();
        return job;
    }

    public Job_Blacksmith CreateJob_Blacksmith (string name, bool open, float timeRequired, System.Action onComplete, Prop_Smithery prop, GameObject smithPoint, GameObject anvilPoint)
    {
        Job_Blacksmith job = new Job_Blacksmith ( this, name, open, timeRequired, onComplete, prop, smithPoint, anvilPoint );
        job.OnCharacterChanged += CheckJobs;
        job.onComplete += CheckJobs;
        currentJobs.Add ( JobController.QueueJob ( job ) );
        CheckJobs ();
        return job;
    }

    public Job_Miner CreateJob_Miner (string name, bool open, float timeRequired, System.Action onComplete, Prop_Mine prop, Transform jobPoint)
    {
        Job_Miner job = new Job_Miner ( this, name, open, timeRequired, onComplete, prop, jobPoint );
        job.OnCharacterChanged += CheckJobs;
        job.onComplete += CheckJobs;
        currentJobs.Add ( JobController.QueueJob ( job ) );
        CheckJobs ();
        return job;
    }

    public Job_CharcoalBurner CreateJob_CharcoalBurner (string name, bool open, float timeRequired, System.Action onComplete, Prop_CharcoalBurnerHut prop, Transform jobPoint)
    {
        Job_CharcoalBurner job = new Job_CharcoalBurner ( this, name, open, timeRequired, onComplete, prop, jobPoint );
        job.OnCharacterChanged += CheckJobs;
        job.onComplete += CheckJobs;
        currentJobs.Add ( JobController.QueueJob ( job ) );
        CheckJobs ();
        return job;
    }

    public Job_Brewery CreateJob_Brewery (string name, bool open, float timeRequired, System.Action onComplete, Prop_Brewery prop, Transform entryPoint, Transform squashPoint, ParticleSystem particles)
    {
        Job_Brewery job = new Job_Brewery ( this, name, open, timeRequired, onComplete, prop, entryPoint, squashPoint, particles );
        job.OnCharacterChanged += CheckJobs;
        job.onComplete += CheckJobs;
        currentJobs.Add ( JobController.QueueJob ( job ) );
        CheckJobs ();
        return job;
    }

    public Job_MarketCart CreateJob_MarketCart(string name, bool open, float timeRequired, System.Action onComplete, Prop targetProp, ResourceInventory propInventory, int resourceID, int maxSupplyQuantity, bool supply)
    {
        Job_MarketCart job = new Job_MarketCart ( this, name, open, timeRequired, onComplete, targetProp, propInventory, resourceID, maxSupplyQuantity, supply );
        //DisplayJobWaitingIcon ();
        job.OnCharacterChanged += CheckJobs;
        job.onComplete += CheckJobs;
        currentJobs.Add ( JobController.QueueJob ( job ) );
        CheckJobs ();
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
            currentJobs[i].onComplete -= CheckJobs;
        }

        JobController.DestroyJobs ( currentJobs );
        CheckJobs ();
    }

    public void OnJobRemovedFromQueue (Job job)
    {
        if (currentJobs.Contains ( job ))
        {
            job.OnCharacterChanged -= CheckJobs;
            job.onComplete -= CheckJobs;
            currentJobs.RemoveAt ( currentJobs.IndexOf ( job ) );

            CheckJobs ();
        }
    }

    private bool displayingJobWaiting = false;

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
        if (displayingJobWaiting) return;

        //Debug.Log ( "DisplayJobWaitingIcon" );
        GetComponent<IconDisplayer> ().AddIconGeneric ( IconDisplayer.IconType.JobWaiting );
        displayingJobWaiting = true;
    }

    private void RemoveJobWaitingIcon ()
    {
        if (this == null) return;
        if (!displayingJobWaiting) return;
        //Debug.Log ( "RemoveJobWaitingIcon" );
        GetComponent<IconDisplayer> ().RemoveIconByType ( IconDisplayer.IconType.JobWaiting );
        displayingJobWaiting = false;
    }
}
