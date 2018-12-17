using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Job_QuarryWorker : Job_Profession {

    private bool givenDestination = false;

    private GameObject rock;

    public enum JobState { Mining, Welling }
    private JobState jobState = JobState.Mining;

    public Job_QuarryWorker (JobEntity entity, string name, bool open, float timeRequired, System.Action onComplete, Prop_Quarry prop, GameObject rock, GameObject wellPoint) : base ( entity, name, open, timeRequired, onComplete, prop )
    {
        this.professionTypes.Add ( ProfessionType.Quarryman );
        this.rock = rock;
    }

    public override void DoJob (float deltaGameTime)
    {
        base.DoJob ( deltaGameTime );

        if (targetInventory.CheckIsFull ( targetProp.resourceIDToGive ))
        {
            OnCharacterLeave ( "Storage is full. Send a market cart!", true );
            IsCompletable = false;
            return;
        }

        if (cBase == null) return;
        if (cBase.gameObject == null) return;

        switch (jobState)
        {
            case JobState.Mining:
                DoJob_Mining ();
                break;
            case JobState.Welling:
                DoJob_Welling ();
                break;
        }
    }

    private void DoJob_Mining ()
    {
        if (!givenDestination)
        {
            givenDestination = true;
            cBase.CitizenMovement.SetDestination ( rock, rock.transform.GetChild ( 0 ).transform.position );
            provideResources = false;
        }

        if (!cBase.CitizenMovement.ReachedPath ()) return;

        if (!provideResources) provideResources = true;

        this.cBase.GetComponent<CitizenGraphics> ().SetUsingAxe ( true, CitizenAnimation.AxeUseAnimation.Splitting );

        LookAtTarget ( rock.transform.position );
    }

    private void DoJob_Welling ()
    {

    }

    public override void OnCharacterLeave (string reason, bool setOpenToTrue)
    {
        if (this.cBase != null)
        {
            this.cBase.CitizenGraphics.SetUsingAxe ( false, CitizenAnimation.AxeUseAnimation.Chopping );
            this.cBase.CitizenGraphics.SetUsingAxe ( false, CitizenAnimation.AxeUseAnimation.Splitting );
        }
        givenDestination = false;

        base.OnCharacterLeave ( reason, setOpenToTrue );
    }

    protected override IEnumerator CheckIsCompletable ()
    {
        if (targetProp.HaltProduction) { IsCompletable = false; yield break; }

        if (targetInventory.CheckIsFull ( targetProp.resourceIDToGive ))
        {
            IsCompletable = false;
            yield break;
        }
        else
        {
            IsCompletable = true;
            yield break;
        }
    }
}
