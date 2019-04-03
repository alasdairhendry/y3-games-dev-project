using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WarehouseController : MonoBehaviour {

    public static WarehouseController Instance;
    public List<Prop_Warehouse> warehouses { get; protected set; } = new List<Prop_Warehouse> ();

    public ResourceInventory Inventory { get; private set; }
    private bool addedStartingResources = false;

    public Dictionary<int, float> dailyResources { get; protected set; } = new Dictionary<int, float> ();
    public System.Action<Dictionary<int, float>> onDailyResourcesChanged;

    private void Awake ()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this)
        {
            Destroy ( this.gameObject );
            return;
        }

        Inventory = new ResourceInventory ( float.MaxValue );

        for (int i = 0; i < ResourceManager.Instance.GetResourceList().Count; i++)
        {
            dailyResources.Add ( ResourceManager.Instance.GetResourceList ()[i].id, 0.0f );
        }

        Inventory.RegisterOnResourceAdded ( OnResourceAdded );
        Inventory.RegisterOnResourceRemoved ( OnResourceRemoved );
        GameTime.onDayChanged += OnDayChanged;
        //inventory.AddItemQuantity ( 0, 30 );
        //inventory.AddItemQuantity ( 1, 10 );
    }

    private void AddStartingResources ()
    {
        if (GameData.Instance.gameDataType == GameData.GameDataType.Loaded) return;
        if (addedStartingResources) return;

        for (int i = 0; i < GameData.Instance.startingConditions.startingResources.Count; i++)
        {
            Inventory.AddItemQuantity ( GameData.Instance.startingConditions.startingResources[i].Key, GameData.Instance.startingConditions.startingResources[i].Value );
        }

        addedStartingResources = true;
    }

    public void Load (PersistentData.SaveData data)
    {
        //inventory = new ResourceInventory ( float.MaxValue );

        Inventory.CLEAR_ALL ();

        for (int i = 0; i < data.WarehouseResourceIDs.Count; i++)
        {
            Inventory.AddItemQuantity ( data.WarehouseResourceIDs[i], data.WarehouseResourceQuantities[i] );
        }
    }

    public Prop_Warehouse FindWarehouseToHaul (Transform citizenTransform, int resourceID, float resourceQuantity)
    {
        if (!PreFindWarehouseCheck ( citizenTransform )) { return null; }
        if (!Inventory.CheckHasQuantityAvailable ( resourceID, resourceQuantity )) { return null; }

        return FindClosestWarehouse ( citizenTransform );
    }

    public Prop_Warehouse FindWarehouseToHaulAnyQuantity (Transform citizenTransform, int resourceID, float resourceQuantity)
    {
        if (!PreFindWarehouseCheck ( citizenTransform )) { return null; }
        if (Inventory.CheckIsEmpty ( resourceID )) { return null; }

        return FindClosestWarehouse ( citizenTransform );
    }

    public Prop_Warehouse FindWarehouseToStore (Transform citizenTransform, int resourceID, float resourceQuantity)
    {
        if (!PreFindWarehouseCheck ( citizenTransform )) { return null; }
        if (!Inventory.CheckCanHold ( resourceID, resourceQuantity )) { return null; }

        return FindClosestWarehouse ( citizenTransform );
    }

    private bool PreFindWarehouseCheck (Transform citizenTransform)
    {
        if (Inventory == null) { return false; }
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

        AddStartingResources ();
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

    private void OnDayChanged(int p, int c)
    {
        dailyResources.Clear ();

        for (int i = 0; i < ResourceManager.Instance.GetResourceList ().Count; i++)
        {
            dailyResources.Add ( ResourceManager.Instance.GetResourceList ()[i].id, 0.0f );
        }

        onDailyResourcesChanged?.Invoke ( dailyResources );
    }

    private void OnResourceAdded(int id, float q)
    {
        if (dailyResources.ContainsKey ( id ))
            dailyResources[id] += q;
        else dailyResources.Add ( id, q );

        onDailyResourcesChanged?.Invoke ( dailyResources );
    }

    private void OnResourceRemoved(int id, float q)
    {
        if (dailyResources.ContainsKey ( id ))
            dailyResources[id] -= q;
        else dailyResources.Add ( id, -q );

        Debug.Log ( "Daily resource removed " + ResourceManager.Instance.GetResourceByID ( id ).name + " // " + q );

        onDailyResourcesChanged?.Invoke ( dailyResources );
    }
}
