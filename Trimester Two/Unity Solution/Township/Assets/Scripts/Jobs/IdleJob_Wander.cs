using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IdleJob_Wander : IdleJob {

    private bool givenDestination = false;
    private Vector3 initialLocation;

    public IdleJob_Wander (JobEntity entity, string name)
    {
        this.ID = JobController.GetNewJobID ();
        this.JobEntity = entity;
        this.Name = name;
        this.Open = true;
        this.IdleJob = true;
        this.professionTypes = new List<ProfessionType> ();
    }

    public override void DoJob ()
    {
        base.DoJob ();

        if (!givenDestination)
        {
            if (cBase == null) return;
            if (cBase.CitizenMovement == null) return;

            //givenDestination = true;

            Vector2 ranPos = Random.insideUnitCircle * 20.0f;
            Vector3 newPos = new Vector3 ( ranPos.x, 0.0f, ranPos.y ) + cBase.transform.position;

            //GameObject go = new GameObject ( "newPos" );
            //go.transform.position = newPos;

            NavMeshHit hit;
            //Debug.Log ( NavMesh.GetAreaFromName ( "Walkable" ) );
            bool found = NavMesh.SamplePosition ( newPos, out hit, 20.0f, 1 );

            if (!found)
            {
                OnCharacterLeave ( "Couldn't find suitable area", true, Job.GetCompletableParams ( CompleteIdentifier.None ) );
                Debug.Log ( "Couldnt find wander route" );
                return;
            }
            else
            {
                givenDestination = true;

                Vector3 finalPosition = hit.position;

                targetPosition = finalPosition;
                SetDestination ( null );

                AgentJobStatus = "Wandering";
            }
        }

        if (!citizenReachedPath) return;

        OnCharacterLeave ( "Completed Wander", true, Job.GetCompletableParams ( CompleteIdentifier.None ) );
        //givenDestination = false;

    }

    public override void OnCharacterAccept (CitizenBase character)
    {
        base.OnCharacterAccept ( character );
        initialLocation = character.transform.position;
    }

    public override void OnCharacterLeave (string reason, bool setOpenToTrue, KeyValuePair<bool, string> isCompletable)
    {
        //cBase.CitizenAnimation.animator.SetBool ( "Dance", false );
        //toldToDance = false;
        givenDestination = false;

        base.OnCharacterLeave ( reason, setOpenToTrue, isCompletable );
    }
}
