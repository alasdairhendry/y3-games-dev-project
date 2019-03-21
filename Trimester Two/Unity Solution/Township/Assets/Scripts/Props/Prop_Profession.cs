using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop_Profession : Prop {    

    [SerializeField] protected int MaxJobs = 2;
    protected int UserDefinedNumberJobs = 2;

    public bool HaltProduction { get; protected set; }
    public List<Job> professionJobs { get; protected set; }

    [SerializeField] protected float ProductionRequired = 20.0f;
    public float CurrentProduction { get; protected set; }

    public int resourceIDToConsume { get; protected set; }
    public int resourceIDToGive { get; protected set; }

    public int consumeAmount { get; protected set; }
    public int giveAmount { get; protected set; }

    protected bool createdSupplyJob = false;
    protected bool createdCollectJob = false;

    public override void OnBuilt ()
    {
        professionJobs = new List<Job> ();

        for (int i = 0; i < MaxJobs; i++)
        {
            CreateProfessionJobs (i);
        }
    }

    protected override void OnPlaced ()
    {        
        SetResources ();
        SetInspectable ();

        GameTime.RegisterGameTick ( Tick );
    }

    protected virtual void SetResources ()
    {
        Debug.LogError ( "override this" );
    }

    protected override void SetInspectable ()
    {
        base.SetInspectable ();

        GetComponent<Inspectable> ().SetAdditiveAction ( () =>
        {
            HUD_EntityInspection_Citizen_Panel panel = FindObjectOfType<HUD_EntityInspection_Citizen_Panel> ();

            if (this == null) return;
            if (this.gameObject == null) return;

            panel.AddButtonData ( () =>
            {
                ToggleProduction ();

            }, "Halt Production", "Overview" );

            panel.AddButtonData ( () =>
            {
                if (this.gameObject == null) return;
                if (this.inventory == null) return;

                inventory.AddItemQuantity ( 0, 1, this.transform, data.UIOffsetY );

            }, "Add 1 Wood", "Overview" );

            panel.AddButtonData ( () =>
            {
                if (this.gameObject == null) return;
                if (this.inventory == null) return;

                inventory.RemoveItemQuantity ( 0, 1, this.transform, data.UIOffsetY );

            }, "Remove 1 Wood", "Overview" );

            if (resourceIDToGive >= 0)
            {
                if (inventory.CheckIsFull ( resourceIDToGive ))
                {
                    panel.AddTextData ( (pair) =>
                    {
                        return "Capacity Reached";

                    }, "Warning", "Any" );
                }
            }

            if (resourceIDToConsume >= 0)
            {
                if (inventory.CheckIsEmpty ( resourceIDToConsume ))
                {
                    panel.AddTextData ( (pair) =>
                    {
                        return "No " + ResourceManager.Instance.GetResourceByID ( resourceIDToConsume ).name + " available";
                    }, "Warning", "Any" );
                }
            }

            panel.AddTextData ( (pair) =>
            {
                return ((CurrentProduction / ProductionRequired) * 100.0f).ToString ( "0" ) + "%";
            }, "Production", "Overview" );

            if (resourceIDToGive >= 0)
            {
                panel.AddTextData ( (pair) =>
                {
                    if (inventory == null) return "0.00";
                    if (inventory.inventoryOverall == null) return "0.00";
                    return inventory.inventoryOverall[resourceIDToGive].ToString ( "0.00" );
                }, ResourceManager.Instance.GetResourceByID ( resourceIDToGive ).name, "Overview" );
            }

            if (resourceIDToConsume >= 0)
            {
                panel.AddTextData ( (pair) =>
                {
                    if (inventory == null) return "0.00";
                    if (inventory.inventoryOverall == null) return "0.00";
                    return inventory.inventoryOverall[resourceIDToConsume].ToString ( "0.00" );
                }, ResourceManager.Instance.GetResourceByID ( resourceIDToConsume ).name, "Overview" );
            }
        } );
    }

    protected virtual void Tick (int relativeTick)
    {
        if (buildable == null) return;
        if (!buildable.IsComplete) return;
        if (HaltProduction) return;
        CheckJobs ();
    }

    protected virtual void CreateProfessionJobs(int index)
    {
        
    }

    protected virtual void CheckJobs ()
    {
        if (resourceIDToConsume < 0 && resourceIDToGive < 0)
        {
            Debug.LogError ( "Prop: " + name + " does not give or take any resources" );
            return;
        }

        if (resourceIDToConsume >= 0)
        {
            if (!inventory.CheckIsFull ( resourceIDToConsume ))
            {
                if (!createdSupplyJob)
                {
                    GetComponent<JobEntity> ().CreateJob_MarketCart ( "Supply " + ResourceManager.Instance.GetResourceByID(resourceIDToConsume).name + " to " + this.worldEntity.EntityName, true,
                        5.0f, () => { createdSupplyJob = false; }, this, inventory, resourceIDToConsume, (int)inventory.EntryCapacity, true );
                    createdSupplyJob = true;                    
                }
            }
        }

        if (resourceIDToGive >= 0)
        {
            if (!inventory.CheckIsEmpty ( resourceIDToGive ))
            {
                if (!createdCollectJob)
                {
                    GetComponent<JobEntity> ().CreateJob_MarketCart ( "Collect " + ResourceManager.Instance.GetResourceByID ( resourceIDToGive ).name + " from " + this.worldEntity.EntityName, true,
                        5.0f, () => { createdCollectJob = false; }, this, inventory, resourceIDToGive, (int)inventory.EntryCapacity, false );
                    createdCollectJob = true;
                }
            }
        }
    }

    public void AddProduction(float productionModifier = 1)
    {
        if (productionModifier <= 0) return;
        CurrentProduction += GameTime.DeltaGameTime * productionModifier;

        if(CurrentProduction >= ProductionRequired)
        {
            CurrentProduction = 0.0f;
            OnProductionComplete ();
        }
    }

    protected void OnProductionComplete ()
    {
        if (resourceIDToConsume < 0 && resourceIDToGive < 0)
        {
            Debug.LogError ( "Prop: " + name + " does not give or take any resources" );
            return;
        }

        if (resourceIDToConsume < 0)
        {
            // TODO: - If this returns something, we are full, alert the user
            inventory.AddItemQuantity ( resourceIDToGive, giveAmount, this.transform, data.UIOffsetY );
        }
        else
        {
            if (inventory.CheckHasQuantityAvailable ( resourceIDToConsume, consumeAmount ))
            {
                inventory.RemoveItemQuantity ( resourceIDToConsume, consumeAmount, this.transform, data.UIOffsetY );
                inventory.AddItemQuantity ( resourceIDToGive, giveAmount, this.transform, data.UIOffsetY );
            }
            else
            {
                Debug.Log ( "Prop: " + name + " does not have enough consumer resources" );
            }
        }
    }

    public virtual void ToggleProduction()
    {
        HaltProduction = !HaltProduction;
        Debug.Log ( "production = " + !HaltProduction );
        OnProductionStatusChanged ();
    }

    public virtual void OnProductionStatusChanged ()
    {
        for (int i = 0; i < professionJobs.Count; i++)
        {
            professionJobs[i].SetOpen ( !HaltProduction );
        }
    }
}
