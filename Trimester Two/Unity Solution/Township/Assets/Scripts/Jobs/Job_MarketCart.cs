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
        Tick_CheckCompletable ( 0 );
        GameTime.RegisterGameTick ( Tick_CheckCompletable );
    }

    public override void DoJob (float deltaGameTime)
    {
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
        if (targetWarehouse == null) { targetWarehouse = FindWarehouse_Supply (); }
        if (targetWarehouse == null) { OnCharacterLeave ( "No warehouses found with required resource.", true ); return; }

        targetWarehouse.inventory.ReserveItemQuantity ( resourceID, resourceQuantity );
        resourcesReservedFromWarehouse = true;

        stage = Stage.Collect;
    }

    private void DoJob_Supply_Stage_Collect ()
    {
        if (!cBase.CitizenMovement.HasPath && !agentGivenDestination)
        {
            cBase.CitizenMovement.SetDestination ( targetWarehouse.gameObject, targetWarehouse.CitizenInteractionPointGlobal );
            agentGivenDestination = true;
            return;
        }

        if (targetWarehouse == null)
        {
            OnCharacterLeave ( "Warehouse was destroyed", true );
            return;
        }

        if (this.cBase.CitizenMovement.ReachedPath ())
        {
            targetWarehouse.inventory.TakeReservedItemQuantity ( resourceID, resourceQuantity );
            cBase.Inventory.AddItemQuantity ( resourceID, resourceQuantity );

            resourcesTakenFromWarehouse = true;
            resourcesGivenToCitizen = true;

            stage = Stage.Transport;
            agentGivenDestination = false;
        }
    }

    private void DoJob_Supply_Stage_Transport ()
    {
        if (!cBase.CitizenMovement.HasPath && !agentGivenDestination)
        {
            cBase.CitizenMovement.SetDestination ( targetProp.gameObject, targetProp.CitizenInteractionPointGlobal );
            agentGivenDestination = true;
            cBase.CitizenGraphics.SetUsingCart ( true );
            return;
        }

        if (this.cBase.CitizenMovement.ReachedPath ())
        {
            cBase.Inventory.RemoveItemQuantity ( resourceID, resourceQuantity );
            resourcesGivenToCitizen = false;
            agentGivenDestination = false;

            propInventory.AddItemQuantity ( resourceID, resourceQuantity );

            cBase.CitizenGraphics.SetUsingCart ( false );

            base.OnComplete ();
        }
    }

    private void DoJob_Collect_Stage_Collect ()
    {
        if (targetProp == null)
        {
            OnCharacterLeave ( "Target Prop was destroyed", true );
            return;
        }

        if (!cBase.CitizenMovement.HasPath && !agentGivenDestination)
        {
            cBase.CitizenMovement.SetDestination ( targetProp.gameObject, targetProp.CitizenInteractionPointGlobal );
            agentGivenDestination = true;
            return;
        }

        if (this.cBase.CitizenMovement.ReachedPath ())
        {
            resourceQuantity = propInventory.GetAvailableQuantity ( resourceID );
            resourceQuantity = propInventory.RemoveItemQuantity ( resourceID, resourceQuantity );

            cBase.Inventory.AddItemQuantity ( resourceID, resourceQuantity );

            resourcesTakenFromWarehouse = true;
            resourcesGivenToCitizen = true;

            stage = Stage.Find;
            agentGivenDestination = false;
        }
    }

    private void DoJob_Collect_Stage_Find ()
    {
        if (targetWarehouse == null) { targetWarehouse = FindWarehouse_Collect (); }
        if (targetWarehouse == null) { OnCharacterLeave ( "No warehouses found with available space.", true ); IsCompletable = false; return; }

        stage = Stage.Transport;
    }

    private void DoJob_Collect_Stage_Transport ()
    {
        if(targetWarehouse == null)
        {
            OnCharacterLeave ( "Warehouse may have been destroyed.", true ); return;
        }

        if (!cBase.CitizenMovement.HasPath && !agentGivenDestination)
        {
            cBase.CitizenMovement.SetDestination ( targetWarehouse.gameObject, targetWarehouse.CitizenInteractionPointGlobal );
            agentGivenDestination = true;
            cBase.CitizenGraphics.SetUsingCart ( true );
            return;
        }

        if (this.cBase.CitizenMovement.ReachedPath ())
        {
            cBase.Inventory.RemoveItemQuantity ( resourceID, resourceQuantity );
            resourcesGivenToCitizen = false;
            agentGivenDestination = false;

            targetWarehouse.inventory.AddItemQuantity ( resourceID, resourceQuantity );

            cBase.CitizenGraphics.SetUsingCart ( false );

            base.OnComplete ();
        }
    }

    public override void OnCharacterLeave (string reason, bool setOpenToTrue)
    {
        if (supply)
        {
            if (resourcesReservedFromWarehouse && !resourcesTakenFromWarehouse)
            {
                if (targetWarehouse != null)
                    targetWarehouse.inventory.UnreserveItemQuantity ( resourceID, resourceQuantity );
            }
            else if (resourcesReservedFromWarehouse && resourcesTakenFromWarehouse)
            {
                if (targetWarehouse != null)
                    targetWarehouse.inventory.AddItemQuantity ( resourceID, resourceQuantity );
            }

            if (resourcesGivenToCitizen)
                base.cBase.Inventory.RemoveItemQuantity ( resourceID, resourceQuantity );

            stage = Stage.Find;
        }
        else
        {
            //Taken from prop
            if (resourcesTakenFromWarehouse)
            {
                if(cBase!= null) { cBase.Inventory.RemoveItemQuantity ( resourceID, resourceQuantity ); }
                if (propInventory != null) { propInventory.AddItemQuantity ( resourceID, resourceQuantity ); }
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

        base.OnCharacterLeave ( reason, setOpenToTrue );
    }

    private Prop_Warehouse FindWarehouse_Supply ()
    {
        List<GameObject> GOs = PropManager.Instance.worldProps;

        if (GOs == null) { Debug.LogError ( "No Warehouses found. We shouldnt run this every frame" ); return null; }

        List<Prop_Warehouse> eligibleWarehouses = new List<Prop_Warehouse> ();
        bool foundWarehouseWithExactOrGreater = false;

        for (int i = 0; i < GOs.Count; i++)
        {
            if (GOs[i] == null) continue;
            Prop_Warehouse w = GOs[i].GetComponent<Prop_Warehouse> ();

            if (w == null) { continue; }
            if (w.inventory == null) { Debug.LogError ( "Warehouse inventory is null" ); continue; }

            float availableQuantity = w.inventory.GetAvailableQuantity ( resourceID );

            if(availableQuantity > 0)
            {
                if (availableQuantity < maxSupplyQuantity && foundWarehouseWithExactOrGreater) continue;
                if (availableQuantity >= maxSupplyQuantity) foundWarehouseWithExactOrGreater = true;
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
            if (base.cBase == null) continue;
            if (eligibleWarehouses[i] == null) continue;

            float availableQuantity = eligibleWarehouses[i].inventory.GetAvailableQuantity ( resourceID );
            if (availableQuantity < maxSupplyQuantity && foundWarehouseWithExactOrGreater) continue;            

            NavMeshPath path = new NavMeshPath ();

            NavMesh.CalculatePath ( base.cBase.transform.position, eligibleWarehouses[i].transform.position, 0, path );

            float f = GetPathLength ( path );
            if (f <= bestDistance)
            {
                bestDistance = f;
                fastestPath = i;
            }
        }

        if (fastestPath == -1) { Debug.LogError ( "No Eligible Path. We also shouldnt run this every frame." ); return null; }

        resourceQuantity = eligibleWarehouses[fastestPath].inventory.GetAvailableQuantity ( resourceID );
        return eligibleWarehouses[fastestPath];
    }

    private Prop_Warehouse FindWarehouse_Collect ()
    {
        List<GameObject> GOs = PropManager.Instance.worldProps;

        if (GOs == null) { Debug.LogError ( "No Warehouses found. We shouldnt run this every frame" ); return null; }

        List<Prop_Warehouse> eligibleWarehouses = new List<Prop_Warehouse> ();

        for (int i = 0; i < GOs.Count; i++)
        {
            if (GOs[i] == null) continue;
            Prop_Warehouse w = GOs[i].GetComponent<Prop_Warehouse> ();

            if (w == null) { continue; }
            if (w.inventory == null) { Debug.LogError ( "Warehouse inventory is null" ); continue; }

            if (w.inventory.CheckCanHold ( resourceID, resourceQuantity ))
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
            if (base.cBase == null) continue;
            if (eligibleWarehouses[i] == null) continue;

            NavMeshPath path = new NavMeshPath ();

            NavMesh.CalculatePath ( base.cBase.transform.position, eligibleWarehouses[i].transform.position, 0, path );

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

    protected override IEnumerator CheckIsCompletable ()
    {
        if (supply)
        {
            List<GameObject> GOs = PropManager.Instance.GetWorldPropsByType ( typeof ( Prop_Warehouse ) );

            if (GOs == null)
            {
                Debug.LogError ( "No Warehouses found. We shouldnt run this every frame" );
                IsCompletable = false;
                yield break;
            }

            List<Prop_Warehouse> eligibleWarehouses = new List<Prop_Warehouse> ();            

            for (int i = 0; i < GOs.Count; i++)
            {
                if (GOs[i] == null) continue;
                Prop_Warehouse w = GOs[i].GetComponent<Prop_Warehouse> ();

                if (w == null) { continue; }
                if (w.inventory == null) { Debug.LogError ( "Warehouse inventory is null" ); continue; }

                float availableQuantity = w.inventory.GetAvailableQuantity ( resourceID );

                if (availableQuantity > 0)
                {
                    eligibleWarehouses.Add ( w );
                    break;
                }
            }

            // TODO: Maybe add some sort of tick system so this doesnt clog up the players client
            if (eligibleWarehouses.Count <= 0)
            {
                Debug.LogError ( "No Eligible Warehouses. We shouldnt run this every frame" );
                IsCompletable = false;
                yield break;
            }
            else
            {
                IsCompletable = true;
                yield break;
            }
        }
        else
        {
            List<GameObject> GOs = PropManager.Instance.GetWorldPropsByType ( typeof ( Prop_Warehouse ) );

            if (GOs == null)
            {
                Debug.LogError ( "No Warehouses found. We shouldnt run this every frame" );
                IsCompletable = false;
                yield break;
            }

            List<Prop_Warehouse> eligibleWarehouses = new List<Prop_Warehouse> ();

            for (int i = 0; i < GOs.Count; i++)
            {
                if (GOs[i] == null) continue;
                Prop_Warehouse w = GOs[i].GetComponent<Prop_Warehouse> ();

                if (w == null) { continue; }
                if (w.inventory == null) { Debug.LogError ( "Warehouse inventory is null" ); continue; }

                if (w.inventory.CheckCanHold ( resourceID, resourceQuantity ))
                {           
                    eligibleWarehouses.Add ( w );
                }
            }
            // TODO: Maybe add some sort of tick system so this doesnt clog up the players client
            if (eligibleWarehouses.Count <= 0)
            {
                Debug.LogError ( "No Eligible Warehouses. We shouldnt run this every frame" );
                IsCompletable = false;
                yield break;
            }
            else
            {
                IsCompletable = true;
                yield break;
            }
        }
    }
}

