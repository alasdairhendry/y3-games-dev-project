using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Job_Haul : Job {

    private Prop_Warehouse target;
    private Vector3 destination;

    private bool reachedTarget = false;
    private bool reachedDestination = false;

    private bool givenDestination = false;

    private int resourceID;
    private float resourceQuantity;
    
    public Job_Haul(string name, bool open, int resourceID, float resourceQuantity, Vector3 destination)
    {
        base.Name = name;
        base.Open = open;
        this.resourceID = resourceID;
        this.resourceQuantity = resourceQuantity;
        this.destination = destination;
    }

    public override void DoJob (float deltaTime)
    {
        if (target == null) { target = FindResource (); }

        if (target == null) { Debug.Log ( "Still no warehouse." ); return; }

        if (!reachedTarget && !character.agent.hasPath && !givenDestination)
        {
            character.agent.SetDestination ( target.transform.position );
            givenDestination = true;
            return;
        }

        Debug.Log ( "Remaining Distance: " + character.agent.remainingDistance );

        if (ReachedPath () && !reachedTarget)
        {
            Debug.Log ( "Reached target position" );

            if (!target.inventory.CheckHasQuantity ( resourceID, resourceQuantity ))
            {
                target = null;
                return;
            }

            target.inventory.AddItemQuantity ( resourceID, character.inventory.AddItemQuantity ( resourceID, target.inventory.RemoveItemQuantity ( resourceID, resourceQuantity ) ) );

            reachedTarget = true;
            return;
        }


    }

    private bool ReachedPath ()
    {
        if (character.agent.pathPending) return false;
        if (character.agent.remainingDistance > character.agent.stoppingDistance) return false;
        if (character.agent.hasPath) return false;
        return true;
    }

    private Prop_Warehouse FindResource ()
    {
        List<GameObject> GOs = PropManager.Instance.worldProps;        

        if(GOs == null) { Debug.LogError ( "No Warehouses found. We shouldnt run this every frame" ); return null; }

        List<Prop_Warehouse> eligibleWarehouses = new List<Prop_Warehouse> ();

        for (int i = 0; i < GOs.Count; i++)
        {
            Prop_Warehouse w = GOs[i].GetComponent<Prop_Warehouse> ();

            if(w.inventory.CheckHasQuantity(resourceID, resourceQuantity))
            {
                eligibleWarehouses.Add ( w );
            }
        }

        // TODO: Maybe add some sort of tick system so this doesnt clog up the players client
        if (eligibleWarehouses.Count <= 0) { Debug.LogError ( "No Eligible Warehouses. We shouldnt run this every frame" ); return null; }

        int fastestPath = -1;
        float bestDistance = float.MaxValue;

        for (int i = 0; i < eligibleWarehouses.Count; i++)
        {
            NavMeshPath path = new NavMeshPath ();
            NavMesh.CalculatePath ( base.character.transform.position, eligibleWarehouses[i].transform.position, 0, path );

            float f = GetPathLength ( path );
            if(f <= bestDistance)
            {
                bestDistance = f;
                fastestPath = i;
            }
        }   
        
        if(fastestPath == -1) { Debug.LogError ( "No Eligible Path. We also shouldnt run this every frame." ); return null; }

        return eligibleWarehouses[fastestPath];
    }

    private float GetPathLength(NavMeshPath path)
    {
        float length = float.MaxValue;
        if (path.status != NavMeshPathStatus.PathComplete) return length;

        for (int i = 1; i < path.corners.Length; i++)
        {
            length += Vector3.Distance ( path.corners[i - 1], path.corners[i] );
        }

        return length;
    }
}
