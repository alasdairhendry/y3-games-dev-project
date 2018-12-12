using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop_Warehouse : Prop {

    public ResourceInventory inventory;

    protected override void Start ()
    {
        SetInspectable ();
        SetInventory ();        
    }

    protected override void SetInspectable ()
    {
        base.SetInspectable ();
        GetComponent<Inspectable> ().SetAdditiveAction ( () =>
        {
            HUD_EntityInspection_Citizen_Panel panel = FindObjectOfType<HUD_EntityInspection_Citizen_Panel> ();

            panel.AddButtonData ( () =>
            {
                if (this.gameObject == null) return;
                if (this.inventory == null) return;

                inventory.AddItemQuantity ( 0, 10 );

            }, "Add 10 Wood", "Inventory" );

            panel.AddButtonData ( () =>
            {
                if (this.gameObject == null) return;
                if (this.inventory == null) return;

                inventory.AddItemQuantity ( 1, 10 );

            }, "Add 10 Brick", "Inventory" );

            panel.AddTextData ( () =>
            {
                if (inventory == null) return "00";
                if (inventory.inventoryOverall == null) return "00";
                return inventory.inventoryOverall[0].ToString ( "00" );
            }, "Wood", "Inventory" );

            panel.AddTextData ( () =>
            {
                if (inventory == null) return "00";
                if (inventory.inventoryOverall == null) return "00";
                return inventory.inventoryOverall[1].ToString ( "00" );
            }, "Brick", "Inventory" );
        } );
    }

    private void SetInventory ()
    {
        inventory = new ResourceInventory ();
        inventory.AddItemQuantity ( 0, 6 );
        inventory.AddItemQuantity ( 1, 2 );
    }
}
