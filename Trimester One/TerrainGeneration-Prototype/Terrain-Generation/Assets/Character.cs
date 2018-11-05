using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour {

    public NavMeshAgent agent { get; protected set; }
    public ResourceInventory inventory { get; protected set; }
    Job currentJob;

    // Use this for initialization
    void Start ()
    {
        agent = GetComponent<NavMeshAgent> ();
        inventory = new ResourceInventory ();
        RaycastHit hit;

        if (Physics.Raycast ( transform.position, Vector3.down, out hit, 10000, 1 << 9 ))
        {
            agent.Warp ( hit.point );
        }
    }

    private void Update ()
    {
        if(currentJob == null)
        {
            currentJob = JobController.GetNext (this);

            if (currentJob == null) return;                
        }

        currentJob.DoJob ( Time.deltaTime );
    }
}
