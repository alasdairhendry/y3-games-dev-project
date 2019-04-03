using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop_StonemasonHut : Prop_Profession {

    [SerializeField] private GameObject plinth;
    [SerializeField] private GameObject plinthPoint;

    protected override void OnPlaced ()
    {
        base.OnPlaced ();
    }

    protected override void SetResources ()
    {
        resourceIDToGive = 2;
        resourceIDToConsume = 1;

        giveAmount = 1;
        consumeAmount = 2;

        ProductionRequired = 20.0f;
    }
    protected override void SetInspectable ()
    {        
        GetComponent<Inspectable> ().SetAdditiveAction ( () =>
        {
            HUD_EntityInspection_Citizen_Panel panel = FindObjectOfType<HUD_EntityInspection_Citizen_Panel> ();

            if (this == null) return;
            if (this.gameObject == null) return;

            panel.AddButtonData ( () =>
            {
                if (this == null) return;
                if (this.buildable == null) return;
                if (this.gameObject == null) return;
                this.buildable.CompleteInspectorDEBUG ();

            }, "Complete", "Any" );


            panel.AddButtonData ( () =>
            {
                if (this == null) return;
                if (this.gameObject == null) return;
                DestroyProp ();

            }, "Destroy", "Any" );

            panel.AddButtonData ( () =>
                {
                    ToggleProduction ();

                }, "Halt Production", "Overview" );

            panel.AddButtonData ( () =>
            {
                if (this.gameObject == null) return;
                if (this.inventory == null) return;

                inventory.AddItemQuantity ( 1, 1, this.transform, data.UIOffsetY );

            }, "Add 1 Brick", "Overview" );

            panel.AddButtonData ( () =>
            {
                if (this.gameObject == null) return;
                if (this.inventory == null) return;

                inventory.RemoveItemQuantity ( 1, 1, this.transform, data.UIOffsetY );

            }, "Remove 1 Brick", "Overview" );

            panel.AddButtonData ( () =>
            {
                if (this.gameObject == null) return;
                if (this.inventory == null) return;

                inventory.AddItemQuantity ( 2, 1, this.transform, data.UIOffsetY );

            }, "Add 1 Stone", "Overview" );

            panel.AddButtonData ( () =>
            {
                if (this.gameObject == null) return;
                if (this.inventory == null) return;

                inventory.RemoveItemQuantity ( 2, 1, this.transform, data.UIOffsetY );

            }, "Remove 1 Stone", "Overview" );

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


    // Called from Prop_Profession on built "MaxJobs" times
    protected override void CreateProfessionJobs (int index)
    {
        Job_Stonemason job = GetComponent<JobEntity> ().CreateJob_StoneMason ( "Stone Masonry", !HaltProduction, 5.0f, null, this, plinth, plinthPoint);
        professionJobs.Add ( job );
    }
}
