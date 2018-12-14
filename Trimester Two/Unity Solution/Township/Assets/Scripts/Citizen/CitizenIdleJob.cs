using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitizenIdleJob : MonoBehaviour {

    public CitizenBase cBase { get; protected set; }

    private List<IdleJob> idleJobs = new List<IdleJob> ();
    private List<IdleJob> eligibleJobs = new List<IdleJob> ();

    private void Awake ()
    {
        cBase = GetComponent<CitizenBase> ();
        CreateJobs ();
        cBase.CitizenJob.onProfessionChanged += (profession) => { CheckEligibleJobs (); };
    }

    private void CreateJobs ()
    {
        //idleJobs.Add ( new IdleJob_MoveTo ( null, "Be Lazy" ) );
        idleJobs.Add ( new IdleJob_StandByCampfire ( null, "Warm up" ) );
        idleJobs.Add ( new IdleJob_Dance ( null, "Dance The YMCA" ) );
    }

    private void CheckEligibleJobs ()
    {
        eligibleJobs.Clear ();

        for (int i = 0; i < idleJobs.Count; i++)
        {
            if (!idleJobs[i].professionTypes.Contains ( cBase.CitizenJob.profession ) && idleJobs[i].professionTypes.Count > 0) { Debug.Log ( "Profession invalid" ); continue; }
            if (!idleJobs[i].IdleJob) { Debug.Log ( "Isnt idle job" ); continue; }

            eligibleJobs.Add ( idleJobs[i] );
        }
    }

    public Job GetDefaultIdleJob ()
    {
        if (eligibleJobs.Count <= 0) { Debug.LogError ( "Eligible Idle Jobs are 0" ); return null; }

        Job job = eligibleJobs[0];
        job.OnCharacterAccept ( cBase );

        return job;
    }

    public Job GetIdleJob ()
    {
        if(eligibleJobs.Count <= 0) { Debug.LogError ( "Eligible Idle Jobs are 0" ); return null; }

        Job job = eligibleJobs[Random.Range(0, eligibleJobs.Count)];

        job.OnCharacterAccept ( cBase );

        return job;
    }
}
