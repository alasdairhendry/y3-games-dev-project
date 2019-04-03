using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CitizenMovement : MonoBehaviour {

    public CitizenBase cBase { get; protected set; }
    private NavMeshAgent agent;

    public enum MovementState { Idle, Moving }
    private MovementState movementState = MovementState.Idle;

    private float agentSpeed = 0.0f;
    private float agentAngularSpeed = 0.0f;
    private float agentAcceleration = 0.0f;
    public int AvoidancePriority { get; set; }
    
    [SerializeField] [Range ( 0, 2 )] private float agentSpeedModifer = 1;
    [SerializeField] [Range ( 0, 2 )] private float agentAngularSpeedModifier = 1;
    [SerializeField] [Range ( 0, 2 )] private float agentAccelerationModifier = 1;

    public GameObject DestinationObject { get; protected set; }
    private Vector3 destinationPosition;

    private NavMeshPath path;
    public NavMeshPath Path { get { return path; } }
    public NavMeshPath GetAgentPath { get { return agent.path; } }
    public bool HasPath { get; protected set; }

    public System.Action<Vector3> onReachedPath;
    public System.Action onDestinationCleared;
    public System.Action onPathConfirmed;
    private System.Action<MovementState> onMovementStateChanged;

    private void Awake ()
    {
        cBase = GetComponent<CitizenBase> ();
        agent = GetComponent<NavMeshAgent> ();
        DestinationObject = null;
        SetupAgent ();
        GameTime.RegisterGameTick ( Tick );
    }

    void Start () {
        onMovementStateChanged += OnMovementStateChanged;       
        WarpAgentToNavMesh ();       
    }

    private void Update ()
    {
        if (!agent.isOnNavMesh) { return; }

        if (HasPath)
            CheckCurrentPath ();
    }

    private void Tick (int relativeTick)
    {
        SnowController.Instance.DrawDepression ( 4500, 0.75f, transform.position + (transform.forward * 0.5f) );
        agent.speed = agentSpeed * agentSpeedModifer * GameTime.GameTimeModifier;
        agent.acceleration = agentAcceleration * agentAccelerationModifier * GameTime.GameTimeModifier;
        agent.angularSpeed = agentAngularSpeed * agentAngularSpeedModifier * GameTime.GameTimeModifier;
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

        if (Physics.Raycast ( transform.position + (Vector3.up * 20.0f), Vector3.down, out hit, 10000, 1 << 9 ))
        {
            agent.Warp ( hit.point );
        }
        else
        {
            Debug.LogError ( "No navmesh to hit" );
        }
    }

    public void WarpSpecific(Vector3 position)
    {
        agent.Warp ( position );
    }

    public void WarpAgentToNavMesh (Vector3 position)
    {
        RaycastHit hit;

        if (Physics.Raycast ( position + (Vector3.up * 10.0f), Vector3.down, out hit, 10000, 1 << 9 ))
        {
            agent.Warp ( hit.point );
        }
        else
        {
            Debug.LogError ( "No navmesh to hit" );
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

    public bool SetDestination(GameObject target, Vector3 targetPosition)
    {
        if (!agent.isOnNavMesh) { WarpAgentToNavMesh (); }

        DestinationObject = target;
        destinationPosition = targetPosition;

        NavMeshPath newPath = new NavMeshPath ();
        agent.CalculatePath ( destinationPosition, newPath );

        if (newPath.status == NavMeshPathStatus.PathComplete)
        {
            OnPathConfirmed ( newPath );
            return true;
        }
        else
        {          
            OnPathInvalid ( newPath );
            return false;
        }
    }

    private void OnPathConfirmed (NavMeshPath newPath)
    {
        path = newPath;

        movementState = MovementState.Moving;
        onMovementStateChanged ( movementState );
        agent.avoidancePriority = AvoidancePriority;
        HasPath = true;
        agent.SetDestination ( destinationPosition );

        if (movementState == MovementState.Idle)
        {
            movementState = MovementState.Moving;
            onMovementStateChanged ( movementState );
        }

        onPathConfirmed?.Invoke ();

        //Debug.Log ( "OnPathConfirmed" );
    }

    private void OnPathInvalid(NavMeshPath newPath)
    {
        path = newPath;  
        //Debug.Log ( "OnPathInvalid" );
    }

    private void CheckCurrentPath ()
    {
        if (agent.pathPending) { return; }
        if (agent.pathStatus != NavMeshPathStatus.PathComplete) return;
        if (agent.remainingDistance > agent.stoppingDistance) { return; }
        //if (agent.hasPath) { Debug.Log ( "hasPath" ); return; }
        if (!HasPath) { return; }
        OnReachedPath ();
    }

    private void OnReachedPath ()
    {
        if (movementState == MovementState.Moving)
        {
            movementState = MovementState.Idle;
            agent.avoidancePriority = 100;
            onMovementStateChanged ( movementState );
        }

        if (onReachedPath != null) onReachedPath ( destinationPosition );
        ClearDestination ();
    }

    public void ClearDestination ()
    {
        if (this == null) return;
        if (agent == null) return;
        if (!agent.isOnNavMesh) return;

        destinationPosition = Vector3.zero;
        DestinationObject = null;
        path = null;
        HasPath = false;
        agent.ResetPath ();
        onDestinationCleared?.Invoke ();
    }

    private void OnDestroy ()
    {
        GameTime.UnRegisterGameTick ( Tick );
    }
}
