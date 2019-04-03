using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop_House : Prop {

    public List<CitizenBase> occupants { get; protected set; }
    public bool occupied { get; protected set; }
    public int familyID { get; protected set; }
    public int maxOccupants { get; protected set; }

    private float woodDrainRatePerOccupant = 0.015f;
    private float woodAddQuantityPerOccupant = 1.0f;
    private JobEntity jobEntity;

    public bool allowOccupants { get; protected set; } = true;

    public bool hasAddWoodJob = false;

    protected override void OnPlaced ()
    {
        SetInspectable ();

        jobEntity = GetComponent<JobEntity> ();

        occupants = new List<CitizenBase> ();
        occupied = false;
        familyID = -1;
        maxOccupants = 4;
        buildable.onComplete += () => { HousingController.OnHouseBecomesEmpty ( this ); };
        GameTime.RegisterGameTick ( Tick_CheckWood );

        PropManager.Instance.GetPropDataByName ( "Lumberjack's Hut" ).Unlock ();
    }

    protected override void SetInspectable ()
    {
        base.SetInspectable ();

        base.Inspectable.SetAdditiveAction ( () => {

            HUD_EntityInspection_Citizen_Panel panel = FindObjectOfType<HUD_EntityInspection_Citizen_Panel> ();

            panel.AddTextData ( (pair) =>
            {
                if (inventory == null) return "";
                return inventory.GetAvailableQuantity ( 0 ).ToString ( "0.0" );
            }, "Wood", "Overview" );

            for (int i = 0; i < occupants.Count; i++)
            {
                if (occupants[i] == null) continue;
                int x = i;

                panel.AddButtonTextData ( () =>
                {
                    if (x >= occupants.Count) return;

                    occupants[x].Inspectable.InspectAndLockCamera ();
                }, (pair) =>
                {
                    if (x >= occupants.Count) return "Unknown";

                    return occupants[x].CitizenFamily.thisMember.fullName;
                }, "Occupant", "Overview" );
            }
        } );
    }

    public virtual bool AddCitizen(CitizenBase citizen)
    {
        if (!allowOccupants) return false;

        if(occupants.Count >= maxOccupants)
        {            
            return false;
        }

        if (citizen.CitizenFamily.familyID == familyID || familyID == -1)
        {
            if(familyID == -1)
            {
                familyID = citizen.CitizenFamily.familyID;
                HousingController.OnHouseGetsNewFamily ( this );
            }

            occupants.Add ( citizen );
            citizen.OnCitizenDied += (cBase) => { RemoveCitizen ( cBase ); };
            occupied = true;

            if (occupants.Count >= maxOccupants)
            {
                HousingController.OnHouseBecomesFull ( this );
            }

            return true;
        }

        return false;    
    }

    public virtual void RemoveCitizen(CitizenBase citizen)
    {
        //Debug.Log ( "Remove Citizen" );
        if (!occupants.Contains ( citizen )) return;
        occupants.Remove ( citizen );
        HousingController.OnCitizenLeavesHouse ( this );

        if(occupants.Count <= 0)
        {
            familyID = -1;
            occupied = false;
            HousingController.OnHouseBecomesEmpty ( this );
        }
    }

    protected override void Update ()
    {
        base.Update ();

        if (IsBluePrint) return;
        RemoveWood ();
    }

    private void RemoveWood ()
    {
        if (inventory == null) { Debug.LogError ( "Why is the inventory null" ); return; }
        if (occupants == null) { Debug.LogError ( "Why is the occupants null" ); return; }
        if (occupants.Count <= 0) return;

        if (!inventory.CheckIsEmpty ( 0 ))
        {
            float drainRate = (woodDrainRatePerOccupant * occupants.Count) * Mathf.InverseLerp ( TemperatureController.temperatureMax, TemperatureController.temperatureMin, TemperatureController.Temperature );
            inventory.RemoveItemQuantity ( 0, drainRate * GameTime.DeltaGameTime );
        }
    }

    private void Tick_CheckWood (int relativeTick)
    {
        if (relativeTick % 5 != 0) return;
        if(inventory == null) { Debug.LogError ( "Why is the inventory null" ); return; }
        if(jobEntity == null) { Debug.LogError ( "Why is the jobEntity null" ); return; }
        if(occupants == null) { Debug.LogError ( "Why is the occupants null" ); return; }
        if (occupants.Count <= 0) return;

        if (inventory.CheckIsEmpty ( 0 ))
        {
            if (!hasAddWoodJob)
            {
                hasAddWoodJob = true;
                jobEntity.CreateJob_Haul ( "Haul Wood To House", true, 5.0f, () => { hasAddWoodJob = false; }, 0, woodAddQuantityPerOccupant * occupants.Count, this, inventory );
            }
        }
        else
        {
            //float drainRate = (woodDrainRatePerOccupant * occupants.Count) * Mathf.InverseLerp ( TemperatureController.temperatureMin, TemperatureController.temperatureMax, TemperatureController.Temperature );
            //inventory.RemoveItemQuantity ( 0, drainRate * GameTime.DeltaGameTime );
        }
    }

    public override void DestroyProp ()
    {
        allowOccupants = false;

        for (int i = 0; i < occupants.Count; i++)
        {
            occupants[i].CitizenHousing.LeaveHouse ( false );
            Debug.Log ( "Leave house - " + occupants[i].CitizenFamily.thisMember.fullName );
        }

        base.DestroyProp ();
    }

    protected override void OnDestroy ()
    {
        base.OnDestroy ();
        GameTime.UnRegisterGameTick ( Tick_CheckWood );
    }
}
