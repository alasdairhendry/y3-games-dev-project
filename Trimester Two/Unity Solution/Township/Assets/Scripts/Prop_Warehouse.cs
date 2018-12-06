using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop_Warehouse : Prop {

    public ResourceInventory inventory;

    private void Start ()
    {        
        inventory = new ResourceInventory ();
        inventory.AddItemQuantity ( 0, 6 );
        inventory.AddItemQuantity ( 1, 2 );

        GetComponent<Inspectable> ().SetAdditiveAction ( () =>
        {
            HUD_EntityInspection_Citizen_Panel panel = FindObjectOfType<HUD_EntityInspection_Citizen_Panel> ();            

            panel.AddButtonData ( () =>
            {
                if (this.gameObject == null) return;
                if (this.inventory == null) return;

                inventory.AddItemQuantity ( 0, 10 );

            }, "Add 10 Wood" );

            panel.AddButtonData ( () =>
            {
                if (this.gameObject == null) return;
                if (this.inventory == null) return;

                inventory.AddItemQuantity ( 1, 10 );

            }, "Add 10 Brick" );

            panel.AddTextData ( () =>
            {
                if (inventory == null) return "00";
                if (inventory.inventoryOverall == null) return "00";
                return inventory.inventoryOverall[0].ToString ( "00" );
            }, "Wood" );

            panel.AddTextData ( () =>
            {
                if (inventory == null) return "00";
                if (inventory.inventoryOverall == null) return "00";
                return inventory.inventoryOverall[1].ToString ( "00" );
            }, "Brick" );
        } );
    }
}
