using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WarehouseController : MonoBehaviour {

    public static WarehouseController Instance;
    [SerializeField] private List<Prop_Warehouse> warehouses = new List<Prop_Warehouse> ();

    private ResourceInventory inventory;
    public ResourceInventory Inventory { get { return inventory; } }

    private void Awake ()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this)
        {
            Destroy ( this.gameObject );
            return;
        }

        inventory = new ResourceInventory ( float.MaxValue );
        inventory.AddItemQuantity ( 0, 30 );
        inventory.AddItemQuantity ( 1, 10 );
    }

    public void Load (PersistentData.SaveData data)
    {
        inventory = new ResourceInventory ( float.MaxValue );

        for (int i = 0; i < data.WarehouseResourceIDs.Count; i++)
        {
            inventory.AddItemQuantity ( data.WarehouseResourceIDs[i], data.WarehouseResourceQuantities[i] );
        }
    }

    public Prop_Warehouse FindWarehouseToHaul (Transform citizenTransform, int resourceID, float resourceQuantity)
    {
        if (!PreFindWarehouseCheck ( citizenTransform )) { return null; }
        if (!inventory.CheckHasQuantityAvailable ( resourceID, resourceQuantity )) { return null; }

        return FindClosestWarehouse ( citizenTransform );
    }

    public Prop_Warehouse FindWarehouseToHaulAnyQuantity (Transform citizenTransform, int resourceID, float resourceQuantity)
    {
        if (!PreFindWarehouseCheck ( citizenTransform )) { return null; }
        if (inventory.CheckIsEmpty ( resourceID )) { return null; }

        return FindClosestWarehouse ( citizenTransform );
    }

    public Prop_Warehouse FindWarehouseToStore (Transform citizenTransform, int resourceID, float resourceQuantity)
    {
        if (!PreFindWarehouseCheck ( citizenTransform )) { return null; }
        if (!inventory.CheckCanHold ( resourceID, resourceQuantity )) { return null; }

        return FindClosestWarehouse ( citizenTransform );
    }

    private bool PreFindWarehouseCheck (Transform citizenTransform)
    {
        if (inventory == null) { return false; }
        if (warehouses.Count <= 0) { return false; }
        if (citizenTransform == null) { return false; }

        return true;
    }

    private Prop_Warehouse FindClosestWarehouse(Transform citizenTransform)
    {
        int fastestPath = -1;
        float bestDistance = float.MaxValue;

        for (int i = 0; i < warehouses.Count; i++)
        {
            NavMeshPath path = new NavMeshPath ();

            if (citizenTransform == null) { continue; }
            if (warehouses[i] == null) continue;
            if (warehouses[i].buildable.IsComplete == false) { continue; }

            //NavMesh.CalculatePath ( citizenTransform.position, warehouses[i].transform.position, NavMesh.GetAreaFromName ( "Walkable" ), path );
            float distance = Vector3.Distance ( citizenTransform.position, warehouses[i].transform.position );

            //float f = GetPathLength ( path );
            if (distance <= bestDistance)
            {
                //Debug.Log ( "Was - " + bestDistance + " : Now - " + f );
                bestDistance = distance;
                fastestPath = i;
            }
        }

        if (fastestPath == -1) { Debug.LogError ( "No Eligible Path. We also shouldnt run this every frame." ); return null; }

        return warehouses[fastestPath];
    }

    protected float GetPathLength (NavMeshPath path)
    {
        float length = float.MaxValue;
        Debug.Log ( path.status );
        if (path.status != NavMeshPathStatus.PathComplete) return length;

        for (int i = 1; i < path.corners.Length; i++)
        {
            length += Vector3.Distance ( path.corners[i - 1], path.corners[i] );
        }

        return length;
    }

    public void AddWarehouse (Prop_Warehouse warehouse)
    {
        if (warehouses.Contains ( warehouse ))
        {
            Debug.LogError ( "Warehouse already exists in our list" );
            return;
        }

        warehouses.Add ( warehouse );
    }

    public void RemoveWarehouse (Prop_Warehouse warehouse)
    {
        if (!warehouses.Contains ( warehouse ))
        {
            Debug.LogError ( "Warehouse doesnt exist in our list" );
            return;
        }

        warehouses.Remove ( warehouse );
    }   
}
