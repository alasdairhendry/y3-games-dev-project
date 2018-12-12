using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inspectable_RawMaterial : Inspectable {

    private RawMaterial rawMaterial;

    protected override void Start ()
    {
        rawMaterial = GetComponent<RawMaterial> ();

        base.action = () =>
        {
            HUD_EntityInspection_Citizen_Panel panel = FindObjectOfType<HUD_EntityInspection_Citizen_Panel> ();
            //panel.ShowPanel ( rawMaterial.gameObject );

            panel.AddButtonData ( () =>
            {
                if (rawMaterial == null) return;

                rawMaterial.CreateRemovalJob ();
                panel.Hide ();

            }, "Cut Down", "Overview" );

            panel.AddButtonData ( () =>
            {
                if (rawMaterial == null) return;

                rawMaterial.RemoveOnBuildingPlaced ();
                panel.Hide ();

            }, "Remove", "Overview" );
        };
    }
}
