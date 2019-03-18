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
    //private Buildable targetBuildable;
    private Prop targetProp;
    private ResourceInventory targetInventory;

    private bool agentGivenDestination = false;

    private bool resourcesReservedFromWarehouse = false;
    private bool resourcesTakenFromWarehouse = false;
    private bool resourcesGivenToCitizen = false;

    private int resourceID;
    private float resourceQuantity;

    public Job_Haul (JobEntity entity, string name, bool open, float timeRequired, System.Action onComplete, int resourceID, float resourceQuantity, Prop targetProp, ResourceInventory targetInventory) : base ( entity, name, open, timeRequired, onComplete )
    {        
        this.resourceID = resourceID;
        this.resourceQuantity = resourceQuantity;
        this.targetProp = targetProp;
        this.targetInventory = targetInventory;
        this.professionTypes.Add ( ProfessionType.Worker );
        BreakForEnergy = false;
        Debug.Log ( "Creating haul job for " + this.resourceQuantity + " " + ResourceManager.Instance.GetResourceByID ( this.resourceID ).name );

        //Tick_CheckCompletable ( 0 );
    }

    public override void DoJob ()
    {
        base.DoJob ();

        switch (stage)
        {
            case Stage.Find:
                DoJob_Stage_Find ();
                AgentJobStatus = "Searching for " + ResourceManager.Instance.GetResourceByID ( resourceID ).name + " to take to " + targetProp.data.name;
                break;
            case Stage.Collect:
                DoJob_Stage_Collect ();
                AgentJobStatus = "Collecting " + ResourceManager.Instance.GetResourceByID ( resourceID ).name + " to take to " + targetProp.data.name;
                break;
            case Stage.Transport:
                DoJob_Stage_Transport ();
                AgentJobStatus = "Delivering " + ResourceManager.Instance.GetResourceByID ( resourceID ).name + " to " + targetProp.data.name;
                break;
        }
    }

    private void DoJob_Stage_Find ()
    {
        //if (targetWarehouse == null) { targetWarehouse = FindResource (); }
        if (targetWarehouse == null) { targetWarehouse = WarehouseController.Instance.FindWarehouseToHaul ( base.cBase.transform, resourceID, resourceQuantity ); }

        if (targetWarehouse == null)
        {
            /// TODO
            /// ADD CALLBACK FOR ANY WAREHOUSE GAINING THIS RESOURCE
            OnCharacterLeave ( "No warehouses found with required resource.", true, GetCompletableParams ( CompleteIdentifier.NotEnoughResources, resourceID, resourceQuantity ) );
            return;
        }

        //targetWarehouse.inventory.ReserveItemQuantity ( resourceID, resourceQuantity );
        WarehouseController.Instance.Inventory.ReserveItemQuantity ( resourceID, resourceQuantity );
        resourcesReservedFromWarehouse = true;

        stage = Stage.Collect;
    }

    private void DoJob_Stage_Collect ()
    {
        if (!cBase.CitizenMovement.HasPath && !agentGivenDestination)
        {
            targetPosition = targetWarehouse.CitizenInteractionPointGlobal;
            SetDestination ( targetWarehouse.gameObject );
            agentGivenDestination = true;
            return;
        }

        if (targetWarehouse == null)
        {
            OnCharacterLeave ( "Warehouse was destroyed", true, GetCompletableParams ( CompleteIdentifier.None ) );
            return;
        }

        if (!citizenReachedPath) return;

        WarehouseController.Instance.Inventory.TakeReservedItemQuantity ( resourceID, resourceQuantity );
        cBase.Inventory.AddItemQuantity ( resourceID, resourceQuantity, base.cBase.transform, 2.0f );

        resourcesTakenFromWarehouse = true;
        resourcesGivenToCitizen = true;

        stage = Stage.Transport;
        agentGivenDestination = false;

    }

    private void DoJob_Stage_Transport ()
    {
        if (!cBase.CitizenMovement.HasPath && !agentGivenDestination)
        {
            targetPosition = targetProp.CitizenInteractionPointGlobal;
            SetDestination ( targetProp.gameObject );
            agentGivenDestination = true;
            cBase.CitizenGraphics.SetUsingCrate ( true );
            return;
        }

        if (!citizenReachedPath) return;

        cBase.Inventory.RemoveItemQuantity ( resourceID, resourceQuantity, base.cBase.transform, 2.0f );
        resourcesGivenToCitizen = false;
        agentGivenDestination = false;
        
        targetInventory.AddItemQuantity ( resourceID, resourceQuantity, targetProp.transform, targetProp.data.UIOffsetY );

        cBase.CitizenGraphics.SetUsingCrate ( false );

        base.OnComplete ();
    }    

    public override void OnCharacterLeave (string reason, bool setOpenToTrue, KeyValuePair<bool, string> isCompletable)
    {
        if(resourcesReservedFromWarehouse && !resourcesTakenFromWarehouse)
        {
            WarehouseController.Instance.Inventory.UnreserveItemQuantity ( resourceID, resourceQuantity );
        }
        else if(resourcesReservedFromWarehouse && resourcesTakenFromWarehouse)
        {
            if (targetWarehouse == null)
                WarehouseController.Instance.Inventory.AddItemQuantity ( resourceID, resourceQuantity, null, 0 );
            else
                WarehouseController.Instance.Inventory.AddItemQuantity ( resourceID, resourceQuantity, targetWarehouse.transform, targetWarehouse.data.UIOffsetY );
        }

        cBase.CitizenGraphics.SetUsingCrate ( false );

        if (resourcesGivenToCitizen)
            base.cBase.Inventory.RemoveItemQuantity ( resourceID, resourceQuantity, base.cBase.transform, 2.0f );

        stage = Stage.Find;
        targetWarehouse = null;        

        agentGivenDestination = false;
        resourcesReservedFromWarehouse = false;
        resourcesTakenFromWarehouse = false;
        resourcesGivenToCitizen = false;

        base.OnCharacterLeave (reason , setOpenToTrue, isCompletable);
    }    

    protected override void AddCompletableListeners ()
    {
        WarehouseController.Instance.Inventory.RegisterOnResourceAdded ( OnResourceAdded );
        base.AddCompletableListeners ();
    }

    protected override void RemoveCompletableListeners ()
    {
        WarehouseController.Instance.Inventory.UnregisterOnResourceAdded ( OnResourceAdded );
        base.RemoveCompletableListeners ();
    }

    private void OnResourceAdded (int resourceID, float resourceQuantity)
    {
        if (base.IsCompletable) return;

        if (resourceID != this.resourceID) return;

        if(WarehouseController.Instance.Inventory.CheckHasQuantityAvailable( this.resourceID, this.resourceQuantity ))
        {
            Debug.Log ( "Warehouse inventory now has " + this.resourceQuantity + " of " + ResourceManager.Instance.GetResourceByID ( this.resourceID ).name + " available" );
            base.SetIsCompletable ();
        }
    }

    //private Prop_Warehouse FindResource ()
    //{
    //    List<GameObject> GOs = EntityManager.Instance.GetEntitiesByType ( typeof ( Prop_Warehouse ) );

    //    if (GOs == null) { Debug.LogError ( "No Warehouses found. We shouldnt run this every frame" ); return null; }

    //    List<Prop_Warehouse> eligibleWarehouses = new List<Prop_Warehouse> ();

    //    for (int i = 0; i < GOs.Count; i++)
    //    {
    //        if (GOs[i] == null) continue;
    //        Prop_Warehouse w = GOs[i].GetComponent<Prop_Warehouse> ();

    //        if(w == null) { continue; }
    //        if(w.inventory == null) { Debug.LogError ( "Warehouse inventory is null" ); continue; }
    //        if (w.buildable.IsComplete == false) { continue; }

    //        if (w.inventory.CheckHasQuantityAvailable ( resourceID, resourceQuantity ))
    //        {
    //            eligibleWarehouses.Add ( w );
    //        }
    //        else
    //        {
    //            Debug.LogError ( "Warehouse inventory does not have sufficient quantity" ); continue;
    //        }
    //    }

    //    // TODO: Maybe add some sort of tick system so this doesnt clog up the players client
    //    if (eligibleWarehouses.Count <= 0) { Debug.LogError ( "No Eligible Warehouses. We shouldnt run this every frame" ); return null; }

    //    int fastestPath = -1;
    //    float bestDistance = float.MaxValue;

    //    for (int i = 0; i < eligibleWarehouses.Count; i++)
    //    {
    //        NavMeshPath path = new NavMeshPath ();

    //        if (base.cBase == null) continue;
    //        if (eligibleWarehouses[i] == null) continue;

    //        NavMesh.CalculatePath ( base.cBase.transform.position, eligibleWarehouses[i].transform.position, 0, path );

    //        float f = GetPathLength ( path );
    //        if (f <= bestDistance)
    //        {               
    //            bestDistance = f;
    //            fastestPath = i;
    //        }
    //    }

    //    if (fastestPath == -1) { Debug.LogError ( "No Eligible Path. We also shouldnt run this every frame." ); return null; }

    //    return eligibleWarehouses[fastestPath];
    //}

    //private Prop_Warehouse FindResourceIsCompletable ()
    //{
    //    List<GameObject> GOs = PropManager.Instance.worldProps;

    //    if (GOs == null) { Debug.LogError ( "No Warehouses found. We shouldnt run this every frame" ); return null; }

    //    List<Prop_Warehouse> eligibleWarehouses = new List<Prop_Warehouse> ();

    //    for (int i = 0; i < GOs.Count; i++)
    //    {
    //        if (GOs[i] == null) continue;
    //        Prop_Warehouse w = GOs[i].GetComponent<Prop_Warehouse> ();

    //        if (w == null) { continue; }
    //        if (w.inventory == null) { Debug.LogError ( "Warehouse inventory is null" ); continue; }

    //        if (w.inventory.CheckHasQuantityAvailable ( resourceID, resourceQuantity ))
    //        {
    //            eligibleWarehouses.Add ( w );
    //        }
    //        else
    //        {
    //            Debug.LogError ( "Warehouse inventory does not have sufficient quantity" ); continue;
    //        }
    //    }

    //    // TODO: Maybe add some sort of tick system so this doesnt clog up the players client
    //    if (eligibleWarehouses.Count <= 0) { Debug.LogError ( "No Eligible Warehouses. We shouldnt run this every frame" ); return null; }
    //    else { return eligibleWarehouses[0]; }
    //}

    //protected override IEnumerator CheckIsCompletable ()
    //{        
    //    isCheckingCompletable = true;

    //    List<GameObject> GOs = EntityManager.Instance.GetEntitiesByType ( typeof ( Prop_Warehouse ) );

    //    if (GOs == null)
    //    {
    //        IsCompletable = false;
    //        isCheckingCompletable = false;
    //        IsCompletableReason = "No warehouses in the world.";
    //        yield break;
    //    }

    //    bool found = false;

    //    for (int i = 0; i < GOs.Count; i++)
    //    {
    //        if (GOs[i] == null) continue;
    //        Prop_Warehouse w = GOs[i].GetComponent<Prop_Warehouse> ();

    //        if (w == null) { continue; }
    //        if (w.inventory == null) { continue; }
    //        if (w.buildable.IsComplete == false) { continue; }

    //        if (w.inventory.CheckHasQuantityAvailable ( resourceID, resourceQuantity ))
    //        {
    //            found = true;
    //            break;
    //        }
    //    }

    //    if (!found)
    //    {
    //        IsCompletable = false;
    //        isCheckingCompletable = false;
    //        IsCompletableReason = "No warehouses with available resource.";
    //        yield break;
    //    }
    //    else
    //    {
    //        IsCompletable = true;
    //        isCheckingCompletable = false;
    //        IsCompletableReason = "";
    //        yield break;
    //    }
    //}
}
