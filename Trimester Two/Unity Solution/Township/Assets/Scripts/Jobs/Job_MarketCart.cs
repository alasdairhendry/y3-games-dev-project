using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Job_MarketCart : Job {

    public enum Stage { Find, Collect, Transport }
    private Stage stage = Stage.Find;

    private Prop_Warehouse targetWarehouse;
    private float resourceQuantity = 0; // The formulated quantity we will take from the warehouse to the prop
    private bool agentGivenDestination = false;
    private bool resourcesReservedFromWarehouse = false;
    private bool resourcesTakenFromWarehouse = false;
    private bool resourcesGivenToCitizen = false;

    protected Prop targetProp;
    protected ResourceInventory propInventory;
    protected int resourceID;
    protected int maxSupplyQuantity;
    protected bool supply = false;

    // If supply is true, we are taking resourceID from warehouse to prop
    // if supply is false we are taking resourceID from prop to warehouse
    // if we are supply, we wont supply more than the maxSupplyQuantity, but if we are taking we will take all of them
    public Job_MarketCart (JobEntity entity, string name, bool open, float timeRequired, System.Action onComplete, Prop targetProp, ResourceInventory propInventory, int resourceID, int maxSupplyQuantity, bool supply) : base ( entity, name, open, timeRequired, onComplete )
    {
        this.resourceID = resourceID;
        this.targetProp = targetProp;
        this.propInventory = propInventory;
        this.resourceID = resourceID;
        this.maxSupplyQuantity = maxSupplyQuantity;
        this.supply = supply;

        if (supply) stage = Stage.Find;
        else stage = Stage.Collect;

        this.professionTypes.Add ( ProfessionType.Worker );
        //Tick_CheckCompletable ( 0 );
        //GameTime.RegisterGameTick ( Tick_CheckCompletable );
    }

    public override void DoJob ()
    {
        base.DoJob ();

        if (supply)
        {
            switch (stage)
            {
                case Stage.Find:
                    DoJob_Supply_Stage_Find ();
                    AgentJobStatus = "Searching for " + ResourceManager.Instance.GetResourceByID ( resourceID ).name + " to take to " + targetProp.data.name;
                    break;
                case Stage.Collect:
                    DoJob_Supply_Stage_Collect ();
                    AgentJobStatus = "Collecting " + ResourceManager.Instance.GetResourceByID ( resourceID ).name + " to take to " + targetProp.data.name;
                    break;
                case Stage.Transport:
                    DoJob_Supply_Stage_Transport ();
                    AgentJobStatus = "Delivering " + ResourceManager.Instance.GetResourceByID ( resourceID ).name + " to " + targetProp.data.name;
                    break;
            }
        }
        else
        {
            switch (stage)
            {
                case Stage.Collect:
                    DoJob_Collect_Stage_Collect ();
                    AgentJobStatus = "Collecting " + ResourceManager.Instance.GetResourceByID ( resourceID ).name + " to take to Warehouse";
                    break;
                case Stage.Find:
                    DoJob_Collect_Stage_Find ();
                    AgentJobStatus = "Searching for " + ResourceManager.Instance.GetResourceByID ( resourceID ).name + " to take to Warehouse";
                    break;
                case Stage.Transport:
                    DoJob_Collect_Stage_Transport ();
                    AgentJobStatus = "Delivering " + ResourceManager.Instance.GetResourceByID ( resourceID ).name + " to Warehouse";
                    break;
            }
        }
    }

    private void DoJob_Supply_Stage_Find ()
    {
        //if (targetWarehouse == null) { targetWarehouse = FindWarehouse_Supply (); }
        if (targetWarehouse == null) { targetWarehouse = WarehouseController.Instance.FindWarehouseToHaulAnyQuantity ( base.cBase.transform, resourceID, resourceQuantity ); }

        if (targetWarehouse == null)
        {
            /// TODO
            /// ADD CALLBACK FOR WHEN A WAREHOUSE GETS THE RESOURCE
            OnCharacterLeave ( "No warehouses found with required resource.", true, GetCompletableParams ( CompleteIdentifier.NotEnoughResources, resourceID, resourceQuantity ) );
            return;
        }

        float availableQuantityInWarehouse = WarehouseController.Instance.Inventory.GetAvailableQuantity ( resourceID );
        float spaceAvailable = propInventory.GetAvailableSpace ( resourceID );

        if (availableQuantityInWarehouse > spaceAvailable)
            resourceQuantity = spaceAvailable;
        else resourceQuantity = availableQuantityInWarehouse;

        WarehouseController.Instance.Inventory.ReserveItemQuantity ( resourceID, resourceQuantity );
        resourcesReservedFromWarehouse = true;

        stage = Stage.Collect;
    }

    private void DoJob_Supply_Stage_Collect ()
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
            stage = Stage.Find;
            agentGivenDestination = false;
            //OnCharacterLeave ( "Warehouse was destroyed", true, GetCompletableParams ( CompleteIdentifier.None ) );
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

    private void DoJob_Supply_Stage_Transport ()
    {
        if (!cBase.CitizenMovement.HasPath && !agentGivenDestination)
        {
            targetPosition = targetProp.CitizenInteractionPointGlobal;
            SetDestination ( targetProp.gameObject );
            agentGivenDestination = true;
            cBase.CitizenGraphics.SetUsingCart ( true );
            return;
        }

        if (!citizenReachedPath) return;

        cBase.Inventory.RemoveItemQuantity ( resourceID, resourceQuantity, base.cBase.transform, 2.0f );
        resourcesGivenToCitizen = false;
        agentGivenDestination = false;

        propInventory.AddItemQuantity ( resourceID, resourceQuantity, targetProp.transform, targetProp.data.UIOffsetY );

        cBase.CitizenGraphics.SetUsingCart ( false );

        base.OnComplete ();
    }

    private void DoJob_Collect_Stage_Collect ()
    {
        if (targetProp == null)
        {
            OnCharacterLeave ( "Target Prop was destroyed", true, GetCompletableParams ( CompleteIdentifier.None ) );
            return;
        }

        if (!cBase.CitizenMovement.HasPath && !agentGivenDestination)
        {
            targetPosition = targetProp.CitizenInteractionPointGlobal;
            SetDestination ( targetProp.gameObject );
            agentGivenDestination = true;
            return;
        }

        if (!citizenReachedPath) return;

        resourceQuantity = propInventory.GetAvailableQuantity ( resourceID );
        resourceQuantity = propInventory.RemoveItemQuantity ( resourceID, resourceQuantity, targetProp.transform, targetProp.data.UIOffsetY );

        cBase.Inventory.AddItemQuantity ( resourceID, resourceQuantity, base.cBase.transform, 2.0f );

        resourcesTakenFromWarehouse = true;
        resourcesGivenToCitizen = true;

        stage = Stage.Find;
        agentGivenDestination = false;
    }

    private void DoJob_Collect_Stage_Find ()
    {
        //if (targetWarehouse == null) { targetWarehouse = FindWarehouse_Collect (); }
        if (targetWarehouse == null) { targetWarehouse = WarehouseController.Instance.FindWarehouseToStore ( base.cBase.transform, resourceID, resourceQuantity ); }

        if (targetWarehouse == null)
        {
            /// TODO
            /// ADD CALLBACK FOR WHEN A WAREHOUSE GETS THE SPACE
            OnCharacterLeave ( "No warehouses found with available space.", true, GetCompletableParams(CompleteIdentifier.NotEnoughResources, resourceID, resourceQuantity) );
            //IsCompletable = false;

            if (resourcesGivenToCitizen)
            {
                resourcesGivenToCitizen = false;
                base.cBase.Inventory.RemoveItemQuantity ( resourceID, resourceQuantity, base.cBase.transform, 2.0f );
                Resource.DropResource ( resourceID, resourceQuantity, base.cBase.transform.position + (base.cBase.transform.forward), base.cBase.transform.eulerAngles );
            }
            return;
        }

        stage = Stage.Transport;
    }

    private void DoJob_Collect_Stage_Transport ()
    {
        if (targetWarehouse == null)
        {
            //OnCharacterLeave ( "Warehouse may have been destroyed.", true, GetCompletableParams ( CompleteIdentifier.None ) );
            stage = Stage.Find;
            agentGivenDestination = false;
            return;
        }

        if (!cBase.CitizenMovement.HasPath && !agentGivenDestination)
        {
            targetPosition = targetWarehouse.CitizenInteractionPointGlobal;
            SetDestination ( targetWarehouse.gameObject );
            agentGivenDestination = true;
            cBase.CitizenGraphics.SetUsingCart ( true );
            return;
        }

        if (!citizenReachedPath) return;

        cBase.Inventory.RemoveItemQuantity ( resourceID, resourceQuantity, base.cBase.transform, 2.0f );
        resourcesGivenToCitizen = false;
        agentGivenDestination = false;

        WarehouseController.Instance.Inventory.AddItemQuantity ( resourceID, resourceQuantity, targetWarehouse.transform, targetWarehouse.data.UIOffsetY );

        cBase.CitizenGraphics.SetUsingCart ( false );

        base.OnComplete ();
    }

    public override void OnCharacterLeave (string reason, bool setOpenToTrue, KeyValuePair<bool, string> isCompletable)
    {
        if (supply)
        {
            if (resourcesReservedFromWarehouse && !resourcesTakenFromWarehouse)
            {
                if (targetWarehouse != null)
                    WarehouseController.Instance.Inventory.UnreserveItemQuantity ( resourceID, resourceQuantity );
            }
            else if (resourcesReservedFromWarehouse && resourcesTakenFromWarehouse)
            {
                if (targetWarehouse != null)
                    WarehouseController.Instance.Inventory.AddItemQuantity ( resourceID, resourceQuantity, targetWarehouse.transform, targetWarehouse.data.UIOffsetY );
            }

            if (resourcesGivenToCitizen)
                base.cBase.Inventory.RemoveItemQuantity ( resourceID, resourceQuantity, base.cBase.transform, 2.0f );

            stage = Stage.Find;
        }
        else
        {
            //Taken from prop
            if (resourcesTakenFromWarehouse)
            {
                if(cBase!= null) { cBase.Inventory.RemoveItemQuantity ( resourceID, resourceQuantity, base.cBase.transform, 2.0f ); }
                if (propInventory != null) { propInventory.AddItemQuantity ( resourceID, resourceQuantity, targetProp.transform, targetProp.data.UIOffsetY ); }
            }
            stage = Stage.Collect;
        }

        cBase.CitizenGraphics.SetUsingCart ( false );

        targetWarehouse = null;
        resourceQuantity = 0;

        agentGivenDestination = false;
        resourcesReservedFromWarehouse = false;
        resourcesTakenFromWarehouse = false;
        resourcesGivenToCitizen = false;

        base.OnCharacterLeave ( reason, setOpenToTrue, isCompletable );
    }

    protected override void AddCompletableListeners ()
    {
        if (supply)
        {
            WarehouseController.Instance.Inventory.RegisterOnResourceAdded ( CheckCompletable );
        }
        else
        {
            WarehouseController.Instance.Inventory.RegisterOnResourceRemoved ( CheckCompletable );
        }

        base.AddCompletableListeners ();
    }

    private void CheckCompletable (int id, float amount)
    {
        if (supply)
        {
            if (id != resourceID) return;
            if (WarehouseController.Instance.Inventory.CheckIsEmpty ( resourceID )) return;

            base.SetIsCompletable ();
        }
        else
        {
            if (id != resourceID) return;
            if (WarehouseController.Instance.Inventory.CheckIsFull ( resourceID )) return;

            base.SetIsCompletable ();
        }
    }

    protected override void RemoveCompletableListeners ()
    {
        if (supply)
        {
            WarehouseController.Instance.Inventory.UnregisterOnResourceAdded ( CheckCompletable );
        }
        else
        {
            WarehouseController.Instance.Inventory.UnregisterOnResourceRemoved ( CheckCompletable );
        }

        base.RemoveCompletableListeners ();
    }

    //private Prop_Warehouse FindWarehouse_Supply ()
    //{
    //    List<GameObject> GOs = EntityManager.Instance.GetEntitiesByType ( typeof ( Prop_Warehouse ) );

    //    if (GOs == null) { Debug.LogError ( "No Warehouses found. We shouldnt run this every frame" ); return null; }

    //    List<Prop_Warehouse> eligibleWarehouses = new List<Prop_Warehouse> ();
    //    bool foundWarehouseWithExactOrGreater = false;

    //    for (int i = 0; i < GOs.Count; i++)
    //    {
    //        if (GOs[i] == null) continue;
    //        Prop_Warehouse w = GOs[i].GetComponent<Prop_Warehouse> ();

    //        if (w == null) { continue; }
    //        if (w.inventory == null) { Debug.LogError ( "Warehouse inventory is null" ); continue; }
    //        if (w.buildable.IsComplete == false) { continue; }

    //        float availableQuantity = w.inventory.GetAvailableQuantity ( resourceID );

    //        if(availableQuantity > 0)
    //        {
    //            if (availableQuantity < maxSupplyQuantity && foundWarehouseWithExactOrGreater) continue;
    //            if (availableQuantity >= maxSupplyQuantity) foundWarehouseWithExactOrGreater = true;
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
    //        if (base.cBase == null) continue;
    //        if (eligibleWarehouses[i] == null) continue;

    //        float availableQuantity = eligibleWarehouses[i].inventory.GetAvailableQuantity ( resourceID );
    //        if (availableQuantity < maxSupplyQuantity && foundWarehouseWithExactOrGreater) continue;            

    //        NavMeshPath path = new NavMeshPath ();

    //        NavMesh.CalculatePath ( base.cBase.transform.position, eligibleWarehouses[i].transform.position, 0, path );

    //        float f = GetPathLength ( path );
    //        if (f <= bestDistance)
    //        {
    //            bestDistance = f;
    //            fastestPath = i;
    //        }
    //    }

    //    if (fastestPath == -1) { Debug.LogError ( "No Eligible Path. We also shouldnt run this every frame." ); return null; }

    //    float availableQuantityInWarehouse = eligibleWarehouses[fastestPath].inventory.GetAvailableQuantity ( resourceID );
    //    float spaceAvailable = propInventory.GetAvailableSpace ( resourceID );

    //    if (availableQuantityInWarehouse > spaceAvailable)
    //        resourceQuantity = spaceAvailable;
    //    else resourceQuantity = availableQuantityInWarehouse;

    //    return eligibleWarehouses[fastestPath];
    //}

    //private Prop_Warehouse FindWarehouse_Collect ()
    //{
    //    List<GameObject> GOs = EntityManager.Instance.GetEntitiesByType ( typeof ( Prop_Warehouse ) );

    //    if (GOs == null) { Debug.LogError ( "No Warehouses found. We shouldnt run this every frame" ); return null; }

    //    List<Prop_Warehouse> eligibleWarehouses = new List<Prop_Warehouse> ();

    //    for (int i = 0; i < GOs.Count; i++)
    //    {
    //        if (GOs[i] == null) continue;
    //        Prop_Warehouse w = GOs[i].GetComponent<Prop_Warehouse> ();

    //        if (w == null) { continue; }
    //        if (w.inventory == null) { Debug.LogError ( "Warehouse inventory is null" ); continue; }
    //        if (w.buildable.IsComplete == false) { continue; }

    //        if (w.inventory.CheckCanHold ( resourceID, resourceQuantity ))
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
    //        if (base.cBase == null) continue;
    //        if (eligibleWarehouses[i] == null) continue;

    //        NavMeshPath path = new NavMeshPath ();

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

    //protected override IEnumerator CheckIsCompletable ()
    //{
    //    if (supply)
    //    {
    //        List<GameObject> GOs = EntityManager.Instance.GetEntitiesByType ( typeof ( Prop_Warehouse ) );

    //        if (GOs == null)
    //        {
    //            Debug.LogError ( "No Warehouses found. We shouldnt run this every frame" );
    //            IsCompletable = false;
    //            yield break;
    //        }

    //        List<Prop_Warehouse> eligibleWarehouses = new List<Prop_Warehouse> ();            

    //        for (int i = 0; i < GOs.Count; i++)
    //        {
    //            if (GOs[i] == null) continue;
    //            Prop_Warehouse w = GOs[i].GetComponent<Prop_Warehouse> ();

    //            if (w == null) { continue; }
    //            if (w.inventory == null) { Debug.LogError ( "Warehouse inventory is null" ); continue; }
    //            if (w.buildable.IsComplete == false) { continue; }

    //            float availableQuantity = w.inventory.GetAvailableQuantity ( resourceID );

    //            if (availableQuantity > 0)
    //            {
    //                eligibleWarehouses.Add ( w );
    //                break;
    //            }
    //        }

    //        // TODO: Maybe add some sort of tick system so this doesnt clog up the players client
    //        if (eligibleWarehouses.Count <= 0)
    //        {
    //            Debug.LogError ( "No Eligible Warehouses. We shouldnt run this every frame" );
    //            IsCompletable = false;
    //            yield break;
    //        }
    //        else
    //        {
    //            IsCompletable = true;
    //            yield break;
    //        }
    //    }
    //    else
    //    {
    //        List<GameObject> GOs = EntityManager.Instance.GetEntitiesByType ( typeof ( Prop_Warehouse ) );

    //        if (GOs == null)
    //        {
    //            Debug.LogError ( "No Warehouses found. We shouldnt run this every frame" );
    //            IsCompletable = false;
    //            yield break;
    //        }

    //        List<Prop_Warehouse> eligibleWarehouses = new List<Prop_Warehouse> ();

    //        for (int i = 0; i < GOs.Count; i++)
    //        {
    //            if (GOs[i] == null) continue;
    //            Prop_Warehouse w = GOs[i].GetComponent<Prop_Warehouse> ();

    //            if (w == null) { continue; }
    //            if (w.inventory == null) { Debug.LogError ( "Warehouse inventory is null" ); continue; }
    //            if (w.buildable.IsComplete == false) { continue; }

    //            if (w.inventory.CheckCanHold ( resourceID, resourceQuantity ))
    //            {           
    //                eligibleWarehouses.Add ( w );
    //            }
    //        }
    //        // TODO: Maybe add some sort of tick system so this doesnt clog up the players client
    //        if (eligibleWarehouses.Count <= 0)
    //        {
    //            Debug.LogError ( "No Eligible Warehouses. We shouldnt run this every frame" );
    //            IsCompletable = false;
    //            yield break;
    //        }
    //        else
    //        {
    //            IsCompletable = true;
    //            yield break;
    //        }
    //    }
    //}
}

