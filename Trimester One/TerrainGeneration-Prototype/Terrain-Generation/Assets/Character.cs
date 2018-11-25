using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour {

    public NavMeshAgent agent { get; protected set; }

    private ResourceInventory inventory;
    public ResourceInventory Inventory { get { return inventory; } set { GetComponent<CharacterGraphics> ().OnInventoryChanged ( value ); inventory = value; } }

    private List<Job> previouslyAttemptedJobs = new List<Job> ();
    private bool moveToRandomLocation = false;    

    private Job currentJob;
    public Job GetCurrentJob { get { return currentJob; } }

    void Start ()
    {
        GameTime.RegisterGameTick ( OnGameTick );        
        inventory = new ResourceInventory ();
        agent = GetComponent<NavMeshAgent> ();
    }

    private void Update ()
    {
        DoJob ();
    }

    private void OnGameTick ()
    {
        Tick_CheckJob ();
        Tick_CheckRandomMovement ();
    }

    private void DoJob ()
    {
        if (currentJob != null)
            currentJob.DoJob ( GameTime.DeltaGameTime );
    }

    private void Tick_CheckJob ()
    {
        if (currentJob == null && !moveToRandomLocation)
        {
            currentJob = JobController.GetNext ( this, previouslyAttemptedJobs );

            if (currentJob == null) { Debug.Log ( "Could not accept any jobs" ); OnFailedToGetJob (); return; }            
        }
    }

    private void Tick_CheckRandomMovement ()
    {
        if (!moveToRandomLocation) return;
        if (agent == null) return;

        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {                
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {                    
                    previouslyAttemptedJobs.Clear ();
                    moveToRandomLocation = false;
                }
            }
        }
    }
    
    private void OnFailedToGetJob ()
    {
        Vector3 direction = new Vector3 ( Random.Range(-1.0f, 1.0f), 0.0f, Random.Range ( -1.0f, 1.0f ) ).normalized;        
        agent.SetDestination ( transform.position + (direction * Random.Range ( 6.0f, 10.0f )) );

        moveToRandomLocation = true;
    }

    public void OnJob_Complete ()
    {
        currentJob = null;
    }

    public void OnJob_Leave ()
    {
        if (currentJob == null) return;

        if (!previouslyAttemptedJobs.Contains ( currentJob ))
            previouslyAttemptedJobs.Add ( currentJob );

        currentJob = null;
    }

    private void OnDestroy ()
    {
        GameTime.UnRegisterGameTick ( OnGameTick );
    }
}