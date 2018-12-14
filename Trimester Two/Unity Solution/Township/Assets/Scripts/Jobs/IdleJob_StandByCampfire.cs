using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IdleJob_StandByCampfire : IdleJob
{

    private bool toldToAnimate = false;
    private bool givenPosition = false;
    private Vector3 targetPosition = new Vector3 ();
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

    public override void DoJob (float deltaGameTime)
    {
        base.DoJob ( deltaGameTime );

        if (cBase == null) { OnCharacterLeave ( "Citizen is null", true ); return; }

        if (!givenPosition)
        {
            List<GameObject> campfires = PropManager.Instance.GetWorldPropsByType ( typeof ( Prop_Campfire ) );

            if (campfires == null)
            {
                OnCharacterLeave ( "No campfires", true );
                return;
            }

            if (campfires.Count <= 0)
            {
                OnCharacterLeave ( "No campfires", true );
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

            cBase.CitizenMovement.SetDestination (campfires[0], offsetPosition );

            givenPosition = true;
            AgentJobStatus = "Walking To Campfire";
            return;
        }

        if(targetCampfire == null) { OnCharacterLeave ( "Campfire was destroyed", true ); return; }

        if (cBase.CitizenMovement.ReachedPath ())
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

    public override void OnCharacterLeave (string reason, bool setOpenToTrue)
    {
        if (cBase != null)
            cBase.CitizenAnimation.animator.SetBool ( "KneelByFire", false );
        toldToAnimate = false;
        givenPosition = false;
        targetPosition = new Vector3 ();
        targetCampfire = null;

        base.OnCharacterLeave ( reason, setOpenToTrue);
    }
}
