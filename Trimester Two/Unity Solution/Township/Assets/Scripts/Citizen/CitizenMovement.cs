using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CitizenMovement : MonoBehaviour {

    public CitizenBase cBase { get; protected set; }

    public enum MovementState { Idle, Moving }
    private MovementState movementState = MovementState.Idle;

    public NavMeshAgent agent { get; protected set; }    

    public bool HasPath { get { return agent.hasPath; } }
    public NavMeshPath GetPath { get { return agent.path; } }

    private float agentSpeed = 1.0f;
    private float agentAcceleration = 2.0f;
    private float agentAngularSpeed = 20.0f;

    [SerializeField] [Range ( 0, 2 )] private float agentSpeedModifer = 1;
    [SerializeField] [Range ( 0, 2 )] private float agentAccelerationModifier = 1;
    [SerializeField] [Range ( 0, 2 )] private float agentAngularSpeedModifier = 1;

    public int AvoidancePriority { get; set; }

    //private GameObject destinationObject;
    public GameObject DestinationObject { get; protected set; }

    private Vector3 destinationPosition;

    private System.Action<MovementState> onMovementStateChanged;

    private DEBUG_DrawSnowDepressionsWithMouse snowDepressions;

    private void Awake ()
    {
        cBase = GetComponent<CitizenBase> ();
        agent = GetComponent<NavMeshAgent> ();
        snowDepressions = FindObjectOfType<DEBUG_DrawSnowDepressionsWithMouse> ();
        DestinationObject = null;
        GameTime.RegisterGameTick ( Tick_SetSpeeds );
    }

    void Start () {
        onMovementStateChanged += OnMovementStateChanged;
        SetupAgent ();
        WarpAgentToNavMesh ();
    }

    private void SetupAgent ()
    {
        agentSpeed = agent.speed;
        agentAcceleration = agent.acceleration;
        agentAngularSpeed = agent.angularSpeed;
    }

    private void WarpAgentToNavMesh ()
    {
        RaycastHit hit;

        if (Physics.Raycast ( transform.position, Vector3.down, out hit, 10000, 1 << 9 ))
        {
            agent.Warp ( hit.point );
        }
    }

    private void Tick_SetSpeeds (int relativeTick)
    {
        //snowDepressions.DrawDepression ( 5500, 0.5f, transform.position + (transform.forward * 0.5f) );
        snowDepressions.DrawDepression ( 4500, 0.75f, transform.position + (transform.forward * 0.5f) );

        if (agent.velocity == Vector3.zero)
        {
            if (movementState == MovementState.Moving)
            {                
                movementState = MovementState.Idle;
                agent.avoidancePriority = 100;
                onMovementStateChanged ( movementState );
            }
            return;
        }

        agent.speed = agentSpeed * agentSpeedModifer * GameTime.GameTimeModifier;
        agent.acceleration = agentAcceleration * agentAccelerationModifier * GameTime.GameTimeModifier;
        agent.angularSpeed = agentAngularSpeed * agentAngularSpeedModifier * GameTime.GameTimeModifier;        

        if (movementState == MovementState.Idle)
        {            
            movementState = MovementState.Moving;
            onMovementStateChanged ( movementState );
        }        
    }

    private void OnMovementStateChanged(MovementState currentState)
    {
        if(currentState == MovementState.Idle)
        {
            cBase.CitizenAnimation.ChangeAnimationState ( CitizenAnimation.AnimationState.Idle );           
        }
        else
        {            
            cBase.CitizenAnimation.ChangeAnimationState ( CitizenAnimation.AnimationState.Walking );            
        }
    }

    public void SetDestination(GameObject target, Vector3 targetPosition)
    {
        DestinationObject = target;
        destinationPosition = targetPosition;

        agent.SetDestination ( destinationPosition );

        movementState = MovementState.Moving;
        onMovementStateChanged ( movementState );
        agent.avoidancePriority = AvoidancePriority;
    }

    public void ClearDestination ()
    {
        destinationPosition = Vector3.zero;
        DestinationObject = null;
        agent.ResetPath ();
    }

    public bool ReachedPath ()
    {
        if (agent.pathPending) { return false; }
        if (agent.remainingDistance > agent.stoppingDistance) { return false; }
        if (agent.hasPath) { return false; }
        return true;
    }

    private void OnDestroy ()
    {
        GameTime.UnRegisterGameTick ( Tick_SetSpeeds );
    }
}
