using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop_Campfire : Prop {

    public bool isLit { get; protected set; }

    [SerializeField] private Light fireLight;
    private JobEntity jobEntity;

    //private DEBUG_DrawSnowDepressionsWithMouse snowDepressions;

    protected override void OnPlaced ()
    {
        jobEntity = GetComponent<JobEntity> ();

        SetInspectable ();
        Tick_CheckWood ( 0 );
        GameTime.RegisterGameTick ( Tick_CheckWood );        
    }

    protected override void Update ()
    {
        if (isLit)
            SnowController.Instance.DrawDepression ( 1000, 0.1f, transform.position );
    }

    private void Tick_CheckWood (int relativeTick)
    {
        if (relativeTick % 5 != 0) return;

        if (inventory.CheckIsEmpty ( 0 ))
        {
            isLit = false;
            OnLight ();
        }
        else
        {
            isLit = true;
            OnLight ();
            inventory.RemoveItemQuantity ( 0, 0.15f * GameTime.DeltaGameTime );
        }

        if (jobEntity.HasNonNullJob ()) return;
        {
            if (!inventory.CheckHasQuantityAvailable ( 0, 0.5f ))
            {
                jobEntity.CreateJob_Haul ( "Haul Wood To Campfire", true, 5.0f, null, 0, 1, this, inventory );
            }
        }
    }

    private void OnLight ()
    {
        if (!buildable.IsComplete) isLit = false;

        if (isLit)
        {
            fireLight.gameObject.SetActive ( true );
        }
        else
        {
            fireLight.gameObject.SetActive ( false );
        }
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

                inventory.AddItemQuantity ( 0, 1, this.transform, data.UIOffsetY );

            }, "Add 1 Wood", "Inventory" );

            panel.AddTextData ( (pair) =>
            {
                if (inventory == null) return "0.00";
                if (inventory.inventoryOverall == null) return "0.00";
                return inventory.inventoryOverall[0].ToString ( "0.00" );
            }, "Wood", "Inventory" );
        } );
    }

    protected override void OnDestroy ()
    {
        base.OnDestroy ();
        GameTime.UnRegisterGameTick ( Tick_CheckWood );
    }
}
