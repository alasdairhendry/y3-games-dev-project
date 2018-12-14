using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop_Quarry : Prop_Profession {

    public ResourceInventory inventory;

    protected override void OnPlaced ()
    {
        SetInspectable ();
        SetInventory ();
    }

    // Called from Prop_Profession on built "MaxJobs" times
    protected override void CreateProfessionJobs (int index)
    {
        //Job_Lumberjack job = GetComponent<JobEntity> ().CreateJob_Lumberjack ( "Lumberjack", !HaltProduction, 5.0f, null, this, trees[index].Objects, stumps[index] );
        //professionJobs.Add ( job );
        //JobController.QueueJob ( job );
    }

    protected override void SetInspectable ()
    {
        base.SetInspectable ();

        GetComponent<Inspectable> ().SetAdditiveAction ( () =>
        {
            HUD_EntityInspection_Citizen_Panel panel = FindObjectOfType<HUD_EntityInspection_Citizen_Panel> ();

            panel.AddButtonData ( () =>
            {
                if (this == null) return;
                if (this.gameObject == null) return;

                ToggleProduction ();

            }, "Halt Production", "Overview" );

            panel.AddButtonData ( () =>
            {
                if (this.gameObject == null) return;
                if (this.inventory == null) return;

                inventory.AddItemQuantity ( 0, 10 );

            }, "Add 10 Wood", "Overview" );

            panel.AddButtonData ( () =>
            {
                if (this.gameObject == null) return;
                if (this.inventory == null) return;

                inventory.RemoveItemQuantity ( 0, 10 );

            }, "Remove 10 Wood", "Overview" );

            panel.AddTextData ( () =>
            {
                if (inventory == null) return "0.00";
                if (inventory.inventoryOverall == null) return "0.00";
                return inventory.inventoryOverall[0].ToString ( "0.00" );
            }, "Wood", "Overview" );

            panel.AddTextData ( () =>
            {
                if (inventory == null) return "0.00";
                if (inventory.inventoryOverall == null) return "0.00";
                return inventory.inventoryOverall[1].ToString ( "0.00" );
            }, "Brick", "Overview" );
        } );
    }

    private void SetInventory ()
    {
        inventory = new ResourceInventory ( 10 );
    }
}
