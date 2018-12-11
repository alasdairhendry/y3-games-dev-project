using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterMovement : MonoBehaviour {

    public enum AnimationState { Idle, Walking, Carrying, Cart }
    public enum MovementState { Idle, Moving }

    private AnimationState animationState = AnimationState.Idle;
    public AnimationState SetAnimationState { set { animationState = value; ChangeAnimationState ( value ); } }

    private MovementState movementState = MovementState.Idle;

    public NavMeshAgent agent { get; protected set; }
    private Animator animator;

    public bool HasPath { get { return agent.hasPath; } }
    public NavMeshPath GetPath { get { return agent.path; } }

    private float agentSpeed = 1.0f;
    private float agentAcceleration = 2.0f;
    private float agentAngularSpeed = 20.0f;

    [SerializeField] [Range ( 0, 2 )] private float agentSpeedModifer = 1;
    [SerializeField] [Range ( 0, 2 )] private float agentAccelerationModifier = 1;
    [SerializeField] [Range ( 0, 2 )] private float agentAngularSpeedModifier = 1;

    private GameObject destinationObject;
    public GameObject DestinationObject { get { return destinationObject; } }

    private Vector3 destinationPosition;

    private System.Action<MovementState> onMovementStateChanged;

    void Start () {
        onMovementStateChanged += OnMovementStateChanged;
        SetupAgent ();
        WarpAgentToNavMesh ();
    }
	
	void Update () {
        SetSpeeds ();
    }

    private void SetupAgent ()
    {
        agent = GetComponent<NavMeshAgent> ();
        animator = GetComponentInChildren<Animator> ();

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

    private void SetSpeeds ()
    {
        FindObjectOfType<DEBUG_DrawSnowDepressionsWithMouse> ().DrawDepression ( 5500, 0.5f, transform.position + (transform.forward * 0.5f) );
        if (agent.velocity == Vector3.zero)
        {
            if (movementState == MovementState.Moving)
            {                
                movementState = MovementState.Idle;
                onMovementStateChanged ( movementState );
            }
            return;
        }

        agent.speed = agentSpeed * agentSpeedModifer * GameTime.GameTimeModifier;
        agent.acceleration = agentAcceleration * agentAccelerationModifier * GameTime.GameTimeModifier;
        agent.angularSpeed = agentAngularSpeed * agentAngularSpeedModifier * GameTime.GameTimeModifier;
        animator.speed = GameTime.GameTimeModifier;

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
            ChangeAnimationState ( AnimationState.Idle );
        }
        else
        {
            ChangeAnimationState ( animationState );
        }
    }

    public void SetUsingTool (bool state)
    {
        animator.SetBool ( "isChopping", state );
    }

    private void ChangeAnimationState(AnimationState state)
    {       
        switch (state)
        {
            case AnimationState.Idle:
                animator.SetBool ( "isWalking", false );
                animator.SetBool ( "isCarrying", false );
                animator.SetBool ( "isWheelbarrow", false );
                animator.SetFloat ( "IdleState", 0 );
                break;

            case AnimationState.Walking:
                animator.SetBool ( "isWalking", true );
                animator.SetFloat ( "IdleState", 0 );
                break;

            case AnimationState.Carrying:
                animator.SetBool ( "isWalking", true );
                animator.SetBool ( "isCarrying", true );
                animator.SetFloat ( "IdleState", 0 );
                break;

            case AnimationState.Cart:
                animator.SetBool ( "isWalking", true );
                animator.SetBool ( "isWheelbarrow", true );
                animator.SetFloat ( "IdleState", 1 );
                break;

            default:
                break;
        }
    }

    public void SetInjuryState(float healthValue)
    {
        animator.SetFloat ( "Health", healthValue );
    }

    public void SetDestination(GameObject target, Vector3 targetPosition)
    {
        destinationObject = target;
        destinationPosition = targetPosition;

        agent.SetDestination ( destinationPosition );
    }

    public void ClearDestination ()
    {
        destinationPosition = Vector3.zero;
        destinationObject = null;
        agent.ResetPath ();
    }

    public bool ReachedPath ()
    {
        if (agent.pathPending) { return false; }
        if (agent.remainingDistance > agent.stoppingDistance) { return false; }
        if (agent.hasPath) { return false; }
        return true;
    }
}
