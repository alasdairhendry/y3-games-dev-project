using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop_Smithery : Prop_Profession
{
    [SerializeField] private GameObject smelterPoint;
    [SerializeField] private GameObject anvilPoint;

    public enum WorkType { Smelting, Smithing }
    public WorkType workType { get; protected set; } = WorkType.Smelting;

    protected override void OnPlaced ()
    {
        base.OnPlaced ();
    }

    protected override void SetResources ()
    {
        resourceIDToGive = 4;
        resourceIDToConsume = 3;

        giveAmount = 1;
        consumeAmount = 1;

        ProductionRequired = 20.0f;
    }

    // Called from Prop_Profession on built "MaxJobs" times
    protected override void CreateProfessionJobs (int index)
    {
        Job_Blacksmith job = GetComponent<JobEntity> ().CreateJob_Blacksmith ( "Blacksmithing", !HaltProduction, 5.0f, null, this, smelterPoint, anvilPoint );
        professionJobs.Add ( job );
    }

    protected override void SetInspectable ()
    {
        base.SetInspectable ();

        GetComponent<Inspectable> ().SetAdditiveAction ( () =>
        {
            HUD_EntityInspection_Citizen_Panel panel = FindObjectOfType<HUD_EntityInspection_Citizen_Panel> ();

            if (this == null) return;
            if (this.gameObject == null) return;

            panel.AddDropdownData ( (index, options) =>
            {
                WorkType w = (WorkType)index;
                if (w == workType) return;

                workType = (WorkType)index;

                if (workType == WorkType.Smithing)
                {
                    resourceIDToGive = 8;
                    resourceIDToConsume = 4;

                    giveAmount = 1;
                    consumeAmount = 2;

                    for (int i = 0; i < base.professionJobs.Count; i++)
                    {
                        Job_Blacksmith b = (Job_Blacksmith)professionJobs[i];
                        b.SetIsSmelting ( false );
                        b.CheckMaterials ();                       
                    }

                    base.DestroyMarketJobs ();
                }
                else
                {
                    resourceIDToGive = 4;
                    resourceIDToConsume = 3;

                    giveAmount = 1;
                    consumeAmount = 1;

                    for (int i = 0; i < base.professionJobs.Count; i++)
                    {
                        Job_Blacksmith b = (Job_Blacksmith)professionJobs[i];
                        b.SetIsSmelting ( true );
                        b.CheckMaterials ();
                    }

                    base.DestroyMarketJobs ();
                }

                base.Inspectable.Inspect ();

            }, (pair) =>
            {
                return workType.ToString ();
            }, "Production Type", "Overview",
            WorkType.Smelting.ToString(),
            WorkType.Smithing.ToString() );

        } );
    }
}
