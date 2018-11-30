using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryDisplayer : MonoBehaviour {

    [SerializeField] private Character targetCitizen;
    [SerializeField] private Prop_Warehouse targetWarehouse;

    private TextMesh textMesh;        
    private ResourceInventory inventory;

    private void Start ()
    {        
        textMesh = GetComponent<TextMesh> ();

        if(targetCitizen == null && targetWarehouse== null)
        {
            targetCitizen = GetComponentInParent<Character> ();
            targetWarehouse = GetComponentInParent<Prop_Warehouse> ();
        }

        if (targetCitizen != null) inventory = targetCitizen.Inventory;
        else if (targetWarehouse != null) inventory = targetWarehouse.inventory;
    }

    void Update () {
        string text = "";

        if(targetCitizen != null && targetWarehouse == null)
        {
            if (targetCitizen.GetCurrentJob != null)
            {
                text += targetCitizen.GetCurrentJob.AgentJobStatus;
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
