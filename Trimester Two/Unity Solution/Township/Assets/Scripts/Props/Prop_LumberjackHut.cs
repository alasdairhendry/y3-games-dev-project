﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop_LumberjackHut : Prop_Profession {

    [SerializeField] private List<GameObjectList> trees = new List<GameObjectList> ();
    [SerializeField] private List<GameObject> stumps = new List<GameObject> ();

    protected override void OnPlaced ()
    {
        base.OnPlaced ();
    }

    protected override void SetResources ()
    {
        resourceIDToGive = 0;
        resourceIDToConsume = -1;

        giveAmount = 1;
        consumeAmount = 0;

        ProductionRequired = 20.0f;
    }

    // Called from Prop_Profession on built "MaxJobs" times
    protected override void CreateProfessionJobs (int index)
    {
        Job_Lumberjack job = GetComponent<JobEntity> ().CreateJob_Lumberjack ( "Lumberjack", !HaltProduction, 5.0f, null, this, trees[index].Objects, stumps[index] );
        professionJobs.Add ( job );
        JobController.QueueJob ( job );
    }

    protected override void SetInspectable ()
    {
        base.SetInspectable ();

        //GetComponent<Inspectable> ().SetAdditiveAction ( () =>
        //{
        //    HUD_EntityInspection_Citizen_Panel panel = FindObjectOfType<HUD_EntityInspection_Citizen_Panel> ();

        //    panel.AddButtonData ( () =>
        //    {
        //        if (this == null) return;
        //        if (this.gameObject == null) return;

        //        ToggleProduction ();

        //    }, "Halt Production", "Overview" );

        //    panel.AddButtonData ( () =>
        //    {
        //        if (this.gameObject == null) return;
        //        if (this.inventory == null) return;

        //        inventory.AddItemQuantity ( 0, 10 );

        //    }, "Add 10 Wood", "Overview" );

        //    panel.AddButtonData ( () =>
        //    {
        //        if (this.gameObject == null) return;
        //        if (this.inventory == null) return;

        //        inventory.RemoveItemQuantity ( 0, 10 );

        //    }, "Remove 10 Wood", "Overview" );


        //    panel.AddTextData ( () =>
        //    {
        //        if (inventory == null) return "0.00";
        //        if (inventory.inventoryOverall == null) return "0.00";
        //        return inventory.inventoryOverall[0].ToString ( "0.00" );
        //    }, "Wood", "Overview" );

        //    panel.AddTextData ( () =>
        //    {
        //        if (inventory == null) return "0.00";
        //        if (inventory.inventoryOverall == null) return "0.00";
        //        return inventory.inventoryOverall[1].ToString ( "0.00" );
        //    }, "Brick", "Overview" );
        //} );
    }
}

[System.Serializable]
public class GameObjectList
{
    [SerializeField] private List<GameObject> objects = new List<GameObject> ();
    public List<GameObject> Objects { get { return objects; } }

    public GameObject this[int key]
    {
        get
        {
            return objects[key];
        }
        set
        {
            objects[key] = value;
        }
    }
}
