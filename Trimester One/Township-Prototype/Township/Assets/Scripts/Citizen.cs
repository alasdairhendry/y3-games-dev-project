using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Citizen : MonoBehaviour {

    NavMeshAgent navMesh;

	// Use this for initialization
	void Start () {
        navMesh = GetComponent<NavMeshAgent>();

        RaycastHit hit;
        if(Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            navMesh.Warp(hit.point);
        }
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    private void SetDestination(Vector3 position)
    {
        navMesh.SetDestination(position);        
    }
}
