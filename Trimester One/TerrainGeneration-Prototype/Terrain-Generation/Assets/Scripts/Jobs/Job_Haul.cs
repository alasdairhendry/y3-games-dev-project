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

    private bool resourcesReservedFromWarehouse = false;
    private bool resourcesTakenFromWarehouse = false;
    private bool resourcesGivenToCitizen = false;

    private int resourceID;
    private float resourceQuantity;

    public Job_Haul (string name, bool open, int resourceID, float resourceQuantity, Buildable targetBuildable)
    {
        this.id = JobController.GetNewJobID ();
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
                agentJobStatus = "Searching for " + ResourceManager.Instance.GetResourceByID ( resourceID ).name + " to take to " + targetBuildable.gameObject.name;
                break;
            case Stage.Collect:
                DoJob_Stage_Collect ();
                agentJobStatus = "Collecting " + ResourceManager.Instance.GetResourceByID ( resourceID ).name + " to take to " + targetBuildable.gameObject.name;
                break;
            case Stage.Transport:
                DoJob_Stage_Transport ();
                agentJobStatus = "Delivering " + ResourceManager.Instance.GetResourceByID ( resourceID ).name + " to " + targetBuildable.gameObject.name;
                break;
        }
    }

    private void DoJob_Stage_Find ()
    {
        if (targetWarehouse == null) { targetWarehouse = FindResource (); }
        if (targetWarehouse == null) { OnCharacterLeave ("No warehouses found with required resource."); return; }

        targetWarehouse.inventory.ReserveItemQuantity ( resourceID, resourceQuantity );
        resourcesReservedFromWarehouse = true;

        stage = Stage.Collect;
    }

    private void DoJob_Stage_Collect ()
    {
        if (!character.agent.hasPath && !agentGivenDestination)
        {
            character.agent.SetDestination ( targetWarehouse.CitizenInteractionPointGlobal );
            agentGivenDestination = true;
            return;
        }

        if (ReachedPath ())
        {
            targetWarehouse.inventory.TakeReservedItemQuantity ( resourceID, resourceQuantity );
            character.Inventory.AddItemQuantity ( resourceID, resourceQuantity );

            resourcesTakenFromWarehouse = true;
            resourcesGivenToCitizen = true;

            stage = Stage.Transport;
            agentGivenDestination = false;            
        }
    }

    private void DoJob_Stage_Transport ()
    {
        if (!character.agent.hasPath && !agentGivenDestination)
        {
            character.agent.SetDestination ( targetBuildable.GetPropData.CitizenInteractionPointGlobal );
            agentGivenDestination = true;
            return;
        }

        if (ReachedPath ())
        {
            character.Inventory.RemoveItemQuantity ( resourceID, resourceQuantity );
            resourcesGivenToCitizen = false;
            agentGivenDestination = false;

            targetBuildable.AddMaterial ( resourceID, resourceQuantity );

            base.OnComplete ();
        }
    }

    public override void OnCharacterLeave (string reason)
    {
        if(resourcesReservedFromWarehouse && !resourcesTakenFromWarehouse)
        {
            targetWarehouse.inventory.UnreserveItemQuantity ( resourceID, resourceQuantity );
        }
        else if(resourcesReservedFromWarehouse && resourcesTakenFromWarehouse)
        {
            targetWarehouse.inventory.AddItemQuantity ( resourceID, resourceQuantity );
        }
        
        if (resourcesGivenToCitizen)
            base.character.Inventory.RemoveItemQuantity ( resourceID, resourceQuantity );

        stage = Stage.Find;
        targetWarehouse = null;        

        agentGivenDestination = false;
        resourcesReservedFromWarehouse = false;
        resourcesTakenFromWarehouse = false;
        resourcesGivenToCitizen = false;

        base.OnCharacterLeave (reason);
    }    

    private Prop_Warehouse FindResource ()
    {
        List<GameObject> GOs = PropManager.Instance.worldProps;

        if (GOs == null) { Debug.LogError ( "No Warehouses found. We shouldnt run this every frame" ); return null; }

        List<Prop_Warehouse> eligibleWarehouses = new List<Prop_Warehouse> ();

        for (int i = 0; i < GOs.Count; i++)
        {
            Prop_Warehouse w = GOs[i].GetComponent<Prop_Warehouse> ();

            if(w == null) { continue; }
            if(w.inventory == null) { Debug.LogError ( "Warehouse inventory is null" ); continue; }

            if (w.inventory.CheckHasQuantityAvailable ( resourceID, resourceQuantity ))
            {
                eligibleWarehouses.Add ( w );
            }
            else
            {
                Debug.LogError ( "Warehouse inventory does not have sufficient quantity" ); continue;
            }
        }

        // TODO: Maybe add some sort of tick system so this doesnt clog up the players client
        if (eligibleWarehouses.Count <= 0) { Debug.LogError ( "No Eligible Warehouses. We shouldnt run this every frame" ); return null; }

        int fastestPath = -1;
        float bestDistance = float.MaxValue;

        for (int i = 0; i < eligibleWarehouses.Count; i++)
        {
            NavMeshPath path = new NavMeshPath ();

            if (base.character == null) continue;
            if (eligibleWarehouses[i] == null) continue;

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
}
