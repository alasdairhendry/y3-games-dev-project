using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitizenJob : MonoBehaviour {

    public CitizenBase cBase { get; protected set; }

    private Job currentJob;
    public Job GetCurrentJob { get { return currentJob; } }

    private List<Job> previouslyAttemptedJobs = new List<Job> ();

    public ProfessionType profession { get; protected set; }
    public System.Action<ProfessionType> onProfessionChanged;

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
        if (currentJob == null)
        {    
            if(cBase.CitizenAge.Age < 6)
            {
                GetIdleJob ();
                return;
            }

            //Debug.Log ( "Attempt get normal job" );
            currentJob = JobController.GetNext ( cBase, previouslyAttemptedJobs );

            if (currentJob == null) { GetIdleJob (); }

            GetComponent<CitizenGraphics> ().OnInventoryChanged ( cBase.Inventory );        
        } 
        else if(currentJob != null)
        {
            if (currentJob.IdleJob)
            {                
                Job newJob = JobController.GetNext ( cBase, previouslyAttemptedJobs );

                if (newJob != null) {
                    currentJob.OnCharacterLeave ( "Found a non-idle job", true );
                    currentJob = newJob;
                }

                GetComponent<CitizenGraphics> ().OnInventoryChanged ( cBase.Inventory );
            }
        }

        ClearPreviousJobs ();
    }

    private void GetIdleJob ()
    {
        currentJob = cBase.CitizenIdleJob.GetIdleJob ();        
    }

    public void SwitchToDefaultIdleJob (GameObject senderGO, Type senderType)
    {
        if(currentJob == null)
        {
            Debug.LogError ( "Why are we switching when we have no job? Sent by " + senderGO.name + " - " + senderType );
            return;
        }

        if (!currentJob.IdleJob)
        {
            Debug.LogError ( "Why are we switching when we have no IDLE job? Sent by " + senderGO.name + " - " + senderType );
            return;
        }

        currentJob.OnCharacterLeave ( "Citizen wants to perform default idle job", true );
        currentJob = cBase.CitizenIdleJob.GetDefaultIdleJob ();
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
        if (onProfessionChanged != null) onProfessionChanged ( profession );

        if(currentJob != null)
        {
            if (!currentJob.professionTypes.Contains ( profession ))
            {
                currentJob.OnCharacterLeave ( "Character Changed Profession", true );
            }
        }
    }

    private void OnDestroy ()
    {
        GameTime.UnRegisterGameTick ( OnGameTick );
    }
}
