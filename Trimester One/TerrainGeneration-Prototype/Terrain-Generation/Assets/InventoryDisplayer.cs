using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryDisplayer : MonoBehaviour {

    private TextMesh textMesh;
    [SerializeField] private Behaviour target;
    [SerializeField] private string inventoryParam;

    private ResourceInventory inventory;
    	
	void Update () {
        if(inventory == null && target.enabled)
        {
            if(target == null)
            {
                Debug.LogError ( "null target" );
                return;
            }
            textMesh = GetComponent<TextMesh> ();
            inventory = (ResourceInventory)target.GetType ().GetField ( inventoryParam ).GetValue ( target );
        }
        else { return; }

        if (inventory.inventory == null) return;

        string text = "";

        foreach (int key in inventory.inventory.Keys)
        {
            text += inventory.inventory[key] + " - " + inventory.GetResourceByID(key).name;
            text += "/n";
        }

        textMesh.text = text;
	}
}
