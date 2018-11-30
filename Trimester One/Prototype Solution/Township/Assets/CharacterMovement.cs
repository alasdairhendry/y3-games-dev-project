using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterMovement : MonoBehaviour {

    public NavMeshAgent agent { get; protected set; }
    private Animator animator;

    private float agentSpeed = 1.0f;
    private float agentAcceleration = 2.0f;
    private float agentAngularSpeed = 20.0f;
    
    void Start () {
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
        if (agent.velocity == Vector3.zero) { animator.SetBool ( "isMoving", false ); return; }

        agent.speed = agentSpeed * GameTime.GameTimeModifier;
        agent.acceleration = agentAcceleration * GameTime.GameTimeModifier;
        agent.angularSpeed = agentAngularSpeed * GameTime.GameTimeModifier;
        animator.speed = GameTime.GameTimeModifier;

        animator.SetBool ( "isMoving", true );
    }

    public void SetUsingTool (bool state)
    {
        animator.SetBool ( "usingTool", state );
    }
}
