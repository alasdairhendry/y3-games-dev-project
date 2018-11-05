using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop_Warehouse : Prop {

    public ResourceInventory inventory;

    private void Start ()
    {
        inventory = new ResourceInventory ();
    }

    private void Update ()
    {
        float ran = Random.Range (2, 7);


        if (Input.GetKeyDown ( KeyCode.W ))
        {
            if (inventory.CheckIsFull ( 0 )) { Debug.Log ( "Resource entry is full. Returning..." ); return; }
            Debug.Log ( "Adding " + ran.ToString ( "0.00" ) + " quantity to " + ResourceManager.Instance.GetResourceByID ( 0 ) );
            float r = inventory.AddItemQuantity ( 0, ran );
            Debug.Log ( "Returned " + r.ToString ( "0.00" ) + " quantity" );
        }

        if (Input.GetKeyDown ( KeyCode.E ))
        {
            if (inventory.CheckIsEmpty ( 0 )) { Debug.Log ( "Resource entry is empty. Returning..." ); return; }
            Debug.Log ( "Removing " + ran.ToString ( "0.00" ) + " quantity from " + ResourceManager.Instance.GetResourceByID ( 0 ) );
            float r = inventory.RemoveItemQuantity ( 0, ran );
            Debug.Log ( "Returned " + r.ToString ( "0.00" ) + " quantity" );
        }
    }

}
