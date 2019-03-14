using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceInventory {

    public Dictionary<int, float> inventoryOverall { get; private set; }
    private Dictionary<int, float> inventoryAvailable;
    private Dictionary<int, float> inventoryReserved;

    private System.Action<ResourceInventory> OnInventoryChanged;
    private System.Action<int, float> onResourceAdded;

    public float EntryCapacity { get; protected set; }

    public ResourceInventory (float entryCapacity = 32.0f)
    {
        this.EntryCapacity = entryCapacity;
        inventoryOverall = new Dictionary<int, float> ();
        inventoryAvailable = new Dictionary<int, float> ();
        inventoryReserved = new Dictionary<int, float> ();

        List<Resource> resources = ResourceManager.Instance.GetResourceList ();

        for (int i = 0; i < resources.Count; i++)
        {
            inventoryOverall.Add ( resources[i].id, 0.0f );
            inventoryAvailable.Add ( resources[i].id, 0.0f );
            inventoryReserved.Add ( resources[i].id, 0.0f );
        }
    }

    public bool CheckIsFull(int itemID)
    {
        if (!inventoryOverall.ContainsKey ( itemID )) { Debug.LogError ( "Item does not exist" ); return true; }
        return inventoryOverall[itemID] >= EntryCapacity ? true : false;
    }

    public bool CheckIsEmpty(int itemID)
    {
        if (!inventoryOverall.ContainsKey ( itemID )) { Debug.LogError ( "Item does not exist" ); return true; }
        return inventoryOverall[itemID] <= 0 ? true : false;
    }

    public bool CheckCanHold(int itemID, float quantity)
    {
        if (!inventoryOverall.ContainsKey ( itemID )) { Debug.LogError ( "Item does not exist" ); return false; }

        if (inventoryOverall[itemID] + quantity > EntryCapacity) return false;
        else return true;
    }

    public float GetAvailableQuantity(int itemID)
    {
        if (!inventoryAvailable.ContainsKey ( itemID )) { Debug.LogError ( "Item does not exist" ); return -1; }
        return inventoryAvailable[itemID];
    }

    public float GetAvailableSpace(int itemID)
    {
        if (!inventoryOverall.ContainsKey ( itemID )) { Debug.LogError ( "Item does not exist" ); return -1; }
        return EntryCapacity - inventoryOverall[itemID];
    }

    public bool CheckHasQuantityAvailable(int itemID, float quantity)
    {
        if (!inventoryAvailable.ContainsKey ( itemID )) { Debug.LogError ( "Item does not exist" ); return false; }
        return inventoryAvailable[itemID] >= quantity ? true : false;
    }

    // Return however much quantity we cannot store
    public float AddItemQuantity(int itemID, float quantity)
    {
        if (!inventoryAvailable.ContainsKey ( itemID )) { Debug.LogError ( "Item does not exist" ); return quantity; }
        float spaceAvailable = EntryCapacity - inventoryAvailable[itemID];
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

        inventoryAvailable[itemID] += quantityToAdd;
        if (onResourceAdded != null) onResourceAdded ( itemID, quantityToAdd );

        SetOverallInventory ();

        return quantity;
    }

    // Return however much we have available, up to a maximum of the quantity requested
    public float RemoveItemQuantity(int itemID, float quantity)
    {
        if (!inventoryAvailable.ContainsKey ( itemID )) { Debug.LogError ( "Item does not exist" ); return 0.0f; }
        float quantityAvailable = inventoryAvailable[itemID];

        if(quantityAvailable >= quantity)
        {
            quantityAvailable -= quantity;
        }
        else if(quantityAvailable < quantity)
        {
            quantity = quantityAvailable;
            quantityAvailable = 0.0f;
        }

        inventoryAvailable[itemID] -= quantity;

        SetOverallInventory ();

        return quantity;
    }	

    public bool ReserveItemQuantity(int itemID, float quantity)
    {
        if (!inventoryAvailable.ContainsKey ( itemID )) { Debug.LogError ( "Item does not exist in available" ); return false; }
        if (!inventoryReserved.ContainsKey ( itemID )) { Debug.LogError ( "Item does not exist in reserved" ); return false; }

        float quantityAvailable = inventoryAvailable[itemID];

        if (quantityAvailable >= quantity)
        {
            quantityAvailable -= quantity;
        }
        else if (quantityAvailable < quantity)
        {
            quantity = quantityAvailable;
            quantityAvailable = 0.0f;
        }

        inventoryAvailable[itemID] -= quantity;
        inventoryReserved[itemID] += quantity;

        SetOverallInventory ();

        return true;
    }

    public bool UnreserveItemQuantity(int itemID, float quantity)
    {
        if (!inventoryAvailable.ContainsKey ( itemID )) { Debug.LogError ( "Item does not exist in available" ); return false; }
        if (!inventoryReserved.ContainsKey ( itemID )) { Debug.LogError ( "Item does not exist in reserved" ); return false; }

        float quantityAvailable = inventoryReserved[itemID];

        if (quantityAvailable >= quantity)
        {
            quantityAvailable -= quantity;
        }
        else if (quantityAvailable < quantity)
        {
            quantity = quantityAvailable;
            quantityAvailable = 0.0f;
        }

        inventoryReserved[itemID] -= quantity;
        inventoryAvailable[itemID] += quantity;

        SetOverallInventory ();

        return true;
    }

    public float TakeReservedItemQuantity(int itemID, float quantity)
    {
        if (!inventoryReserved.ContainsKey ( itemID )) { Debug.LogError ( "Item does not exist" ); return 0.0f; }
        float quantityAvailable = inventoryReserved[itemID];

        if (quantityAvailable >= quantity)
        {
            quantityAvailable -= quantity;
        }
        else if (quantityAvailable < quantity)
        {
            quantity = quantityAvailable;
            quantityAvailable = 0.0f;
        }

        inventoryReserved[itemID] -= quantity;

        SetOverallInventory ();

        return quantity;
    }

    private void SetOverallInventory ()
    {        
        List<int> ints = inventoryOverall.Keys.ToList ();        

        for (int i = 0; i < ints.Count; i++)
        {
            float amount = 0.0f;
            amount += inventoryAvailable[ints[i]];
            amount += inventoryReserved[ints[i]];
            inventoryOverall[ints[i]] = amount;
        }

        if (OnInventoryChanged != null) OnInventoryChanged ( this );
    }

    public Resource GetResourceByID(int itemID)
    {
        return ResourceManager.Instance.GetResourceByID ( itemID );
    }

    public bool IsEmpty ()
    {
        bool isEmpty = true;

        for (int i = 0; i < ResourceManager.Instance.GetResourceList().Count; i++)
        {
            if(inventoryOverall[ResourceManager.Instance.GetResourceList()[i].id] > 0)
            {
                isEmpty = false;
                break;
            }
        }

        return isEmpty;
    }

    public void RegisterOnInventoryChanged (System.Action<ResourceInventory> action)
    {
        OnInventoryChanged += action;
    }

    public void UnregisterOnInventoryChanged (System.Action<ResourceInventory> action)
    {
        OnInventoryChanged -= action;
    }

    public void RegisterOnResourceAdded (System.Action<int, float> action)
    {
        onResourceAdded += action;
    }

    public void UnregisterOnResourceAdded (System.Action<int, float> action)
    {
        onResourceAdded -= action;
    }
}
