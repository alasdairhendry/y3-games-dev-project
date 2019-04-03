using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitizenHousing : MonoBehaviour {

    public CitizenBase cBase { get; protected set; }
    public bool hasHouse { get; protected set; }
    public Prop_House house { get; protected set; }

    [HideInInspector] public bool allowLookForHouse = true;

    private void Awake ()
    {
        cBase = GetComponent<CitizenBase> ();
        HousingController.onHouseBecomesEmpty += TryJoinHouse;
        HousingController.onCitizenLeavesHouse += TryJoinHouse;

        GetComponent<IconDisplayer> ().AddIconGeneric ( IconDisplayer.IconType.NoHouse );
    }

    private void Start ()
    {
        SearchForHouse ();
        SetInspection ();
    }

    private void SearchForHouse ()
    {
        if (hasHouse) { return; }

        Prop_House[] houses = FindObjectsOfType<Prop_House> ();

        for (int i = 0; i < houses.Length; i++)
        {
            if (!houses[i].allowOccupants) continue;
            if (houses[i].IsBluePrint) continue;
            if (!houses[i].buildable.IsComplete) continue;
            if (houses[i].familyID != cBase.CitizenFamily.familyID && houses[i].familyID != -1) continue;
            if (houses[i].occupied && houses[i].familyID != cBase.CitizenFamily.familyID) continue;

            if(houses[i].AddCitizen ( cBase ))
            {
                house = houses[i];
                hasHouse = true;
                OnJoinHouse ( houses[i] );
                return;
            }
        }
    }

    private void TryJoinHouse(Prop_House house)
    {
        if (!allowLookForHouse) return;
        if (!house.allowOccupants) { return; }
        if (hasHouse) { return; }
        if (house.IsBluePrint) { return; }
        if (!house.buildable.IsComplete) { return; }
        if (house.familyID != cBase.CitizenFamily.familyID && house.familyID != -1) { return; };
        if (house.occupied && house.familyID != cBase.CitizenFamily.familyID) { return; };

        if (house.AddCitizen ( cBase ))
        {
            this.house = house;
            hasHouse = true;
            OnJoinHouse (house);
        }        
    }

    public void LeaveHouse (bool callRemoveCitizenOnHouse)
    {
        if (!hasHouse) return;
        if (house == null) return;

        if (callRemoveCitizenOnHouse)
            house.RemoveCitizen ( cBase );

        hasHouse = false;

        Prop_House call = house;
        house = null;

        OnLeaveHouse ( call );
    }

    private void OnJoinHouse (Prop_House house)
    {
        HousingController.onHouseBecomesEmpty -= TryJoinHouse;
        HousingController.onCitizenLeavesHouse -= TryJoinHouse;

        GetComponent<IconDisplayer> ().RemoveIconByType ( IconDisplayer.IconType.NoHouse );
    }

    private void OnLeaveHouse (Prop_House house)
    {
        SearchForHouse ();

        if(hasHouse && house != null)
        {
            return;
        }

        HousingController.onHouseBecomesEmpty += TryJoinHouse;
        HousingController.onCitizenLeavesHouse += TryJoinHouse;

        GetComponent<IconDisplayer> ().AddIconGeneric ( IconDisplayer.IconType.NoHouse );
    }

    private void SetInspection ()
    {
        GetComponent<Inspectable> ().SetAdditiveAction ( () =>
        {
            HUD_EntityInspection_Citizen_Panel panel = FindObjectOfType<HUD_EntityInspection_Citizen_Panel> ();

            panel.AddButtonTextData (() =>
            {
                if (!hasHouse) return;
                if (house == null) return;
                house.Inspectable.InspectAndLockCamera ();
                house.GetComponentInChildren<Inspectable> ().Inspect ();
            }, (pair) =>
            {
                if (!hasHouse) return "None";
                if (house == null) return "None";

                return house?.name;

            }, "House", "Housing" );
         
        } );
    }

    private void OnDestroy ()
    {
        HousingController.onHouseBecomesEmpty -= TryJoinHouse;
    }
}
