using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Job_Haul : Job
{

    public enum Stage { Find, Collect, Transport }
    private Stage stage = Stage.Find;

    private Prop_Warehouse targetWarehouse;
    private Buildable targetBuildable;

    private bool agentGivenDestination = false;

    private bool resourcesTakenFromWarehouse = false;
    private bool resourcesGivenToCitizen = false;

    private int resourceID;
    private float resourceQuantity;

    public Job_Haul (string name, bool open, int resourceID, float resourceQuantity, Buildable targetBuildable)
    {
        base.Name = name;
        base.Open = open;
        this.resourceID = resourceID;
        this.resourceQuantity = resourceQuantity;
        this.targetBuildable = targetBuildable;
    }

    public override void DoJob (float deltaGameTime)
    {
        switch (stage)
        {
            case Stage.Find:
                DoJob_Stage_Find ();
                break;
            case Stage.Collect:
                DoJob_Stage_Collect ();
                break;
            case Stage.Transport:
                DoJob_Stage_Transport ();
                break;
        }
    }

    private void DoJob_Stage_Find ()
    {
        if (targetWarehouse == null) { targetWarehouse = FindResource (); }
        if (targetWarehouse == null) { Debug.Log ( "Still no warehouse." ); return; }

        targetWarehouse.inventory.RemoveItemQuantity ( resourceID, resourceQuantity );
        resourcesTakenFromWarehouse = true;

        stage = Stage.Collect;
        Debug.LogError ( "SetStage: " + stage.ToString () );
    }

    private void DoJob_Stage_Collect ()
    {
        if (!character.agent.hasPath && !agentGivenDestination)
        {
            character.agent.SetDestination ( targetWarehouse.transform.position );
            agentGivenDestination = true;
            return;
        }

        if (ReachedPath ())
        {
            character.inventory.AddItemQuantity ( resourceID, resourceQuantity );
            resourcesGivenToCitizen = true;

            stage = Stage.Transport;
            agentGivenDestination = false;
            Debug.LogError ( "SetStage: " + stage.ToString () );
        }
    }

    private void DoJob_Stage_Transport ()
    {
        if (!character.agent.hasPath && !agentGivenDestination)
        {
            character.agent.SetDestination ( targetBuildable.transform.position );
            agentGivenDestination = true;
            return;
        }

        if (ReachedPath ())
        {
            character.inventory.RemoveItemQuantity ( resourceID, resourceQuantity );
            resourcesGivenToCitizen = false;
            agentGivenDestination = false;

            targetBuildable.AddMaterial ( resourceID, resourceQuantity );

            base.OnComplete ();
        }
    }

    public override void OnCharacterLeave ()
    {
        if (resourcesTakenFromWarehouse)
            targetWarehouse.inventory.AddItemQuantity ( resourceID, resourceQuantity );
        if (resourcesGivenToCitizen)
            base.character.inventory.RemoveItemQuantity ( resourceID, resourceQuantity );

        stage = Stage.Find;
        base.OnCharacterLeave ();
    }

    private bool ReachedPath ()
    {
        if (character.agent.pathPending) { Debug.LogError ( "Path Pending" ); return false; }
        if (character.agent.remainingDistance > character.agent.stoppingDistance) { Debug.LogError ( "Path Working" ); return false; }
        if (character.agent.hasPath) { Debug.LogError ( "Path Is Valid" ); return false; }
        return true;
    }

    private Prop_Warehouse FindResource ()
    {
        List<GameObject> GOs = PropManager.Instance.worldProps;

        if (GOs == null) { Debug.LogError ( "No Warehouses found. We shouldnt run this every frame" ); return null; }

        List<Prop_Warehouse> eligibleWarehouses = new List<Prop_Warehouse> ();

        for (int i = 0; i < GOs.Count; i++)
        {
            Prop_Warehouse w = GOs[i].GetComponent<Prop_Warehouse> ();

            if (w.inventory.CheckHasQuantity ( resourceID, resourceQuantity ))
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
            if (f <= bestDistance)
            {
                bestDistance = f;
                fastestPath = i;
            }
        }

        if (fastestPath == -1) { Debug.LogError ( "No Eligible Path. We also shouldnt run this every frame." ); return null; }

        return eligibleWarehouses[fastestPath];
    }

    private float GetPathLength (NavMeshPath path)
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
