using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitizenJob : MonoBehaviour {

    public CitizenBase cBase { get; protected set; }

    private Job currentJob;
    public Job GetCurrentJob { get { return currentJob; } }

    private List<Job> previouslyAttemptedJobs = new List<Job> ();

    private bool allowWork = true;

    public ProfessionType profession { get; protected set; }
    public System.Action<ProfessionType> onProfessionChanged;

    public bool RegainingEnergy { get; protected set; }

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
        CheckEnergy ();
        Tick_CheckJob ();
    }

    private void CheckEnergy ()
    {
        if (cBase.CitizenNeeds.NeedsDictionary[Need.Type.Energy].currentValue <= 0.30f)
        {
            if (currentJob != null)
            {
                if (!currentJob.IdleJob)
                {
                    currentJob.OnCharacterLeave ( "Character has no energy left", true );
                    RegainingEnergy = true;
                }
            }
        }
        else if (cBase.CitizenNeeds.NeedsDictionary[Need.Type.Energy].currentValue >= 1.0f && RegainingEnergy)
        {
            RegainingEnergy = false;
        }
    }

    private void DoJob ()
    {
        if (currentJob != null)
            currentJob.DoJob ();
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

            if (ShouldSeekNonIdle ())
            {
                currentJob = JobController.GetNext ( cBase, previouslyAttemptedJobs );
                if (currentJob == null) { GetIdleJob (); }
            }
            else
            {
                GetIdleJob ();
            }

            GetComponent<CitizenGraphics> ().OnInventoryChanged ( cBase.Inventory );        
        } 
        else if(currentJob != null)
        {
            if (currentJob.IdleJob && ShouldSeekNonIdle())
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

    private bool ShouldSeekNonIdle ()
    {
        if (cBase.CitizenNeeds.NeedsDictionary[Need.Type.Energy].currentValue <= 0.30f)
        {
            return false;
        }

        if (!allowWork)
        {
            return false;
        }

        if (RegainingEnergy)
        {
            return false;
        }

        return true;
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
