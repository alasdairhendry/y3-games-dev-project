using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class Job_GatherResource : Job {

    private RawMaterial rawMaterial;
    private int resourceID;
    private float resourceQuantity;
    private float currentTime = 0.0f;

    public enum Stage { TravelTo, Remove, FindWarehouse, TravelFrom }
    private Stage stage = Stage.TravelTo;

    private bool destinationProvided = false;
    private bool givenResourceToCitizen = false;

    private Prop_Warehouse targetWarehouse;

    public Job_GatherResource (string name, bool open, int resourceID, float resourceQuantity, float timeRequired, RawMaterial rawMaterial)
    {
        this.id = JobController.GetNewJobID ();
        base.Name = name;
        base.Open = open;

        this.resourceID = resourceID;
        this.resourceQuantity = resourceQuantity;
        this.rawMaterial = rawMaterial;
        this.TimeRequired = timeRequired;
    }

    public override void DoJob (float deltaGameTime)
    {
        switch (stage)
        {
            case Stage.TravelTo:
                DoJob_Stage_TravelTo ();
                break;


            case Stage.Remove:
                DoJob_Stage_Remove ( deltaGameTime );
                break;

            case Stage.FindWarehouse:
                DoJob_Stage_FindWarehouse ();
                break;

            case Stage.TravelFrom:
                DoJob_Stage_TravelFrom ();
                break;
        }
    }

    public override void OnCharacterAccept (Character character)
    {
        base.OnCharacterAccept ( character );

        if(!base.character.Inventory.CheckCanHold(resourceID, resourceQuantity ))
        {
            OnCharacterLeave ( "Citizen can't hold that many resources" );
        }
    }

    public override void OnCharacterLeave (string reason)
    {
        if (givenResourceToCitizen)
        {
            base.character.Inventory.RemoveItemQuantity ( resourceID, resourceQuantity );
        }

        stage = Stage.TravelTo;
        currentTime = 0.0f;
        destinationProvided = false;
        targetWarehouse = null;
        givenResourceToCitizen = false;


        base.OnCharacterLeave ( reason );
    }

    private void DoJob_Stage_TravelTo ()
    {
        if(!destinationProvided)
        {
            base.character.agent.SetDestination ( rawMaterial.transform.position + new Vector3 ( 0.0f, 0.0f, -3.0f ) );
            destinationProvided = true;
        }

        if (!ReachedPath ()) return;
        destinationProvided = false;

        stage = Stage.Remove;
    }

    private void DoJob_Stage_Remove (float deltaGameTime)
    {
        currentTime += deltaGameTime;

        if(currentTime>= base.TimeRequired)
        {            
            stage = Stage.FindWarehouse;
            base.character.Inventory.AddItemQuantity ( resourceID, resourceQuantity );
            rawMaterial.Gather ();
            givenResourceToCitizen = true;
        }
    }

    private void DoJob_Stage_FindWarehouse ()
    {
        targetWarehouse = FindEligibleWarehouse ();

        if (targetWarehouse == null)
        {
            if (givenResourceToCitizen)
                character.Inventory.RemoveItemQuantity ( resourceID, resourceQuantity );

            base.OnComplete ();
        }
        else
        {
            stage = Stage.TravelFrom;
        }
    }

    private void DoJob_Stage_TravelFrom ()
    {
        if (!destinationProvided)
        {
            base.character.agent.SetDestination ( targetWarehouse.CitizenInteractionPointGlobal );
            destinationProvided = true;
        }

        if (!ReachedPath ()) return;

        targetWarehouse.inventory.AddItemQuantity ( resourceID, resourceQuantity );

        if (givenResourceToCitizen)
            character.Inventory.RemoveItemQuantity ( resourceID, resourceQuantity );

        givenResourceToCitizen = false;
        destinationProvided = false;

        base.OnComplete ();
    }

    private Prop_Warehouse FindEligibleWarehouse ()
    {
        List<GameObject> GOs = PropManager.Instance.worldProps;

        if (GOs == null) { Debug.LogError ( "No Warehouses found. We shouldnt run this every frame" ); return null; }

        List<Prop_Warehouse> eligibleWarehouses = new List<Prop_Warehouse> ();

        for (int i = 0; i < GOs.Count; i++)
        {
            Prop_Warehouse w = GOs[i].GetComponent<Prop_Warehouse> ();

            if (w == null) { continue; }
            if (w.inventory == null) { Debug.LogError ( "Warehouse inventory is null" ); continue; }

            if (w.inventory.CheckCanHold ( resourceID, resourceQuantity ))
            {
                eligibleWarehouses.Add ( w );
            }
            else
            {
                Debug.LogError ( "Warehouse inventory can't hold that many resources" ); continue;
            }
        }

        // TODO: Maybe add some sort of tick system so this doesnt clog up the players client
        if (eligibleWarehouses.Count <= 0) { Debug.LogError ( "No Eligible Warehouses. We shouldnt run this every frame" ); return null; }

        int fastestPath = -1;
        float bestDistance = float.MaxValue;

        for (int i = 0; i < eligibleWarehouses.Count; i++)
        {
            NavMeshPath path = new NavMeshPath ();

            if (base.character == null) continue;
            if (eligibleWarehouses[i] == null) continue;

            NavMesh.CalculatePath ( base.character.transform.position, eligibleWarehouses[i].transform.position, 0, path );

            float f = GetPathLength ( path );
            if (f <= bestDistance)
            {
                bestDistance = f;
                fastestPath = i;
            }
        }

        if (fastestPath == -1) { Debug.LogError ( "No Eligible Path. We also shouldnt run this every frame." ); return null; }

        return eligibleWarehouses[fastestPath];
    }
}
