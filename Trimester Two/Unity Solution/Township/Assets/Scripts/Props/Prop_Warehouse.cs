using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop_Warehouse : Prop {

    //public ResourceInventory inventory;
    //private ResourceInventory inventory;

    //public new ResourceInventory inv { get { Debug.LogError ( "DO NOT USE THIS PLS" ); return null; } }

    protected override void OnPlaced ()
    {
        SetInspectable ();
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
                if (WarehouseController.Instance.Inventory == null) return;

                WarehouseController.Instance.Inventory.AddItemQuantity ( 0, 10, this.transform, data.UIOffsetY );

            }, "Add 10 Wood", "Inventory" );

            panel.AddButtonData ( () =>
            {
                if (this.gameObject == null) return;
                if (WarehouseController.Instance.Inventory == null) return;

                WarehouseController.Instance.Inventory.AddItemQuantity ( 1, 10, this.transform, data.UIOffsetY );

            }, "Add 10 Brick", "Inventory" );

            panel.AddButtonData ( () =>
            {
                if (this.gameObject == null) return;
                if (WarehouseController.Instance.Inventory == null) return;

                WarehouseController.Instance.Inventory.AddItemQuantity ( 2, 10, this.transform, data.UIOffsetY );

            }, "Add 10 Stone", "Inventory" );

            panel.AddTextData ( (pair) =>
            {
                if (WarehouseController.Instance.Inventory == null) return "0.00";
                if (WarehouseController.Instance.Inventory.inventoryOverall == null) return "0.00";
                return WarehouseController.Instance.Inventory.inventoryOverall[0].ToString ( "0.00" );
            }, "Wood", "Inventory" );

            panel.AddTextData ( (pair) =>
            {
                if (WarehouseController.Instance.Inventory == null) return "0.00";
                if (WarehouseController.Instance.Inventory.inventoryOverall == null) return "0.00";
                return WarehouseController.Instance.Inventory.inventoryOverall[1].ToString ( "0.00" );
            }, "Brick", "Inventory" );

            panel.AddTextData ( (pair) =>
            {
                if (WarehouseController.Instance.Inventory == null) return "0.00";
                if (WarehouseController.Instance.Inventory.inventoryOverall == null) return "0.00";
                return WarehouseController.Instance.Inventory.inventoryOverall[2].ToString ( "0.00" );
            }, "Stone", "Inventory" );
        } );
    }

    //protected override void SetInventory ()
    //{
    //    inventory = new ResourceInventory ( float.MaxValue );
    //    inventory.AddItemQuantity ( 0, 6 );
    //    inventory.AddItemQuantity ( 1, 2 );
    //}
}
