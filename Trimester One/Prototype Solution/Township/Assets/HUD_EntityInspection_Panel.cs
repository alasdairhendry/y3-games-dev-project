using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD_EntityInspection_Panel : UIPanel {
        
    public enum InspectionType { Citizen }    

    public void ShowPanel (InspectionType inspection)
    {
        base.Show ();

        switch (inspection)
        {
            case InspectionType.Citizen:
                GetComponentInChildren<HUD_EntityInspection_Citizen_Panel> ().Show ();
                break;
        }
    }  
}
