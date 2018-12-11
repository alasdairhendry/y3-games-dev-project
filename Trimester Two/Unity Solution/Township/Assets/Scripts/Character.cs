using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour {

    private NavMeshAgent agent { get; set; }

    private ResourceInventory inventory;
    public ResourceInventory Inventory { get { return inventory; } set { inventory = value; } }

    private List<Job> previouslyAttemptedJobs = new List<Job> ();
    private bool moveToRandomLocation = false;    

    private Job currentJob;
    public Job GetCurrentJob { get { return currentJob; } }

    private CharacterMovement characterMovement;
    private CharacterGraphics characterGraphics;

    public CharacterMovement CharacterMovement { get { return characterMovement; } }
    public CharacterGraphics CharacterGraphics { get { return characterGraphics; } }

    void Start ()
    {
        GameTime.RegisterGameTick ( OnGameTick );        
        inventory = new ResourceInventory ();
        agent = GetComponent<NavMeshAgent> ();
        characterMovement = GetComponent<CharacterMovement> ();
        characterGraphics = GetComponent<CharacterGraphics> ();
    }

    private void Update ()
    {
        DoJob ();
    }

    private void OnGameTick (int relativeTick)
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

            if (currentJob == null) { Debug.Log ( "Could not accept any jobs" ); OnFailedToGetJob (); }
            GetComponent<CharacterGraphics> ().OnInventoryChanged ( Inventory );
        }
    }

    private void Tick_CheckRandomMovement ()
    {
        if (!moveToRandomLocation) return;
        if (agent == null) return;
        if (!agent.isOnNavMesh) return;

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