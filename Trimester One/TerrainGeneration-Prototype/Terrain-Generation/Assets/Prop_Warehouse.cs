using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop_Warehouse : Prop {

    public ResourceInventory inventory;

    private void Start ()
    {        
        inventory = new ResourceInventory ();
        inventory.AddItemQuantity ( 0, 10 );
        inventory.AddItemQuantity ( 1, 2 );
    }
}
