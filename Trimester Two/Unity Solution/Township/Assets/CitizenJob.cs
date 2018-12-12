using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitizenJob : MonoBehaviour {

    public CitizenBase cBase { get; protected set; }

    private Job currentJob;
    public Job GetCurrentJob { get { return currentJob; } }

    private List<Job> previouslyAttemptedJobs = new List<Job> ();

    public ProfessionType profession { get; protected set; }

    private void Awake ()
    {
        cBase = GetComponent<CitizenBase> ();
    }

    void Start ()
    {
        GameTime.RegisterGameTick ( OnGameTick );
    }

    private void Update ()
    {
        DoJob ();
    }

    private void OnGameTick (int relativeTick)
    {
        Tick_CheckJob ();        
    }

    private void DoJob ()
    {
        if (currentJob != null)
            currentJob.DoJob ( GameTime.DeltaGameTime );
    }

    private void Tick_CheckJob ()
    {        
        if (currentJob == null && !cBase.MoveToRandomLocation)
        {        
            currentJob = JobController.GetNext ( cBase, previouslyAttemptedJobs );

            if (currentJob == null) { OnFailedToGetJob (); }
            GetComponent<CitizenGraphics> ().OnInventoryChanged ( cBase.Inventory );        
        }
    }

    private void OnFailedToGetJob ()
    {
        Vector3 direction = new Vector3 ( Random.Range ( -1.0f, 1.0f ), 0.0f, Random.Range ( -1.0f, 1.0f ) ).normalized;
        cBase.CitizenMovement.SetDestination ( null, transform.position + (direction * Random.Range ( 6.0f, 10.0f )) );

        cBase.MoveToRandomLocation = true;
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

    public void ClearPreviousJobs ()
    {
        previouslyAttemptedJobs.Clear ();
    }

    public void UpdateProfession(ProfessionType profession)
    {
        this.profession = profession;
    }

    private void OnDestroy ()
    {
        GameTime.UnRegisterGameTick ( OnGameTick );
    }
}
