using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceInventory {

    public Dictionary<int, float> inventory { get; private set; }
    private float entryCapacity = 32.0f;

    public ResourceInventory (float entryCapacity = 32.0f)
    {
        this.entryCapacity = entryCapacity;
        inventory = new Dictionary<int, float> ();
        List<Resource> resources = ResourceManager.Instance.GetResourceList ();

        for (int i = 0; i < resources.Count; i++)
        {
            inventory.Add ( resources[i].id, 0.0f );            
        }
    }

    public bool CheckIsFull(int itemID)
    {
        if (!inventory.ContainsKey ( itemID )) { Debug.LogError ( "Item does not exist" ); return true; }
        return inventory[itemID] >= entryCapacity ? true : false;
    }

    public bool CheckIsEmpty(int itemID)
    {
        if (!inventory.ContainsKey ( itemID )) { Debug.LogError ( "Item does not exist" ); return true; }
        return inventory[itemID] <= 0 ? true : false;
    }

    public bool CheckHasQuantity(int itemID, float quantity)
    {
        if (!inventory.ContainsKey ( itemID )) { Debug.LogError ( "Item does not exist" ); return false; }
        return inventory[itemID] >= quantity ? true : false;
    }

    // Return however much quantity we cannot store
    public float AddItemQuantity(int itemID, float quantity)
    {
        if (!inventory.ContainsKey ( itemID )) { Debug.LogError ( "Item does not exist" ); return quantity; }
        float spaceAvailable = entryCapacity - inventory[itemID];
        float quantityToAdd = 0.0f;

        if(spaceAvailable >= quantity)
        {
            quantityToAdd = quantity;
            quantity = 0;
        }
        else if(spaceAvailable < quantity)
        {
            quantityToAdd = spaceAvailable;
            quantity -= spaceAvailable;
        }

        inventory[itemID] += quantityToAdd;        
        return quantity;
    }

    // Return however much we have available, up to a maximum of the quantity requested
    public float RemoveItemQuantity(int itemID, float quantity)
    {
        if (!inventory.ContainsKey ( itemID )) { Debug.LogError ( "Item does not exist" ); return 0.0f; }
        float quantityAvailable = inventory[itemID];

        if(quantityAvailable >= quantity)
        {
            quantityAvailable -= quantity;
        }
        else if(quantityAvailable < quantity)
        {
            quantity = quantityAvailable;
            quantityAvailable = 0.0f;
        }

        inventory[itemID] -= quantity;        
        return quantity;
    }	
}
