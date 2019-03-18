using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IdleJob_StandByCampfire : IdleJob
{
    private bool toldToAnimate = false;
    private bool givenPosition = false;
    GameObject targetCampfire;

    public IdleJob_StandByCampfire (JobEntity entity, string name)
    {
        this.ID = JobController.GetNewJobID ();
        this.JobEntity = entity;
        this.Name = name;
        this.Open = true;
        this.IdleJob = true;
        this.professionTypes = new List<ProfessionType> ();
        this.averageIdleTime = 25.0f;
    }

    public override void DoJob ()
    {
        base.DoJob ();

        if (cBase == null) { OnCharacterLeave ( "Citizen is null", true, GetCompletableParams ( CompleteIdentifier.None ) ); return; }

        if (!givenPosition)
        {
            List<GameObject> campfires = EntityManager.Instance.GetEntitiesByType ( typeof ( Prop_Campfire ) );

            if (campfires == null)
            {
                OnCharacterLeave ( "No campfires", true , GetCompletableParams ( CompleteIdentifier.None ) );
                return;
            }

            for (int i = 0; i < campfires.Count; i++)
            {
                Prop_Campfire campfire = campfires[i].GetComponent<Prop_Campfire> ();
                if (campfire == null) campfires.RemoveAt ( i );
                if (campfire.buildable == null) campfires.RemoveAt ( i );
                if (campfire.buildable.IsComplete == false) campfires.RemoveAt ( i );
            }

            if (campfires.Count <= 0)
            {
                OnCharacterLeave ( "No campfires", true, GetCompletableParams ( CompleteIdentifier.None ) );
                return;
            }

            campfires.OrderBy ( x => Vector3.Distance ( x.transform.position, this.cBase.transform.position ) );

            targetCampfire = campfires[0];
            targetPosition = targetCampfire.transform.position;

            Vector3 offsetPosition = cBase.GetRandomNavMeshCirclePosition ( targetPosition, 2.5f );

            if(offsetPosition == targetPosition)
            {
                Debug.Log ( "Returned same - returning" );
                return;
            }

            targetPosition = offsetPosition;
            SetDestination ( campfires[0] );

            givenPosition = true;
            AgentJobStatus = "Walking To Campfire";
            return;
        }

        if(targetCampfire == null) { OnCharacterLeave ( "Campfire was destroyed", true, Job.GetCompletableParams ( Job.CompleteIdentifier.None ) ); return; }

        if (citizenReachedPath)
        {
            if (!toldToAnimate)
            {
                if (cBase == null) return;
                cBase.CitizenAnimation.animator.SetBool ( "KneelByFire", true );
                AgentJobStatus = "Warming Hands";
            }

            Quaternion lookRot = Quaternion.LookRotation ( this.targetPosition - this.cBase.transform.position, Vector3.up );
            this.cBase.transform.rotation = Quaternion.Slerp ( this.cBase.transform.rotation, lookRot, GameTime.DeltaGameTime * 2.5f );
        }
    }

    public override void OnCharacterAccept (CitizenBase citizen)
    {
        base.OnCharacterAccept ( citizen );       
    }

    public override void OnCharacterLeave (string reason, bool setOpenToTrue, KeyValuePair<bool, string> isCompletable)
    {
        if (cBase != null)
            cBase.CitizenAnimation.animator.SetBool ( "KneelByFire", false );
        toldToAnimate = false;
        givenPosition = false;
        targetPosition = new Vector3 ();
        targetCampfire = null;

        base.OnCharacterLeave ( reason, setOpenToTrue, isCompletable);
    }
}
