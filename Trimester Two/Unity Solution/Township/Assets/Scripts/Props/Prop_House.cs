using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop_House : Prop {

    public List<CitizenBase> occupants { get; protected set; }
    public bool occupied { get; protected set; }
    public int familyID { get; protected set; }
    public int maxOccupants { get; protected set; }

    protected override void OnPlaced ()
    {
        SetInspectable ();

        occupants = new List<CitizenBase> ();
        occupied = false;
        familyID = -1;
        maxOccupants = 4;
        buildable.onComplete += () => { HousingController.OnHouseBecomesEmpty ( this ); };
    }

    protected override void SetInspectable ()
    {
        base.SetInspectable ();
    }

    public virtual bool AddCitizen(CitizenBase citizen)
    {
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
        Debug.Log ( "Remove Citizen" );
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
}
