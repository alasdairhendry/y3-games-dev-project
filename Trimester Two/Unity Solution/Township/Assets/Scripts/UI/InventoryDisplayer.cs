using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryDisplayer : MonoBehaviour {

    [SerializeField] private CitizenBase cBase;
    [SerializeField] private Prop_Warehouse targetWarehouse;

    private TextMesh textMesh;        
    private ResourceInventory inventory;

    private void Start ()
    {        
        textMesh = GetComponent<TextMesh> ();

        if(cBase == null && targetWarehouse== null)
        {
            cBase = GetComponentInParent<CitizenBase> ();
            targetWarehouse = GetComponentInParent<Prop_Warehouse> ();
        }

        if (cBase != null) inventory = cBase.Inventory;
        else if (targetWarehouse != null) inventory = WarehouseController.Instance.Inventory;
    }

    void Update () {
        string text = "";

        if(cBase != null && targetWarehouse == null)
        {
            if (cBase.CitizenJob.GetCurrentJob != null)
            {
                text += cBase.CitizenJob.GetCurrentJob.AgentJobStatus;
                text += "\n";
            }
            else
            {
                text += "Idling";
                text += "\n";
            }
        }

        if(inventory!= null)
        {            
            foreach (int key in inventory.inventoryOverall.Keys)
            {
                text += inventory.inventoryOverall[key] + " " + ResourceManager.Instance.GetResourceByID ( key ).name;
                text += "\n";
            }
        }
        
        textMesh.text = text;
	}
}
