using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitizenHousing : MonoBehaviour {

    public CitizenBase cBase { get; protected set; }
    public bool hasHouse { get; protected set; }
    public Prop_House house { get; protected set; }

    private void Awake ()
    {
        cBase = GetComponent<CitizenBase> ();
        HousingController.onHouseBecomesEmpty += TryJoinHouse;
        HousingController.onCitizenLeavesHouse += TryJoinHouse;
    }

    private void Start ()
    {
        SearchForHouse ();
        SetInspection ();
    }

    private void SearchForHouse ()
    {
        if (hasHouse) { Debug.LogError ( "Already have house" ); return; }

        Prop_House[] houses = FindObjectsOfType<Prop_House> ();

        for (int i = 0; i < houses.Length; i++)
        {
            if (houses[i].IsBluePrint) continue;
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
        if (hasHouse) { Debug.LogError ( "Already have house" ); return; }
        if (house.IsBluePrint) { Debug.LogError ( "House was blueprint" ); return; }
        if (house.familyID != cBase.CitizenFamily.familyID && house.familyID != -1) { Debug.LogError ( "House isnt my family" ); return; };
        if (house.occupied && house.familyID != cBase.CitizenFamily.familyID) { Debug.Log ( "House was occupied" ); return; };

        if (house.AddCitizen ( cBase ))
        {
            this.house = house;
            hasHouse = true;
            OnJoinHouse (house);            
        }        
    }

    private void LeaveHouse ()
    {
        if (!hasHouse) return;
        if (house == null) return;

        house.RemoveCitizen ( cBase );
        OnLeaveHouse ( house );
        hasHouse = false;
        house = null;
    }

    private void OnJoinHouse (Prop_House house)
    {
        HousingController.onHouseBecomesEmpty -= TryJoinHouse;
        HousingController.onCitizenLeavesHouse -= TryJoinHouse;
        house.onDestroy += (prop) => { LeaveHouse (); };
    }

    private void OnLeaveHouse (Prop_House house)
    {
        HousingController.onHouseBecomesEmpty += TryJoinHouse;
        HousingController.onCitizenLeavesHouse += TryJoinHouse;
        house.onDestroy -= (prop) => { LeaveHouse (); };
    }

    private void SetInspection ()
    {
        GetComponent<Inspectable> ().SetAdditiveAction ( () =>
        {
            HUD_EntityInspection_Citizen_Panel panel = FindObjectOfType<HUD_EntityInspection_Citizen_Panel> ();

            panel.AddButtonTextData (() =>
            {
                if (!hasHouse) return;
                FindObjectOfType<CameraMovement> ().LockTo ( house.transform );
                house.GetComponentInChildren<Inspectable> ().Inspect ();
            }, () =>
            {
                if (!hasHouse) return "None";

                return house.name;

            }, "House", "Housing" );
         
        } );
    }

    private void OnDestroy ()
    {
        HousingController.onHouseBecomesEmpty -= TryJoinHouse;
    }
}
